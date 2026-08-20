using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using MiniMes.Module.BusinessObjects;
using MiniMes.Module.Enums;
using System;
using System.Collections.Generic;

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

        private void ProductionOrder_Approve_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            List<ProductionOrder> selectedOrders = new List<ProductionOrder>();

            for (int i = 0; i < e.SelectedObjects.Count; i++)
            {
                ProductionOrder selectedOrder = e.SelectedObjects[i] as ProductionOrder;

                if (selectedOrder != null)
                {
                    selectedOrders.Add(selectedOrder);
                }
            }

            if (selectedOrders.Count == 0)
            {
                throw new UserFriendlyException("Onaylamak için en az bir üretim emri seçilmelidir.");
            }

            int approvedCount = 0;
            int createdWorkOrderCount = 0;
            List<string> skippedMessages = new List<string>();

            for (int i = 0; i < selectedOrders.Count; i++)
            {
                string skipReason;
                int workOrdersCreatedForOrder;
                bool approved = TryApproveProductionOrder(selectedOrders[i], out skipReason, out workOrdersCreatedForOrder);

                if (approved)
                {
                    approvedCount++;
                    createdWorkOrderCount += workOrdersCreatedForOrder;
                }
                else
                {
                    skippedMessages.Add(skipReason);
                }
            }

            if (approvedCount > 0)
            {
                ObjectSpace.CommitChanges();
                View.ObjectSpace.Refresh();
            }

            string resultMessage = approvedCount.ToString() + " üretim emri onaylandı. " +
                createdWorkOrderCount.ToString() + " iş emri oluşturuldu.";

            if (skippedMessages.Count > 0)
            {
                resultMessage = resultMessage + " Onaylanamayanlar: ";

                for (int i = 0; i < skippedMessages.Count; i++)
                {
                    if (i > 0)
                    {
                        resultMessage = resultMessage + " ";
                    }

                    resultMessage = resultMessage + skippedMessages[i];
                }
            }

            InformationType informationType = InformationType.Success;

            if (approvedCount == 0)
            {
                informationType = InformationType.Error;
            }
            else if (skippedMessages.Count > 0)
            {
                informationType = InformationType.Warning;
            }

            Application.ShowViewStrategy.ShowMessage(resultMessage, informationType);
        }

        private bool TryApproveProductionOrder(ProductionOrder productionOrder, out string skipReason, out int createdWorkOrderCount)
        {
            createdWorkOrderCount = 0;
            skipReason = string.Empty;

            string orderCode = productionOrder.Code;

            if (string.IsNullOrEmpty(orderCode))
            {
                orderCode = "Üretim emri";
            }

            if (productionOrder.Status != ProductionOrderStatus.Planned)
            {
                skipReason = orderCode + ": Yalnızca planlanan üretim emirleri onaylanabilir.";
                return false;
            }

            if (productionOrder.StockCard == null)
            {
                skipReason = orderCode + ": Üretim emrinde stok kartı seçilmelidir.";
                return false;
            }

            if (productionOrder.Routing == null)
            {
                skipReason = orderCode + ": Üretim emrinde rota seçilmelidir.";
                return false;
            }

            if (productionOrder.Routing.StockCard == null ||
                productionOrder.Routing.StockCard != productionOrder.StockCard)
            {
                skipReason = orderCode + ": Seçilen rota, üretim emrinin stok kartına ait olmalıdır.";
                return false;
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
                skipReason = orderCode + ": Bu üretim emri için iş emirleri zaten oluşturulmuş.";
                return false;
            }

            productionOrder.Routing.RoutingDetails.Reload();

            IList<RoutingDetail> loadedDetails = ObjectSpace.GetObjects<RoutingDetail>(CriteriaOperator.Parse("Routings = ?", productionOrder.Routing));

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
                skipReason = orderCode + ": Seçilen rotada rota adımı bulunamadı.";
                return false;
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
                    skipReason = orderCode + ": Rota adımlarında operasyon seçilmelidir.";
                    return false;
                }

                if (routingDetail.WorkStation == null)
                {
                    skipReason = orderCode + ": Rota adımlarında iş istasyonu seçilmelidir.";
                    return false;
                }

                if (routingDetail.SequenceNumber <= 0)
                {
                    skipReason = orderCode + ": Rota adımlarının sıra numarası sıfırdan büyük olmalıdır.";
                    return false;
                }

                if (routingDetail.StockCard != null &&
                    routingDetail.StockCard != productionOrder.Routing.StockCard)
                {
                    skipReason = orderCode + ": Rota adımlarının stok kartı, rota başlığındaki stok kartı ile aynı olmalıdır.";
                    return false;
                }
            }

            for (int i = 0; i < orderedDetails.Count; i++)
            {
                for (int j = i + 1; j < orderedDetails.Count; j++)
                {
                    if (orderedDetails[i].SequenceNumber == orderedDetails[j].SequenceNumber)
                    {
                        skipReason = orderCode + ": Aynı rota içerisinde iki adım aynı sıra numarasına sahip olamaz.";
                        return false;
                    }
                }
            }

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
            return true;
        }
    }
}
