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

        private void ProductionOrder_Approve_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ProductionOrder productionOrder = e.CurrentObject as ProductionOrder;

            if (productionOrder == null)
                return;

            XPCollection<RoutingDetail> routingDetails = new XPCollection<RoutingDetail>(productionOrder.Session, CriteriaOperator.Parse("StockCard = ?", productionOrder.StockCard));

            int workOrderNumber = 1;

            foreach (RoutingDetail routingDetail in routingDetails)
            {
                WorkOrder workOrder = new WorkOrder(productionOrder.Session);

                workOrder.ProductionOrder = productionOrder;
                workOrder.Code = productionOrder.Code + "-" + workOrderNumber.ToString("000");
                workOrder.SequenceNumber = routingDetail.SequenceNumber;
                workOrder.Operation = routingDetail.Operation;
                workOrder.AssignedWorkStation = routingDetail.WorkStation;
                workOrder.Status = WorkOrderStatus.Planned;

                workOrder.Save();
                workOrderNumber++;
            }

            productionOrder.Status = ProductionOrderStatus.InProgress;
            productionOrder.Save();

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }
    }
}