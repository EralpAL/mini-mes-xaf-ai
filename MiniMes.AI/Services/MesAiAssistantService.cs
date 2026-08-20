using Microsoft.Extensions.AI;
using MiniMes.AI.Interfaces;
using MiniMes.AI.Models;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MiniMes.AI.Services
{
    public class MesAiAssistantService : IMesAiAssistantService
    {
        private const int DefaultListLimit = 10;

        private static readonly Regex ProductionOrderCodePattern = new Regex(@"\bPO-\d+\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex WorkOrderCodePattern = new Regex(@"\bWO-\d+(?:-\d+)*\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly IChatClient _chatClient;
        private readonly IProductionOrderTool _productionOrderTool;
        private readonly IMesDataTool? _mesDataTool;

        public MesAiAssistantService(IChatClient chatClient, IProductionOrderTool productionOrderTool, IMesDataTool? mesDataTool = null)
        {
            _chatClient = chatClient;
            _productionOrderTool = productionOrderTool;
            _mesDataTool = mesDataTool;
        }

        public async Task<AssistantResponse> SendAsync(AssistantRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(request.Message))
            {
                AssistantResponse emptyMessageResponse = new AssistantResponse();
                emptyMessageResponse.IsSuccessful = false;
                emptyMessageResponse.ErrorMessage = "Mesaj boş olamaz.";
                return emptyMessageResponse;
            }

            string usedToolName = string.Empty;
            ProductionOrderDto? productionOrder = null;
            WorkOrderDto? workOrder = null;

            // A code written in the question is resolved from the database before the model runs,
            // so the answer of a record question can never be produced without real data.
            string workOrderCode = ExtractCode(WorkOrderCodePattern, request.Message);

            if (workOrderCode.Length > 0 && _mesDataTool != null)
            {
                workOrder = await _mesDataTool.GetWorkOrderAsync(workOrderCode, cancellationToken);
                usedToolName = "get_work_order";

                if (workOrder == null)
                {
                    return CreateInformationResponse(workOrderCode + " kodlu iş emri bulunamadı.", usedToolName);
                }
            }

            if (workOrder == null)
            {
                string productionOrderCode = ResolveProductionOrderCode(request);

                if (productionOrderCode.Length > 0)
                {
                    productionOrder = await _productionOrderTool.GetAsync(productionOrderCode, cancellationToken);
                    usedToolName = "get_production_order";

                    if (productionOrder == null)
                    {
                        return CreateInformationResponse(productionOrderCode + " kodlu üretim emri bulunamadı.", usedToolName);
                    }
                }
            }

            bool analysisRequested = IsAnalysisRequest(request.Message);
            bool reportRequested = IsReportRequest(request.Message);
            bool recordDataLoaded = productionOrder != null || workOrder != null;

            ChatOptions chatOptions = new ChatOptions();
            chatOptions.Tools = new List<AITool>();

            // Tools are offered only when the answer was not already loaded from the database,
            // which keeps the small local model focused on a single decision.
            if (!recordDataLoaded)
            {
                List<AITool> tools = BuildTools(cancellationToken, delegate (string toolName) { usedToolName = toolName; });

                for (int i = 0; i < tools.Count; i++)
                {
                    chatOptions.Tools.Add(tools[i]);
                }
            }

            List<ChatMessage> messages = new List<ChatMessage>();
            messages.Add(new ChatMessage(ChatRole.System, BuildSystemPrompt(recordDataLoaded, analysisRequested, reportRequested)));

            if (productionOrder != null)
            {
                messages.Add(new ChatMessage(ChatRole.System, BuildDataMessage("üretim emri", productionOrder)));
            }

            if (workOrder != null)
            {
                messages.Add(new ChatMessage(ChatRole.System, BuildDataMessage("iş emri", workOrder)));
            }

            messages.Add(new ChatMessage(ChatRole.User, request.Message));

            try
            {
                ChatResponse chatResponse = await _chatClient.GetResponseAsync(messages, chatOptions, cancellationToken);

                string assistantText = chatResponse.Text ?? string.Empty;

                AssistantResponse successResponse = new AssistantResponse();
                successResponse.Message = assistantText;
                successResponse.IsSuccessful = true;

                if (productionOrder != null)
                {
                    successResponse.Report = productionOrder.ToReport(assistantText);
                }

                if (workOrder != null)
                {
                    successResponse.WorkOrderReport = workOrder.ToReport(assistantText);
                }

                if (usedToolName.Length > 0)
                {
                    successResponse.UsedToolName = usedToolName;
                }

                return successResponse;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                WriteDevelopmentException(exception);

                AssistantResponse errorResponse = new AssistantResponse();
                errorResponse.IsSuccessful = false;
                errorResponse.ErrorMessage = BuildUserErrorMessage(exception);
                return errorResponse;
            }
        }

        private List<AITool> BuildTools(CancellationToken cancellationToken, Action<string> toolUsed)
        {
            List<AITool> tools = new List<AITool>();

            tools.Add(AIFunctionFactory.Create(
                (string code) =>
                {
                    toolUsed("get_production_order");
                    return _productionOrderTool.GetAsync(code, cancellationToken);
                },
                "get_production_order",
                "Üretim emri kodu (örnek PO-15) ile üretim emrini, iş emirlerini ve üretim girişlerini getirir."));

            if (_mesDataTool == null)
            {
                return tools;
            }

            IMesDataTool dataTool = _mesDataTool;

            tools.Add(AIFunctionFactory.Create(
                () =>
                {
                    toolUsed("get_mes_summary");
                    return dataTool.GetSummaryAsync(cancellationToken);
                },
                "get_mes_summary",
                "Genel MES özeti: üretim emri ve iş emri sayıları, durum dağılımı, toplam planlanan, üretilen ve fire miktarları, iş istasyonu ve personel sayıları."));

            tools.Add(AIFunctionFactory.Create(
                (string status) =>
                {
                    toolUsed("list_production_orders");
                    return dataTool.FindProductionOrdersAsync(status, DefaultListLimit, cancellationToken);
                },
                "list_production_orders",
                "Üretim emirlerini listeler. status parametresi Planned, InProgress, Completed veya Canceled olabilir; boş bırakılırsa tüm durumlar listelenir."));

            tools.Add(AIFunctionFactory.Create(
                () =>
                {
                    toolUsed("list_highest_scrap_production_orders");
                    return dataTool.GetHighestScrapProductionOrdersAsync(DefaultListLimit, cancellationToken);
                },
                "list_highest_scrap_production_orders",
                "Fire miktarı en yüksek üretim emirlerini büyükten küçüğe listeler."));

            tools.Add(AIFunctionFactory.Create(
                (string code) =>
                {
                    toolUsed("get_work_order");
                    return dataTool.GetWorkOrderAsync(code, cancellationToken);
                },
                "get_work_order",
                "İş emri kodu ile iş emrini, üretim girişlerini ve duruş kayıtlarını getirir."));

            tools.Add(AIFunctionFactory.Create(
                (string status) =>
                {
                    toolUsed("list_work_orders");
                    return dataTool.FindWorkOrdersAsync(status, DefaultListLimit, cancellationToken);
                },
                "list_work_orders",
                "İş emirlerini listeler. status parametresi Planned, InProgress, Completed veya Stopped olabilir; boş bırakılırsa tüm durumlar listelenir."));

            tools.Add(AIFunctionFactory.Create(
                (string nameOrCode) =>
                {
                    toolUsed("get_work_station");
                    return dataTool.GetWorkStationAsync(nameOrCode, cancellationToken);
                },
                "get_work_station",
                "İş istasyonunu kodu veya adı ile getirir: aktiflik, saatlik maliyet, ekipman sayısı, iş emri durum sayıları, duruş ve bakım sayıları."));

            tools.Add(AIFunctionFactory.Create(
                (bool onlyActive) =>
                {
                    toolUsed("list_work_stations");
                    return dataTool.FindWorkStationsAsync(onlyActive, DefaultListLimit, cancellationToken);
                },
                "list_work_stations",
                "İş istasyonlarını listeler. onlyActive true verilirse yalnızca aktif istasyonlar listelenir."));

            tools.Add(AIFunctionFactory.Create(
                (string nameOrCode) =>
                {
                    toolUsed("get_stock_card");
                    return dataTool.GetStockCardAsync(nameOrCode, cancellationToken);
                },
                "get_stock_card",
                "Stok kartını kodu veya adı ile getirir: stok tipi, depo, rotalar ve üretim emirleri."));

            tools.Add(AIFunctionFactory.Create(
                (string search) =>
                {
                    toolUsed("get_routing");
                    return dataTool.GetRoutingAsync(search, cancellationToken);
                },
                "get_routing",
                "Rotayı kodu, adı veya stok kartı adı ile getirir ve rota adımlarını sıra numarasına göre döndürür."));

            tools.Add(AIFunctionFactory.Create(
                (string search) =>
                {
                    toolUsed("list_employees");
                    return dataTool.FindEmployeesAsync(search, DefaultListLimit, cancellationToken);
                },
                "list_employees",
                "Çalışanları listeler. search parametresine sicil numarası veya ad yazılabilir; boş bırakılırsa ilk çalışanlar listelenir."));

            tools.Add(AIFunctionFactory.Create(
                () =>
                {
                    toolUsed("list_shifts");
                    return dataTool.GetShiftsAsync(cancellationToken);
                },
                "list_shifts",
                "Vardiyaları listeler: vardiya adı, başlangıç ve bitiş saati, aktiflik."));

            tools.Add(AIFunctionFactory.Create(
                () =>
                {
                    toolUsed("list_job_roles");
                    return dataTool.GetJobRolesAsync(cancellationToken);
                },
                "list_job_roles",
                "Tanımlı görevleri listeler."));

            tools.Add(AIFunctionFactory.Create(
                (int lastDays) =>
                {
                    toolUsed("list_downtime_logs");
                    return dataTool.FindDowntimeLogsAsync(lastDays, DefaultListLimit, cancellationToken);
                },
                "list_downtime_logs",
                "Duruş kayıtlarını yeniden eskiye listeler. lastDays 1 verilirse bugünkü kayıtlar, 0 verilirse tarih filtresi olmadan son kayıtlar döner."));

            tools.Add(AIFunctionFactory.Create(
                (int lastDays) =>
                {
                    toolUsed("list_maintenance_logs");
                    return dataTool.FindMaintenanceLogsAsync(lastDays, DefaultListLimit, cancellationToken);
                },
                "list_maintenance_logs",
                "Bakım kayıtlarını yeniden eskiye listeler. lastDays 1 verilirse bugünkü kayıtlar, 0 verilirse tarih filtresi olmadan son kayıtlar döner."));

            tools.Add(AIFunctionFactory.Create(
                (string workStation) =>
                {
                    toolUsed("list_equipment");
                    return dataTool.FindEquipmentAsync(workStation, DefaultListLimit, cancellationToken);
                },
                "list_equipment",
                "Ekipmanları listeler. workStation parametresine iş istasyonu kodu veya adı yazılabilir."));

            tools.Add(AIFunctionFactory.Create(
                () =>
                {
                    toolUsed("list_warehouses");
                    return dataTool.GetWarehousesAsync(cancellationToken);
                },
                "list_warehouses",
                "Depoları ve her deponun stok kartı sayısını listeler."));

            return tools;
        }

        private string BuildSystemPrompt(bool recordDataLoaded, bool analysisRequested, bool reportRequested)
        {
            string prompt = "Sen Mini-MES üretim yönetim sistemi için çalışan kurumsal bir veri asistanısın. " +
                "MiniMes veritabanı ile ilgili her soruda cevabını yalnızca araçlardan veya sana verilen veri bloğundan gelen gerçek kayıtlara dayandır. " +
                "Miktar, durum, tarih, süre, çalışan, iş istasyonu, gecikme nedeni gibi hiçbir değeri uydurma. " +
                "Kayıt bulunamazsa kaydın bulunamadığını açıkça söyle. " +
                "Kesin bir hesaplama için gereken alan veri modelinde yoksa bunu açıkça belirt ve tahmin üretme. " +
                "Veritabanından gelen kesin bilgiyi kendi yorumundan ayır: yorum yaparken bunun bir değerlendirme olduğunu belirt. " +
                "Veritabanı gerektirmeyen genel sohbet sorularında araç kullanmak zorunda değilsin. " +
                "Kısa, sade ve Türkçe cevap ver.";

            if (recordDataLoaded)
            {
                prompt = prompt + " İstenen kayıt zaten yüklendi ve aşağıda veri bloğu olarak verildi; araç çağırmadan bu veriyi kullan.";
            }
            else
            {
                prompt = prompt + " Veritabanı sorusu geldiğinde uygun aracı çağır ve aracın döndürdüğü kayıtları kullan.";
            }

            if (analysisRequested)
            {
                prompt = prompt + " Analiz isteniyor: planlanan miktar, üretilen miktar, kalan miktar, fire miktarı, tamamlanma yüzdesi, üretim emri durumu, " +
                    "iş emri durumları, tamamlanmamış iş emirleri, üretim girişi toplamları ve varsa duruş süreleri üzerinden ilerleme ve gecikme riskini değerlendir.";
            }

            if (reportRequested)
            {
                prompt = prompt + " Rapor isteniyor: verilen metrikleri başlıklar halinde düzenli bir metin raporu olarak özetle. Grafik veya tablo dosyası üretme.";
            }

            // The domain model has no planned dates or entry timestamps, so the model must not
            // produce day based delay figures.
            prompt = prompt + " Veri modelinde üretim emri için planlanan başlangıç/bitiş tarihi, iş emri için planlanan miktar ve üretim girişi için zaman damgası bulunmuyor. " +
                "Bu nedenle gün cinsinden gecikme, süre veya termin hesabı yapma; sadece miktar, durum ve varsa duruş süresi bilgilerine dayanan bir değerlendirme yap.";

            return prompt;
        }

        private string BuildDataMessage(string recordName, object data)
        {
            string json = JsonSerializer.Serialize(data);
            return "Aşağıdaki " + recordName + " verisi MiniMes veritabanından alınmıştır. Cevabını yalnızca bu veriye dayandır:\n" + json;
        }

        private string ResolveProductionOrderCode(AssistantRequest request)
        {
            string messageCode = ExtractCode(ProductionOrderCodePattern, request.Message);

            if (messageCode.Length > 0)
            {
                return messageCode;
            }

            if (string.IsNullOrWhiteSpace(request.ProductionOrderCode))
            {
                return string.Empty;
            }

            return request.ProductionOrderCode.Trim().ToUpperInvariant();
        }

        private string ExtractCode(Regex pattern, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return string.Empty;
            }

            Match match = pattern.Match(message);

            if (!match.Success)
            {
                return string.Empty;
            }

            return match.Value.ToUpperInvariant();
        }

        private bool IsAnalysisRequest(string message)
        {
            string normalized = message.ToLowerInvariant();
            return normalized.Contains("analiz") ||
                normalized.Contains("gecik") ||
                normalized.Contains("ilerleme") ||
                normalized.Contains("özetle") ||
                normalized.Contains("değerlendir");
        }

        private bool IsReportRequest(string message)
        {
            string normalized = message.ToLowerInvariant();
            return normalized.Contains("rapor");
        }

        private AssistantResponse CreateInformationResponse(string message, string usedToolName)
        {
            AssistantResponse response = new AssistantResponse();
            response.IsSuccessful = true;
            response.Message = message;

            if (!string.IsNullOrEmpty(usedToolName))
            {
                response.UsedToolName = usedToolName;
            }

            return response;
        }

        private void WriteDevelopmentException(Exception exception)
        {
            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            if (!string.Equals(environmentName, "Development", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string exceptionText = exception.ToString();
            System.Diagnostics.Debug.WriteLine(exceptionText);
            Console.WriteLine(exceptionText);

            Exception innerException = exception.InnerException;
            while (innerException != null)
            {
                string innerExceptionText = innerException.ToString();
                System.Diagnostics.Debug.WriteLine(innerExceptionText);
                Console.WriteLine(innerExceptionText);
                innerException = innerException.InnerException;
            }
        }

        private string BuildUserErrorMessage(Exception exception)
        {
            string technicalMessage = GetFullExceptionMessage(exception);

            if (ContainsOutOfMemoryError(technicalMessage))
            {
                return "Yapay zeka modeli yüklenemedi. Ollama bellek yetersiz: " + technicalMessage;
            }

            return "Yapay zeka servisi yanıt veremedi: " + technicalMessage;
        }

        private string GetFullExceptionMessage(Exception exception)
        {
            string message = exception.Message;
            Exception innerException = exception.InnerException;

            while (innerException != null)
            {
                if (!string.IsNullOrWhiteSpace(innerException.Message) &&
                    message.IndexOf(innerException.Message, StringComparison.Ordinal) < 0)
                {
                    message = message + " | " + innerException.Message;
                }

                innerException = innerException.InnerException;
            }

            return message;
        }

        private bool ContainsOutOfMemoryError(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return false;
            }

            string normalized = message.ToLowerInvariant();
            return normalized.Contains("out of memory") ||
                normalized.Contains("cudamalloc") ||
                normalized.Contains("unable to allocate");
        }
    }
}
