using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using MiniMes.Module.BusinessObjects;
using MiniMes.Module.Enums;
using System.Collections.Generic;
using System.Linq;

namespace MiniMes.Module.Controllers
{
    public class ProductionOrderApproveController : ViewController
    {
        private readonly SimpleAction approveAction;

        public ProductionOrderApproveController()
        {
            TargetObjectType = typeof(ProductionOrder);

            approveAction = new SimpleAction(this, "Approve", PredefinedCategory.Edit)
            {
                Caption = "Approve",
                ToolTip = "Generate the Work Orders of the target product's routing and start the order.",
                ConfirmationMessage = "Are you sure you want to approve this Production Order and generate its Work Orders?",
                TargetObjectsCriteria = "Status = 'Planned'",
                ImageName = "Action_Approve",
                SelectionDependencyType = SelectionDependencyType.RequireSingleObject,
                TargetViewType = ViewType.ListView,
                Category = "View"
            };
            approveAction.Execute += ApproveAction_Execute;

        }

        protected override void OnActivated()
        {
            base.OnActivated();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }

        private void ApproveAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ProductionOrder productionOrder = View.CurrentObject as ProductionOrder;

            List<RoutingDetail> routingDetails = new List<RoutingDetail>();

            if (!productionOrder.TargetStockCard.Routings.Any())
                throw new UserFriendlyException("No routing details were found for the target Stock Card. No Work Orders were created.");


            foreach (RoutingDetail item in productionOrder.TargetStockCard.Routings)
            {
                RoutingDetail routingDetail = item;
                routingDetails.Add(routingDetail);
            }

            IObjectSpace objectSpace = View.ObjectSpace;
            int stepNumber = 0;

            for (int i = 0; i < routingDetails.Count; i++)
            {
                RoutingDetail routingDetail = routingDetails[i];

                stepNumber = stepNumber + 1;

                WorkOrder workOrder = objectSpace.CreateObject<WorkOrder>();
                workOrder.Code = string.Format("{0}-{1:000}", productionOrder.Code, stepNumber);
                workOrder.SequenceNumber = routingDetail.SequenceNumber;
                workOrder.ProductionOrder = productionOrder;
                workOrder.Operation = routingDetail.Operation;
                workOrder.AssignedWorkStation = routingDetail.WorkStation;
                workOrder.Status = WorkOrderStatus.Planned;
            }

            productionOrder.Status = ProductionOrderStatus.InProgress;

            objectSpace.CommitChanges();

            Application.ShowViewStrategy.ShowMessage(string.Format("{0} Work Order(s) created successfully. Production Order approved.", routingDetails.Count), InformationType.Success);
        }

    }
}
