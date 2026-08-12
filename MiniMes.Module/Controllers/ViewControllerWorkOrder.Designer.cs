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

            this.WorkOrder_Start = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);

            this.WorkOrder_Stop =new DevExpress.ExpressApp.Actions.SimpleAction(this.components);

            this.WorkOrder_Resume = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);

            this.WorkOrder_Complete =new DevExpress.ExpressApp.Actions.SimpleAction(this.components);

            //
            // WorkOrder_Start
            //
            this.WorkOrder_Start.Caption = "Başlat";
            this.WorkOrder_Start.Category = "View";
            this.WorkOrder_Start.ConfirmationMessage = "Bu iş emrini başlatmak istediğinize emin misiniz?";
            this.WorkOrder_Start.Id = "WorkOrder.Start";
            this.WorkOrder_Start.TargetObjectType = typeof(MiniMes.Module.BusinessObjects.WorkOrder);
            this.WorkOrder_Start.SelectionDependencyType =DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.WorkOrder_Start.TargetObjectsCriteria = "Status = ##Enum#MiniMes.Module.Enums.WorkOrderStatus,Planned#";
            this.WorkOrder_Start.ToolTip = "Planlanan iş emrini başlatır.";
            this.WorkOrder_Start.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.WorkOrder_Start_Execute);

            //
            // WorkOrder_Stop
            //
            this.WorkOrder_Stop.Caption = "Durdur";
            this.WorkOrder_Stop.Category = "View";
            this.WorkOrder_Stop.ConfirmationMessage ="Bu iş emrini durdurmak istediğinize emin misiniz?";
            this.WorkOrder_Stop.Id = "WorkOrder.Stop";
            this.WorkOrder_Stop.TargetObjectType =typeof(MiniMes.Module.BusinessObjects.WorkOrder);
            this.WorkOrder_Stop.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.WorkOrder_Stop.TargetObjectsCriteria ="Status = ##Enum#MiniMes.Module.Enums.WorkOrderStatus,InProgress#";
            this.WorkOrder_Stop.ToolTip ="Devam eden iş emrini durdurur.";
            this.WorkOrder_Stop.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler( this.WorkOrder_Stop_Execute);

            //
            // WorkOrder_Resume
            //
            this.WorkOrder_Resume.Caption = "Devam Et";
            this.WorkOrder_Resume.Category = "View";
            this.WorkOrder_Resume.ConfirmationMessage = "Bu iş emrine devam etmek istediğinize emin misiniz?";
            this.WorkOrder_Resume.Id = "WorkOrder.Resume";
            this.WorkOrder_Resume.TargetObjectType =typeof(MiniMes.Module.BusinessObjects.WorkOrder);
            this.WorkOrder_Resume.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.WorkOrder_Resume.TargetObjectsCriteria ="Status = ##Enum#MiniMes.Module.Enums.WorkOrderStatus,Stopped#";
            this.WorkOrder_Resume.ToolTip ="Durdurulan iş emrine devam eder.";
            this.WorkOrder_Resume.Execute +=new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.WorkOrder_Resume_Execute);

            //
            // WorkOrder_Complete
            //
            this.WorkOrder_Complete.Caption = "Bitir";
            this.WorkOrder_Complete.Category = "View";
            this.WorkOrder_Complete.ConfirmationMessage ="Bu iş emrini tamamlamak istediğinize emin misiniz?";
            this.WorkOrder_Complete.Id = "WorkOrder.Complete";
            this.WorkOrder_Complete.TargetObjectType = typeof(MiniMes.Module.BusinessObjects.WorkOrder);
            this.WorkOrder_Complete.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.RequireSingleObject;
            this.WorkOrder_Complete.TargetObjectsCriteria = "Status = ##Enum#MiniMes.Module.Enums.WorkOrderStatus,InProgress#";
            this.WorkOrder_Complete.ToolTip ="Devam eden iş emrini tamamlar.";
            this.WorkOrder_Complete.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.WorkOrder_Complete_Execute);

            //
            // ViewControllerWorkOrder
            //
            this.Actions.Add(this.WorkOrder_Start);
            this.Actions.Add(this.WorkOrder_Stop);
            this.Actions.Add(this.WorkOrder_Resume);
            this.Actions.Add(this.WorkOrder_Complete);
        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction WorkOrder_Start;
        private DevExpress.ExpressApp.Actions.SimpleAction WorkOrder_Stop;
        private DevExpress.ExpressApp.Actions.SimpleAction WorkOrder_Resume;
        private DevExpress.ExpressApp.Actions.SimpleAction WorkOrder_Complete;
    }
}