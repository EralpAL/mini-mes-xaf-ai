using Microsoft.Extensions.AI;
using MiniMes.AI.Interfaces;
using MiniMes.AI.Models;

namespace MiniMes.AI.Services
{
    public class MesAiAssistantService : IMesAiAssistantService
    {
        private readonly IChatClient _chatClient;
        private readonly IProductionOrderTool _productionOrderTool;

        public MesAiAssistantService(IChatClient chatClient, IProductionOrderTool productionOrderTool)
        {
            _chatClient = chatClient;
            _productionOrderTool = productionOrderTool;
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

            bool productionOrderToolUsed = false;

            AIFunction getProductionOrderFunction = AIFunctionFactory.Create(
                (string code) =>
                {
                    productionOrderToolUsed = true;
                    return _productionOrderTool.GetAsync(code, cancellationToken);
                },
                "get_production_order",
                "Verilen üretim emri koduna göre Mini-MES üretim emri bilgilerini getirir.");

            ChatOptions chatOptions = new ChatOptions();
            chatOptions.Tools = new List<AITool>();
            chatOptions.Tools.Add(getProductionOrderFunction);

            List<ChatMessage> messages = new List<ChatMessage>();
            messages.Add(new ChatMessage(ChatRole.System, "Sen Mini-MES üretim sistemi için çalışan kurumsal bir AI asistanısın. Kullanıcı bir üretim emri kodu hakkında bilgi istediğinde mutlaka get_production_order aracını kullan. Araçtan gelmeyen üretim verilerini uydurma. Kayıt bulunamazsa bulunamadığını söyle. Kısa ve Türkçe cevap ver."));
            messages.Add(new ChatMessage(ChatRole.User, request.Message));

            try
            {
                ChatResponse chatResponse = await _chatClient.GetResponseAsync(messages, chatOptions, cancellationToken);

                AssistantResponse successResponse = new AssistantResponse();
                successResponse.Message = chatResponse.Text ?? string.Empty;
                successResponse.IsSuccessful = true;
                if (productionOrderToolUsed)
                {
                    successResponse.UsedToolName = "get_production_order";
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
