using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.SystemModule;
using Microsoft.Extensions.AI;
using MiniMes.Module.BusinessObjects;
using MiniMes.Module.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace MiniMes.Module.Controllers
{
    public partial class ViewControllerProductionOrder : ViewController
    {
        public ViewControllerProductionOrder()
        {
            InitializeComponent();
            TargetObjectType = typeof(ProductionOrder);
        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }

        private void ProductionOrder_Approve_Execute( object sender, SimpleActionExecuteEventArgs e)
        {
            ProductionOrder productionOrder = e.CurrentObject as ProductionOrder;

            if (productionOrder == null)
            {
                return;
            }

            if (productionOrder.Status != ProductionOrderStatus.Planned)
            {
                throw new UserFriendlyException("Yalnızca planlanan üretim emirleri onaylanabilir.");
            }

            if (productionOrder.StockCard == null)
            {
                throw new UserFriendlyException("Üretim emrinde stok kartı seçilmelidir.");
            }

            if (productionOrder.Routing == null)
            {
                throw new UserFriendlyException("Üretim emrinde rota seçilmelidir.");
            }

            if (productionOrder.Routing.StockCard == null ||
                productionOrder.Routing.StockCard != productionOrder.StockCard)
            {
                throw new UserFriendlyException("Seçilen rota, üretim emrinin stok kartına ait olmalıdır.");
            }

            int existingWorkOrderCount = 0;

            for (int i = 0; i < productionOrder.WorkOrders.Count; i++)
            {
                if (!productionOrder.WorkOrders[i].IsDeleted)
                {
                    existingWorkOrderCount++;
                }
            }

            if (existingWorkOrderCount > 0)
            {
                throw new UserFriendlyException("Bu üretim emri için iş emirleri zaten oluşturulmuş.");
            }

            productionOrder.Routing.RoutingDetails.Reload();

            IList<RoutingDetail> loadedDetails = ObjectSpace.GetObjects<RoutingDetail>( CriteriaOperator.Parse("Routings = ?", productionOrder.Routing));

            List<RoutingDetail> orderedDetails = new List<RoutingDetail>();

            for (int i = 0; i < loadedDetails.Count; i++)
            {
                RoutingDetail routingDetail = loadedDetails[i];

                if (routingDetail != null && !routingDetail.IsDeleted)
                {
                    orderedDetails.Add(routingDetail);
                }
            }

            if (orderedDetails.Count == 0)
            {
                throw new UserFriendlyException("Seçilen rotada rota adımı bulunamadı.");
            }

            orderedDetails.Sort(delegate (RoutingDetail left, RoutingDetail right)
            {
                return left.SequenceNumber.CompareTo(right.SequenceNumber);
            });

            for (int i = 0; i < orderedDetails.Count; i++)
            {
                RoutingDetail routingDetail = orderedDetails[i];

                if (routingDetail.Operation == null)
                {
                    throw new UserFriendlyException("Rota adımlarında operasyon seçilmelidir.");
                }

                if (routingDetail.WorkStation == null)
                {
                    throw new UserFriendlyException("Rota adımlarında iş istasyonu seçilmelidir.");
                }

                if (routingDetail.SequenceNumber <= 0)
                {
                    throw new UserFriendlyException("Rota adımlarının sıra numarası sıfırdan büyük olmalıdır.");
                }

                if (routingDetail.StockCard != null &&
                    routingDetail.StockCard != productionOrder.Routing.StockCard)
                {
                    throw new UserFriendlyException("Rota adımlarının stok kartı, rota başlığındaki stok kartı ile aynı olmalıdır.");
                }
            }

            for (int i = 0; i < orderedDetails.Count; i++)
            {
                for (int j = i + 1; j < orderedDetails.Count; j++)
                {
                    if (orderedDetails[i].SequenceNumber == orderedDetails[j].SequenceNumber)
                    {
                        throw new UserFriendlyException("Aynı rota içerisinde iki adım aynı sıra numarasına sahip olamaz.");
                    }
                }
            }

            int createdWorkOrderCount = 0;

            for (int i = 0; i < orderedDetails.Count; i++)
            {
                RoutingDetail routingDetail = orderedDetails[i];

                WorkOrder workOrder = ObjectSpace.CreateObject<WorkOrder>();

                workOrder.ProductionOrder = productionOrder;
                workOrder.Code = productionOrder.Code + "-" + (i + 1).ToString("000");
                workOrder.SequenceNumber = routingDetail.SequenceNumber;
                workOrder.Operation = routingDetail.Operation;
                workOrder.AssignedWorkStation = routingDetail.WorkStation;
                workOrder.Status = WorkOrderStatus.Planned;

                createdWorkOrderCount++;
            }

            productionOrder.Status = ProductionOrderStatus.InProgress;

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();

            Application.ShowViewStrategy.ShowMessage(
                createdWorkOrderCount.ToString() + " iş emri oluşturuldu.",
                InformationType.Success);
        }

        private async void ProductionOrder_AiDelayAnalysis_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ProductionOrder productionOrder = e.CurrentObject as ProductionOrder;

            if (productionOrder == null)
            {
                throw new UserFriendlyException("Gecikme analizi için bir üretim emri seçilmelidir.");
            }

            try
            {
                if (Application == null || Application.ServiceProvider == null)
                {
                    throw new UserFriendlyException("Yapay zeka servisine erişilemedi.");
                }

                IChatClient chatClient = Application.ServiceProvider.GetService(typeof(IChatClient)) as IChatClient;

                if (chatClient == null)
                {
                    throw new UserFriendlyException("Kayıtlı yapay zeka sohbet servisi bulunamadı.");
                }

                ChatOptions chatOptions = new ChatOptions();
                chatOptions.ModelId = "gemma3:1b";

                string delayPrompt = BuildAiDelayAnalysisPrompt(productionOrder);
                ChatResponse delayResponse = await chatClient.GetResponseAsync(delayPrompt, chatOptions);

                string optimizationPrompt = BuildAiOptimizationRecommendationPrompt(productionOrder);
                ChatResponse optimizationResponse = await chatClient.GetResponseAsync(optimizationPrompt, chatOptions);

                if (delayResponse == null || optimizationResponse == null)
                {
                    throw new UserFriendlyException("Yapay zeka modeli boş yanıt döndürdü.");
                }

                string delayAnalysis = delayResponse.Text;
                string optimizationRecommendation = optimizationResponse.Text;

                if (string.IsNullOrWhiteSpace(delayAnalysis) && string.IsNullOrWhiteSpace(optimizationRecommendation))
                {
                    throw new UserFriendlyException("Yapay zeka modeli boş yanıt döndürdü.");
                }

                if (delayAnalysis != null)
                {
                    delayAnalysis = delayAnalysis.Trim();
                }

                if (optimizationRecommendation != null)
                {
                    optimizationRecommendation = optimizationRecommendation.Trim();
                }

                productionOrder.AiDelayAnalysis = delayAnalysis;
                productionOrder.AiOptimizationRecommendation = optimizationRecommendation;

                AiAnalysisRecord analysisRecord = ObjectSpace.CreateObject<AiAnalysisRecord>();
                analysisRecord.ProductionOrder = productionOrder;
                analysisRecord.AnalysisDate = DateTime.Now;
                analysisRecord.ModelName = "gemma3:1b";
                analysisRecord.AnalysisText = delayAnalysis;
                analysisRecord.OptimizationRecommendation = optimizationRecommendation;

                ObjectSpace.CommitChanges();
                View.Refresh();

                IObjectSpace popupObjectSpace = Application.CreateObjectSpace(typeof(AiAnalysisRecord));
                AiAnalysisRecord recordToShow = popupObjectSpace.GetObject(analysisRecord);

                DetailView detailView = Application.CreateDetailView(popupObjectSpace, recordToShow, true);
                detailView.Caption = "AI Gecikme Analizi";
                detailView.ViewEditMode = ViewEditMode.View;

                e.ShowViewParameters.CreatedView = detailView;
                e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                e.ShowViewParameters.Context = TemplateContext.PopupWindow;

                Application.ShowViewStrategy.ShowViewInPopupWindow(
                    detailView,
                    null,
                    null,
                    "Kapat",
                    null,
                    Frame,
                    delegate (ShowViewParameters showViewParameters)
                    {
                        showViewParameters.TargetWindow = TargetWindow.NewModalWindow;
                        showViewParameters.Context = TemplateContext.PopupWindow;

                        for (int i = 0; i < showViewParameters.Controllers.Count; i++)
                        {
                            DialogController dialogController = showViewParameters.Controllers[i] as DialogController;

                            if (dialogController != null)
                            {
                                dialogController.SaveOnAccept = false;
                                dialogController.CancelAction.Active["AiAnalysisReadOnly"] = false;
                                break;
                            }
                        }
                    });
            }
            catch (UserFriendlyException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw new UserFriendlyException(
                    "Yapay zeka gecikme analizi tamamlanamadı: " + exception.Message);
            }
        }




        private string BuildAiDelayAnalysisPrompt(ProductionOrder productionOrder)
        {
            StringBuilder prompt = new StringBuilder();

            prompt.AppendLine("Kısa, açık ve profesyonel Türkçe gecikme analizi yaz.");
            prompt.AppendLine("Yalnızca miktar sapması, gecikme riski ve darboğazları belirt.");
            prompt.AppendLine("Optimizasyon önerisi yazma.");
            prompt.AppendLine("Başlık yazma. Uzun rapor yazma.");
            prompt.AppendLine("Zaman yorumunu yalnızca vardiya saatleri varsa yap.");
            prompt.AppendLine();
            AppendProductionOrderPromptData(prompt, productionOrder);

            return prompt.ToString();
        }

        private string BuildAiOptimizationRecommendationPrompt(ProductionOrder productionOrder)
        {
            StringBuilder prompt = new StringBuilder();

            prompt.AppendLine("Kısa, açık ve profesyonel Türkçe optimizasyon önerileri yaz.");
            prompt.AppendLine("Yalnızca uygulanabilir önerileri belirt.");
            prompt.AppendLine("Gecikme analizi veya başlık yazma.");
            prompt.AppendLine("Uzun rapor yazma.");
            prompt.AppendLine();
            AppendProductionOrderPromptData(prompt, productionOrder);

            return prompt.ToString();
        }

        private void AppendProductionOrderPromptData(StringBuilder prompt, ProductionOrder productionOrder)
        {
            prompt.AppendLine("Üretim emri:");
            prompt.AppendLine("Kod: " + productionOrder.Code);
            prompt.AppendLine("Durum: " + productionOrder.Status.ToString());
            prompt.AppendLine("Planlanan miktar: " + productionOrder.PlannedQuantity.ToString());
            prompt.AppendLine("Gerçekleşen miktar (ProducedQuantity): " + productionOrder.ProducedQuantity.ToString());
            prompt.AppendLine("Fire miktarı (ScrapQuantity): " + productionOrder.ScrapQuantity.ToString());
            prompt.AppendLine("Kalan miktar: " + productionOrder.RemainingQuantity.ToString());
            prompt.AppendLine("Tamamlanma yüzdesi: " + productionOrder.CompletionPercentage.ToString("N2"));

            if (productionOrder.StockCard != null)
            {
                prompt.AppendLine("Stok kartı kodu: " + productionOrder.StockCard.Code);
                prompt.AppendLine("Stok kartı adı: " + productionOrder.StockCard.Name);
            }
            else
            {
                prompt.AppendLine("Stok kartı: yok");
            }

            if (productionOrder.Routing != null)
            {
                prompt.AppendLine("Rota kodu: " + productionOrder.Routing.Code);
                prompt.AppendLine("Rota adı: " + productionOrder.Routing.Name);
            }
            else
            {
                prompt.AppendLine("Rota: yok");
            }

            prompt.AppendLine();
            prompt.AppendLine("İş emirleri:");

            productionOrder.WorkOrders.Reload();

            int workOrderCount = 0;

            for (int i = 0; i < productionOrder.WorkOrders.Count; i++)
            {
                WorkOrder workOrder = productionOrder.WorkOrders[i];

                if (workOrder == null || workOrder.IsDeleted)
                {
                    continue;
                }

                workOrderCount++;

                prompt.AppendLine("İş emri " + workOrderCount.ToString() + ":");
                prompt.AppendLine("Kod: " + workOrder.Code);
                prompt.AppendLine("Sıra numarası: " + workOrder.SequenceNumber.ToString());
                prompt.AppendLine("Durum: " + workOrder.Status.ToString());
                prompt.AppendLine("Üretilen miktar (ProducedQuantity): " + workOrder.ProducedQuantity.ToString());
                prompt.AppendLine("Fire miktarı (ScrapQuantity): " + workOrder.ScrapQuantity.ToString());

                if (workOrder.Operation != null)
                {
                    prompt.AppendLine("Operasyon kodu: " + workOrder.Operation.Code);
                    prompt.AppendLine("Operasyon adı: " + workOrder.Operation.Name);
                }
                else
                {
                    prompt.AppendLine("Operasyon: yok");
                }

                if (workOrder.AssignedWorkStation != null)
                {
                    prompt.AppendLine("İş istasyonu kodu: " + workOrder.AssignedWorkStation.Code);
                    prompt.AppendLine("İş istasyonu adı: " + workOrder.AssignedWorkStation.Name);
                }
                else
                {
                    prompt.AppendLine("İş istasyonu: yok");
                }

                if (workOrder.AssignedShift != null)
                {
                    prompt.AppendLine("Vardiya adı: " + workOrder.AssignedShift.ShiftName);
                    prompt.AppendLine("Vardiya başlangıç saati (ShiftTime): " + workOrder.AssignedShift.ShiftTime.ToString());
                    prompt.AppendLine("Vardiya bitiş saati (EndTime): " + workOrder.AssignedShift.EndTime.ToString());
                }
                else
                {
                    prompt.AppendLine("Vardiya: yok");
                }

                workOrder.ProductionEntries.Reload();

                prompt.AppendLine("Üretim girişleri:");

                int productionEntryCount = 0;

                for (int j = 0; j < workOrder.ProductionEntries.Count; j++)
                {
                    ProductionEntry productionEntry = workOrder.ProductionEntries[j];

                    if (productionEntry == null || productionEntry.IsDeleted)
                    {
                        continue;
                    }

                    productionEntryCount++;

                    prompt.AppendLine("Üretim girişi " + productionEntryCount.ToString() + ":");
                    prompt.AppendLine("RealizedAmount: " + productionEntry.RealizedAmount.ToString());
                    prompt.AppendLine("ScrapAmount: " + productionEntry.ScrapAmount.ToString());

                    if (productionEntry.WorkStation != null)
                    {
                        prompt.AppendLine("İş istasyonu kodu: " + productionEntry.WorkStation.Code);
                        prompt.AppendLine("İş istasyonu adı: " + productionEntry.WorkStation.Name);
                    }
                    else
                    {
                        prompt.AppendLine("İş istasyonu: yok");
                    }

                    if (productionEntry.Operator != null)
                    {
                        prompt.AppendLine("Operatör: " + productionEntry.Operator.FullName);
                    }
                    else
                    {
                        prompt.AppendLine("Operatör: yok");
                    }
                }

                if (productionEntryCount == 0)
                {
                    prompt.AppendLine("Bu iş emrine ait üretim girişi yok.");
                }
            }

            if (workOrderCount == 0)
            {
                prompt.AppendLine("Bu üretim emrine ait iş emri yok.");
            }
        }
    }
}
