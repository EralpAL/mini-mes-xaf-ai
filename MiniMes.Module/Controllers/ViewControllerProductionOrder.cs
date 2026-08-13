using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using MiniMes.Module.BusinessObjects;
using MiniMes.Module.Enums;

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
            ProductionOrder productionOrder =
                e.CurrentObject as ProductionOrder;

            if (productionOrder == null)
            {
                return;
            }

            if (productionOrder.StockCard == null)
            {
                throw new UserFriendlyException("Üretim emrinde stok kartı seçilmelidir.");
            }

            XPCollection<RoutingDetail> routingDetails =new XPCollection<RoutingDetail>(productionOrder.Session,CriteriaOperator.Parse("StockCard = ?",productionOrder.StockCard));

            if (routingDetails.Count == 0)
            {
                throw new UserFriendlyException( "Bu stok kartına bağlı rota adımı bulunamadı.");
            }

            int workOrderNumber = 1;

            foreach (RoutingDetail routingDetail in routingDetails)
            {
                if (routingDetail.Operation == null ||
                    routingDetail.WorkStation == null)
                {
                    throw new UserFriendlyException("Rota adımlarında operasyon ve iş istasyonu seçilmelidir.");
                }

                WorkOrder workOrder =ObjectSpace.CreateObject<WorkOrder>();

                workOrder.ProductionOrder = productionOrder;
                workOrder.Code = productionOrder.Code + "-" + workOrderNumber.ToString("000");

                workOrder.SequenceNumber = routingDetail.SequenceNumber;

                workOrder.Operation =   routingDetail.Operation;

                workOrder.AssignedWorkStation = routingDetail.WorkStation;

                workOrder.Status = WorkOrderStatus.Planned;

                workOrderNumber++;
            }

            productionOrder.Status = ProductionOrderStatus.InProgress;

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }
    }
}