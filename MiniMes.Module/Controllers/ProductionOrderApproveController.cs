using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using MiniMes.Module.BusinessObjects;
using MiniMes.Module.Enums;
using System.Collections.Generic;
using System.Linq;

namespace MiniMes.Module.Controllers
{
    // Drives the ProductionOrder life cycle: Approve generates WorkOrders from the target
    // StockCard's RoutingDetail steps, Complete and Cancel close the order and its open steps.
    // See AGENTS.md section 8 (Controller Conventions).
    public class ProductionOrderApproveController : ObjectViewController<DetailView, ProductionOrder>
    {
        private readonly SimpleAction approveAction;
        private readonly SimpleAction completeAction;
        private readonly SimpleAction cancelAction;

        public ProductionOrderApproveController()
        {
            approveAction = new SimpleAction(this, "Approve", PredefinedCategory.Edit)
            {
                Caption = "Approve",
                ToolTip = "Generate the Work Orders of the target product's routing and start the order.",
                ConfirmationMessage = "Are you sure you want to approve this Production Order and generate its Work Orders?"
            };
            approveAction.Execute += ApproveAction_Execute;

            completeAction = new SimpleAction(this, "CompleteProductionOrder", PredefinedCategory.Edit)
            {
                Caption = "Complete",
                ToolTip = "Close the order and all of its open Work Orders.",
                ConfirmationMessage = "Are you sure you want to complete this Production Order?"
            };
            completeAction.Execute += CompleteAction_Execute;

            cancelAction = new SimpleAction(this, "CancelProductionOrder", PredefinedCategory.Edit)
            {
                Caption = "Cancel Order",
                ToolTip = "Cancel the order and all of its Work Orders that are not completed yet.",
                ConfirmationMessage = "Are you sure you want to cancel this Production Order?"
            };
            cancelAction.Execute += CancelAction_Execute;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            UpdateActionState();
            ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
            ObjectSpace.Reloaded += ObjectSpace_Reloaded;
            // A brand new order only becomes approvable once it has been saved.
            ObjectSpace.Committed += ObjectSpace_Committed;
        }

        protected override void OnDeactivated()
        {
            ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
            ObjectSpace.Reloaded -= ObjectSpace_Reloaded;
            ObjectSpace.Committed -= ObjectSpace_Committed;
            base.OnDeactivated();
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            UpdateActionState();
        }

        private void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e)
        {
            UpdateActionState();
        }

        private void ObjectSpace_Reloaded(object sender, System.EventArgs e)
        {
            UpdateActionState();
        }

        private void ObjectSpace_Committed(object sender, System.EventArgs e)
        {
            UpdateActionState();
        }

        private void UpdateActionState()
        {
            ProductionOrder productionOrder = ViewCurrentObject;
            bool isPersisted = productionOrder != null && !ObjectSpace.IsNewObject(productionOrder);

            approveAction.Active["ProductionOrder_CanApprove"] =
                isPersisted && productionOrder.Status == ProductionOrderStatus.Planned;
            completeAction.Active["ProductionOrder_CanComplete"] =
                isPersisted && productionOrder.Status == ProductionOrderStatus.InProgress;
            cancelAction.Active["ProductionOrder_CanCancel"] =
                isPersisted &&
                (productionOrder.Status == ProductionOrderStatus.Planned ||
                 productionOrder.Status == ProductionOrderStatus.InProgress);
        }

        private ProductionOrder GetPersistedCurrentObject()
        {
            ProductionOrder productionOrder = ViewCurrentObject;
            if (productionOrder == null)
            {
                throw new UserFriendlyException("There is no Production Order to process.");
            }
            if (ObjectSpace.IsNewObject(productionOrder))
            {
                throw new UserFriendlyException("Save the Production Order before running this action.");
            }
            return productionOrder;
        }

        private void ApproveAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ProductionOrder productionOrder = GetPersistedCurrentObject();

            if (productionOrder.Status != ProductionOrderStatus.Planned)
            {
                throw new UserFriendlyException("Only Production Orders with 'Planned' status can be approved.");
            }

            if (productionOrder.TargetStockCard == null)
            {
                throw new UserFriendlyException("The Production Order does not have a Target Stock Card assigned.");
            }

            if (productionOrder.PlannedQuantity <= 0)
            {
                throw new UserFriendlyException("The planned quantity must be greater than zero before the order can be approved.");
            }

            if (productionOrder.WorkOrders.Count > 0)
            {
                throw new UserFriendlyException("Work Orders have already been generated for this Production Order.");
            }

            List<RoutingDetail> routingDetails = new List<RoutingDetail>();

            for (int i = 0; i < productionOrder.TargetStockCard.Routings.Count; i++)
            {
                RoutingDetail routingDetail = productionOrder.TargetStockCard.Routings[i];
                routingDetails.Add(routingDetail);
            }

            // The routing steps must run in ascending SequenceNumber order.
            for (int i = 0; i < routingDetails.Count - 1; i++)
            {
                for (int j = 0; j < routingDetails.Count - 1 - i; j++)
                {
                    if (routingDetails[j].SequenceNumber > routingDetails[j + 1].SequenceNumber)
                    {
                        RoutingDetail temporaryDetail = routingDetails[j];
                        routingDetails[j] = routingDetails[j + 1];
                        routingDetails[j + 1] = temporaryDetail;
                    }
                }
            }

            if (routingDetails.Count == 0)
            {
                throw new UserFriendlyException("No routing details were found for the target Stock Card. No Work Orders were created.");
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
            UpdateActionState();

            Application.ShowViewStrategy.ShowMessage(
                string.Format("{0} Work Order(s) created successfully. Production Order approved.", routingDetails.Count),
                InformationType.Success);
        }

        private void CompleteAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ProductionOrder productionOrder = GetPersistedCurrentObject();

            if (productionOrder.Status != ProductionOrderStatus.InProgress)
            {
                throw new UserFriendlyException("Only Production Orders with 'In Progress' status can be completed.");
            }

            if (productionOrder.ProducedQuantity <= 0)
            {
                throw new UserFriendlyException("The order cannot be completed before any production has been reported.");
            }

            for (int i = 0; i < productionOrder.WorkOrders.Count; i++)
            {
                WorkOrder workOrder = productionOrder.WorkOrders[i];

                if (workOrder.Status == WorkOrderStatus.Planned || workOrder.Status == WorkOrderStatus.InProgress)
                {
                    workOrder.Status = WorkOrderStatus.Completed;
                }
            }

            productionOrder.Status = ProductionOrderStatus.Completed;

           ObjectSpace.CommitChanges();
            UpdateActionState();

            Application.ShowViewStrategy.ShowMessage("Production Order completed.", InformationType.Success);
        }

        private void CancelAction_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            ProductionOrder productionOrder = GetPersistedCurrentObject();

            if (productionOrder.Status == ProductionOrderStatus.Completed ||
                productionOrder.Status == ProductionOrderStatus.Canceled)
            {
                throw new UserFriendlyException("Completed or already canceled Production Orders cannot be canceled.");
            }

            for (int i = 0; i < productionOrder.WorkOrders.Count; i++)
            {
                WorkOrder workOrder = productionOrder.WorkOrders[i];

                if (workOrder.Status != WorkOrderStatus.Completed)
                {
                    workOrder.Status = WorkOrderStatus.Canceled;
                }
            }

            productionOrder.Status = ProductionOrderStatus.Canceled;

            View.ObjectSpace.CommitChanges();
            UpdateActionState();

            Application.ShowViewStrategy.ShowMessage("Production Order canceled.", InformationType.Success);
        }
    }
}
