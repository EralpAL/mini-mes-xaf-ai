namespace MiniMes.Module.Controllers
{
    partial class ViewControllerWorkOrder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// true if managed resources should be disposed; otherwise, false.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.WorkOrder_Start = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            this.WorkOrder_Stop = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.WorkOrder_Continue = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.WorkOrder_Finish = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            this.WorkOrder_ProductionEntry = new DevExpress.ExpressApp.Actions.PopupWindowShowAction(this.components);
            //
            // WorkOrder_Start
            //
            this.WorkOrder_Start.Caption = "Başlat";
            this.WorkOrder_Start.Category = "View";
            this.WorkOrder_Start.ConfirmationMessage = "Bu iş emrini başlatmak istediğinize emin misiniz?";
            this.WorkOrder_Start.Id = "WorkOrder.Start";
            this.WorkOrder_Start.TargetObjectType = typeof(MiniMes.Module.BusinessObjects.WorkOrder);
            this.WorkOrder_Start.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireMultipleObjects;
            this.WorkOrder_Start.TargetObjectsCriteria = "Status = 'Planned'";
            this.WorkOrder_Start.ToolTip = "Planlanan iş emrini başlatır.";
            this.WorkOrder_Start.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.WorkOrder_Start_CustomizePopupWindowParams);
            this.WorkOrder_Start.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.WorkOrder_Start_Execute);
            //
            // WorkOrder_Stop
            //
            this.WorkOrder_Stop.Caption = "Durdur";
            this.WorkOrder_Stop.Category = "View";
            this.WorkOrder_Stop.ConfirmationMessage = "Bu iş emrini durdurmak istediğinize emin misiniz?";
            this.WorkOrder_Stop.Id = "WorkOrder.Stop";
            this.WorkOrder_Stop.TargetObjectType = typeof(MiniMes.Module.BusinessObjects.WorkOrder);
            this.WorkOrder_Stop.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.WorkOrder_Stop.TargetObjectsCriteria = "Status = 'InProgress'";
            this.WorkOrder_Stop.ToolTip = "Devam eden iş emrini durdurur.";
            this.WorkOrder_Stop.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.WorkOrder_Stop_Execute);
            //
            // WorkOrder_Continue
            //
            this.WorkOrder_Continue.Caption = "Devam Et";
            this.WorkOrder_Continue.Category = "View";
            this.WorkOrder_Continue.ConfirmationMessage = "Bu iş emrine devam etmek istediğinize emin misiniz?";
            this.WorkOrder_Continue.Id = "WorkOrder.Resume";
            this.WorkOrder_Continue.TargetObjectType = typeof(MiniMes.Module.BusinessObjects.WorkOrder);
            this.WorkOrder_Continue.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.WorkOrder_Continue.TargetObjectsCriteria = "Status = 'Stopped'";
            this.WorkOrder_Continue.ToolTip = "Durdurulan iş emrine devam eder.";
            this.WorkOrder_Continue.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.WorkOrder_Continue_Execute);
            //
            // WorkOrder_Finish
            //
            this.WorkOrder_Finish.Caption = "Bitir";
            this.WorkOrder_Finish.Category = "View";
            this.WorkOrder_Finish.ConfirmationMessage = "Bu iş emrini tamamlamak istediğinize emin misiniz?";
            this.WorkOrder_Finish.Id = "WorkOrder.Complete";
            this.WorkOrder_Finish.TargetObjectType = typeof(MiniMes.Module.BusinessObjects.WorkOrder);
            this.WorkOrder_Finish.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.WorkOrder_Finish.TargetObjectsCriteria = "Status = 'InProgress'";
            this.WorkOrder_Finish.ToolTip = "Devam eden iş emrini tamamlar.";
            this.WorkOrder_Finish.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.WorkOrder_Finish_Execute);
            //
            // WorkOrder_ProductionEntry
            //
            this.WorkOrder_ProductionEntry.Caption = "Üretim Girişi";
            this.WorkOrder_ProductionEntry.Category = "View";
            this.WorkOrder_ProductionEntry.Id = "WorkOrder.ProductionEntry";
            this.WorkOrder_ProductionEntry.TargetObjectType = typeof(MiniMes.Module.BusinessObjects.WorkOrder);
            this.WorkOrder_ProductionEntry.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.WorkOrder_ProductionEntry.TargetObjectsCriteria = "Status = 'InProgress'";
            this.WorkOrder_ProductionEntry.ToolTip = "Devam eden iş emrine üretim miktarı girer.";
            this.WorkOrder_ProductionEntry.CustomizePopupWindowParams += new DevExpress.ExpressApp.Actions.CustomizePopupWindowParamsEventHandler(this.WorkOrder_ProductionEntry_CustomizePopupWindowParams);
            this.WorkOrder_ProductionEntry.Execute += new DevExpress.ExpressApp.Actions.PopupWindowShowActionExecuteEventHandler(this.WorkOrder_ProductionEntry_Execute);
            //
            // ViewControllerWorkOrder
            //
            this.Actions.Add(this.WorkOrder_Start);
            this.Actions.Add(this.WorkOrder_Stop);
            this.Actions.Add(this.WorkOrder_Continue);
            this.Actions.Add(this.WorkOrder_Finish);
            this.Actions.Add(this.WorkOrder_ProductionEntry);
        }

        #endregion

        private DevExpress.ExpressApp.Actions.PopupWindowShowAction WorkOrder_Start;
        private DevExpress.ExpressApp.Actions.SimpleAction WorkOrder_Stop;
        private DevExpress.ExpressApp.Actions.SimpleAction WorkOrder_Continue;
        private DevExpress.ExpressApp.Actions.SimpleAction WorkOrder_Finish;
        private DevExpress.ExpressApp.Actions.PopupWindowShowAction WorkOrder_ProductionEntry;
    }
}