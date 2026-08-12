using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using MiniMes.Module.BusinessObjects;
using MiniMes.Module.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiniMes.Module.Controllers
{
    public partial class ViewControllerWorkOrder : ViewController
    {
        public ViewControllerWorkOrder()
        {
            InitializeComponent();

            TargetObjectType = typeof(WorkOrder);
            TargetViewType = ViewType.ListView;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            View.AllowNew["ManualWorkOrderCreationDisabled"] = false;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }

        private void WorkOrder_Start_Execute( object sender,SimpleActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;

            if (workOrder == null)
                return;

            if (workOrder.Status != WorkOrderStatus.Planned)
            {
                throw new UserFriendlyException(
                    "Yalnızca planlanan iş emirleri başlatılabilir.");
            }

            workOrder.Status = WorkOrderStatus.InProgress;

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }

        private void WorkOrder_Stop_Execute(object sender,SimpleActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;

            if (workOrder == null)
                return;

            if (workOrder.Status != WorkOrderStatus.InProgress)
            {
                throw new UserFriendlyException(
                    "Yalnızca devam eden iş emirleri durdurulabilir.");
            }

            workOrder.Status = WorkOrderStatus.Stopped;

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }

        private void WorkOrder_Resume_Execute(object sender,SimpleActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;

            if (workOrder == null)
                return;

            if (workOrder.Status != WorkOrderStatus.Stopped)
            {
                throw new UserFriendlyException(
                    "Yalnızca durdurulmuş iş emirlerine devam edilebilir.");
            }

            workOrder.Status = WorkOrderStatus.InProgress;

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }

        private void WorkOrder_Complete_Execute(object sender,SimpleActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;

            if (workOrder == null)
                return;

            if (workOrder.Status != WorkOrderStatus.InProgress)
            {
                throw new UserFriendlyException(
                    "Yalnızca devam eden iş emirleri tamamlanabilir.");
            }

            workOrder.Status = WorkOrderStatus.Completed;

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }
    }
}