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
        }

        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
        }

        private void WorkOrder_Start_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            IObjectSpace popupObjectSpace = Application.CreateObjectSpace(typeof(NonPersistentObject));

            NonPersistentObject parameters = popupObjectSpace.CreateObject<NonPersistentObject>();

            DetailView detailView = Application.CreateDetailView(popupObjectSpace, parameters);

            detailView.ViewEditMode = ViewEditMode.Edit;

            e.View = detailView;
        }

        private void WorkOrder_Start_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            NonPersistentObject parameters = e.PopupWindowViewCurrentObject as NonPersistentObject;

            if (parameters == null)
            {
                return;
            }

            Employee selectedEmployee = ObjectSpace.GetObject(parameters.SelectedEmployee);
            JobRole selectedRole = ObjectSpace.GetObject(parameters.SelectedRole);
            Shift selectedShift = ObjectSpace.GetObject(parameters.SelectedShift);

            if (selectedEmployee == null || selectedRole == null || selectedShift == null)
            {
                return;
            }

            for (int i = 0; i < e.SelectedObjects.Count; i++)
            {
                WorkOrder workOrder = e.SelectedObjects[i] as WorkOrder;

                if (workOrder == null)
                {
                    continue;
                }

                workOrder.AssignedEmployee = selectedEmployee;
                workOrder.AssignedRole = selectedRole;
                workOrder.AssignedShift = selectedShift;
                workOrder.Status = WorkOrderStatus.InProgress;
            }

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }

        private void WorkOrder_Stop_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            WorkOrder workOrder = View.CurrentObject as WorkOrder;

            IObjectSpace popupObjectSpace = Application.CreateObjectSpace(typeof(DowntimeStartParameters));
            DowntimeStartParameters parameters = popupObjectSpace.CreateObject<DowntimeStartParameters>();
            parameters.HasWorkStation = workOrder != null && workOrder.AssignedWorkStation != null;
            DetailView detailView = Application.CreateDetailView(popupObjectSpace, parameters);
            detailView.Caption = "Duruş Başlat";
            detailView.ViewEditMode = ViewEditMode.Edit;
            e.View = detailView;
        }

        private void WorkOrder_Stop_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;
            DowntimeStartParameters parameters = e.PopupWindowViewCurrentObject as DowntimeStartParameters;

            if (workOrder == null || parameters == null)
            {
                return;
            }

            StopCause selectedStopCause = ObjectSpace.GetObject(parameters.SelectedStopCause);

            if (selectedStopCause == null)
            {
                return;
            }

            DowntimeLog downtimeLog = ObjectSpace.CreateObject<DowntimeLog>();
            downtimeLog.WorkOrder = workOrder;
            downtimeLog.WorkStation = workOrder.AssignedWorkStation;
            downtimeLog.Operator = workOrder.AssignedEmployee;
            downtimeLog.StopCause = selectedStopCause;
            downtimeLog.StartTime = DateTime.Now;

            workOrder.Status = WorkOrderStatus.Stopped;

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();

            Application.ShowViewStrategy.ShowMessage("Duruş başlatıldı.", InformationType.Success);
        }

        private void WorkOrder_Continue_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;

            if (workOrder == null)
            {
                return;
            }

            workOrder.CloseOpenDowntime();
            workOrder.Status = WorkOrderStatus.InProgress;

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();

            Application.ShowViewStrategy.ShowMessage("Duruş bitirildi.", InformationType.Success);
        }

        private void WorkOrder_Finish_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;

            if (workOrder == null)
            {
                return;
            }

            workOrder.Status = WorkOrderStatus.Completed;

            // Üretim emrinin tüm iş emirleri tamamlandıysa üretim emrini de tamamlar.
            ProductionOrder productionOrder = workOrder.ProductionOrder;

            if (productionOrder != null)
            {
                bool hasWorkOrders = false;
                bool allWorkOrdersCompleted = true;

                for (int i = 0; i < productionOrder.WorkOrders.Count; i++)
                {
                    WorkOrder relatedWorkOrder = productionOrder.WorkOrders[i];

                    if (relatedWorkOrder.IsDeleted)
                    {
                        continue;
                    }

                    hasWorkOrders = true;

                    if (relatedWorkOrder.Status != WorkOrderStatus.Completed)
                    {
                        allWorkOrdersCompleted = false;
                        break;
                    }
                }

                if (hasWorkOrders && allWorkOrdersCompleted)
                {
                    productionOrder.Status = ProductionOrderStatus.Completed;
                }
            }

            ObjectSpace.CommitChanges();
            View.ObjectSpace.Refresh();
        }

        private void WorkOrder_ProductionEntry_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            WorkOrder workOrder = View.CurrentObject as WorkOrder;

            IObjectSpace popupObjectSpace = Application.CreateObjectSpace(typeof(ProductionEntryParameters));
            ProductionEntryParameters parameters = popupObjectSpace.CreateObject<ProductionEntryParameters>();
            parameters.HasWorkStation = workOrder != null && workOrder.AssignedWorkStation != null;
            DetailView detailView = Application.CreateDetailView(popupObjectSpace, parameters);
            detailView.ViewEditMode = ViewEditMode.Edit;
            e.View = detailView;
        }

        private void WorkOrder_ProductionEntry_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;
            ProductionEntryParameters parameters = e.PopupWindowViewCurrentObject as ProductionEntryParameters;

            if (workOrder == null || parameters == null)
            {
                return;
            }

            ProductionEntry productionEntry = ObjectSpace.CreateObject<ProductionEntry>();

            productionEntry.WorkOrder = workOrder;
            productionEntry.Operator = workOrder.AssignedEmployee;
            productionEntry.WorkStation = workOrder.AssignedWorkStation;
            productionEntry.RealizedAmount = parameters.RealizedAmount;

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            Application.ShowViewStrategy.ShowMessage("Üretim miktarı başarıyla kaydedildi.",InformationType.Success);
        }

        private void WorkOrder_ScrapEntry_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            WorkOrder workOrder = View.CurrentObject as WorkOrder;

            IObjectSpace popupObjectSpace = Application.CreateObjectSpace(typeof(ProductionEntryParameters));
            ProductionEntryParameters parameters = popupObjectSpace.CreateObject<ProductionEntryParameters>();
            parameters.IsScrapEntry = true;
            parameters.HasWorkStation = workOrder != null && workOrder.AssignedWorkStation != null;
            parameters.AvailableQuantity = workOrder != null ? workOrder.ProducedQuantity : 0;
            DetailView detailView = Application.CreateDetailView(popupObjectSpace, parameters);
            detailView.Caption = "Fire Girişi";
            detailView.ViewEditMode = ViewEditMode.Edit;
            e.View = detailView;
        }

        private void WorkOrder_ScrapEntry_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            WorkOrder workOrder = e.CurrentObject as WorkOrder;
            ProductionEntryParameters parameters = e.PopupWindowViewCurrentObject as ProductionEntryParameters;

            if (workOrder == null || parameters == null)
            {
                return;
            }

            ProductionEntry scrapEntry = ObjectSpace.CreateObject<ProductionEntry>();

            scrapEntry.WorkOrder = workOrder;
            scrapEntry.Operator = workOrder.AssignedEmployee;
            scrapEntry.WorkStation = workOrder.AssignedWorkStation;
            scrapEntry.ScrapAmount = parameters.ScrapAmount;

            ObjectSpace.CommitChanges();
            ObjectSpace.Refresh();

            Application.ShowViewStrategy.ShowMessage("Fire miktarı başarıyla kaydedildi.", InformationType.Success);
        }
    }
}