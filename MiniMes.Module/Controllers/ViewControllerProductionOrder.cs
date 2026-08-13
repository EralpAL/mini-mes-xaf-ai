using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using MiniMes.Module.BusinessObjects;
using MiniMes.Module.Enums;
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

        private void ProductionOrder_Approve_Execute(
            object sender,
            SimpleActionExecuteEventArgs e)
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
    }
}
