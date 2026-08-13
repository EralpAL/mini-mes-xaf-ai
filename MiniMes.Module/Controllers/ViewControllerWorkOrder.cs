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
            //View.AllowNew["ManualWorkOrderCreationDisabled"] = false;
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }

        private void WorkOrder_Start_CustomizePopupWindowParams(object sender,CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace popupObjectSpace =Application.CreateObjectSpace(typeof(NonPersistentObject));

            NonPersistentObject parameters =popupObjectSpace.CreateObject<NonPersistentObject>();

            DetailView detailView =
                Application.CreateDetailView( popupObjectSpace, parameters);

            detailView.ViewEditMode = ViewEditMode.Edit;

            e.View = detailView;
        }

        private void WorkOrder_Start_Execute(
     object sender,
     PopupWindowShowActionExecuteEventArgs e)
        {
            NonPersistentObject parameters = e.PopupWindowViewCurrentObject as NonPersistentObject;

            if (parameters == null)
            {
                return;
            }

            if (parameters.SelectedEmployee == null)
            {
                throw new UserFriendlyException("İş emrini başlatmak için çalışan seçmelisiniz.");
            }

            if (parameters.SelectedRole == null)
            {
                throw new UserFriendlyException("İş emrini başlatmak için görev seçmelisiniz.");
            }

            if (parameters.SelectedShift == null)
            {
                throw new UserFriendlyException("İş emrini başlatmak için vardiya seçmelisiniz.");
            }

            Employee selectedEmployee = ObjectSpace.GetObject(parameters.SelectedEmployee);

            JobRole selectedRole = ObjectSpace.GetObject(parameters.SelectedRole);

            Shift selectedShift = ObjectSpace.GetObject(parameters.SelectedShift);

            if (selectedEmployee == null || selectedRole == null || selectedShift == null)
            {
                throw new UserFriendlyException( "Seçilen çalışan, görev veya vardiya bulunamadı.");
            }

            // Önce seçilen bütün iş emirlerini kontrol eder.
            for (int i = 0; i < e.SelectedObjects.Count; i++)
            {
                WorkOrder workOrder =
                    e.SelectedObjects[i] as WorkOrder;

                if (workOrder == null || workOrder.Status != WorkOrderStatus.Planned)
                {
                    throw new UserFriendlyException("Yalnızca planlanan iş emirleri başlatılabilir.");
                }
            }

            // Seçilen bilgileri iş emirlerine aktarır ve emirleri başlatır.
            for (int i = 0; i < e.SelectedObjects.Count; i++)
            {
                WorkOrder workOrder =
                    e.SelectedObjects[i] as WorkOrder;

                workOrder.AssignedEmployee = selectedEmployee;
                workOrder.AssignedRole = selectedRole;
                workOrder.AssignedShift = selectedShift;
                workOrder.Status = WorkOrderStatus.InProgress;
            }

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }

        private void WorkOrder_Stop_Execute(
            object sender,
            SimpleActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;

            if (workOrder == null)
            {
                return;
            }

            if (workOrder.Status != WorkOrderStatus.InProgress)
            {
                throw new UserFriendlyException("Yalnızca devam eden iş emirleri durdurulabilir.");
            }

            workOrder.Status = WorkOrderStatus.Stopped;

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }
        private void WorkOrder_Continue_Execute( object sender,SimpleActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;

            if (workOrder == null)
                return;

            workOrder.Status = WorkOrderStatus.InProgress;

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }

        private void WorkOrder_Finish_Execute(
            object sender,SimpleActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;

            if (workOrder == null)
                return;

            workOrder.Status = WorkOrderStatus.Completed;

            // Üretim emrinin tüm iş emirleri tamamlandıysa üretim emrini de tamamla.
            ProductionOrder productionOrder = workOrder.ProductionOrder;

            if (productionOrder != null)
            {
                bool hasWorkOrders = false;
                bool allWorkOrdersCompleted = true;

                for (int i = 0; i < productionOrder.WorkOrders.Count; i++)
                {
                    WorkOrder relatedWorkOrder =productionOrder.WorkOrders[i];

                    if (relatedWorkOrder.IsDeleted)
                    {
                        continue;
                    }

                    hasWorkOrders = true;

                    if (relatedWorkOrder.Status !=WorkOrderStatus.Completed)
                    {
                        allWorkOrdersCompleted = false;
                        break;
                    }
                }

                if (hasWorkOrders && allWorkOrdersCompleted)
                {
                    productionOrder.Status =ProductionOrderStatus.Completed;
                }
            }

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }
    }


}