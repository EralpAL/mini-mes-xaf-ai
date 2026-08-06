using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.Xpo;
using MiniMes.Module.BusinessObjects;
using MiniMes.Module.Enums;
using System.Linq;

namespace MiniMes.Module.Controllers
{
    // Generates WorkOrders from the target StockCard's RoutingDetail steps and moves the
    // ProductionOrder from Planned to InProgress. See AGENTS.md section 8 (Controller Conventions).
    public class ProductionOrderApproveController : ObjectViewController<DetailView, ProductionOrder>
    {
        private readonly SimpleAction approveAction;

        public ProductionOrderApproveController()
        {
            approveAction = new SimpleAction(this, "Approve", PredefinedCategory.Edit)
            {
                Caption = "Approve",
                ConfirmationMessage = "Are you sure you want to approve this Production Order and generate its Work Orders?"
            };
            approveAction.Execute += ApproveAction_Execute;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            UpdateApproveActionState();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            UpdateApproveActionState();
        }

        private void UpdateApproveActionState()
        {
            bool canApprove = ViewCurrentObject != null && ViewCurrentObject.Status == ProductionOrderStatus.Planned;
            approveAction.Active["ProductionOrder_CanApprove"] = canApprove;
        }

        private void ApproveAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ProductionOrder productionOrder = ViewCurrentObject;
            if (productionOrder == null)
            {
                return;
            }

            if (productionOrder.Status != ProductionOrderStatus.Planned)
            {
                throw new UserFriendlyException("Only Production Orders with 'Planned' status can be approved.");
            }

            if (productionOrder.TargetStockCard == null)
            {
                throw new UserFriendlyException("The Production Order does not have a Target Stock Card assigned.");
            }

            if (productionOrder.WorkOrders.Count > 0)
            {
                throw new UserFriendlyException("Work Orders have already been generated for this Production Order.");
            }

            XPCollection<RoutingDetail> routingDetails = productionOrder.TargetStockCard.Routings;
            if (routingDetails == null || routingDetails.Count == 0)
            {
                throw new UserFriendlyException("No routing details were found for the target Stock Card. No Work Orders were created.");
            }

            IObjectSpace objectSpace = View.ObjectSpace;

            foreach (RoutingDetail routingDetail in routingDetails.OrderBy(detail => detail.SequenceNumber))
            {
                WorkOrder workOrder = objectSpace.CreateObject<WorkOrder>();
                workOrder.ProductionOrder = productionOrder;
                workOrder.Operation = routingDetail.Operation;
                workOrder.AssignedWorkStation = routingDetail.WorkStation;
                workOrder.Status = WorkOrderStatus.Planned;
            }

            productionOrder.Status = ProductionOrderStatus.InProgress;

            objectSpace.CommitChanges();

            Application.ShowViewStrategy.ShowMessage(
                string.Format("{0} Work Order(s) created successfully. Production Order approved.", routingDetails.Count),
                InformationType.Success);
        }
    }
}
