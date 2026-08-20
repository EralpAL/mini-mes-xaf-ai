namespace MiniMes.Module.Controllers
{
    partial class ViewControllerProductionOrder
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
            this.ProductionOrder_Approve =new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            //
            // ProductionOrder_Approve
            //
            this.ProductionOrder_Approve.Caption = "Approve";
            this.ProductionOrder_Approve.Category = "View";
            this.ProductionOrder_Approve.ConfirmationMessage ="Are you sure you want to approve this Production Order and generate its Work Orders?";
            this.ProductionOrder_Approve.Id = "ProductionOrder.Approve";
            this.ProductionOrder_Approve.ImageName = "Apply_16x16";
            this.ProductionOrder_Approve.TargetObjectType =typeof(MiniMes.Module.BusinessObjects.ProductionOrder);
            this.ProductionOrder_Approve.ToolTip ="Generate Work Orders from the routing and start the Production Order.";
            this.ProductionOrder_Approve.SelectionDependencyType = DevExpress.ExpressApp.Actions.SelectionDependencyType.Independent;
            this.ProductionOrder_Approve.Execute +=new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.ProductionOrder_Approve_Execute);
            //
            // ViewControllerProductionOrder
            //
            this.Actions.Add(this.ProductionOrder_Approve);
        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction ProductionOrder_Approve;
    }
}