using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MiniMes.Module.BusinessObjects
{

    [NavigationItem(false)]
    [Appearance("ProductionEntry_HideNew",AppearanceItemType.Action,"1=1",TargetItems = "New",Visibility = ViewItemVisibility.Hide)]
    [XafDisplayName("Üretim Girişi")]

    public class ProductionEntry : BaseObject
    {
        public ProductionEntry(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private WorkOrder workOrder;

        [RuleRequiredField]
        [Association("WorkOrder-ProductionEntries")]
        public WorkOrder WorkOrder
        {
            get
            {
                return workOrder;
            }
            set
            {

                WorkOrder previousWorkOrder = workOrder;
                if (!SetPropertyValue(nameof(WorkOrder), ref workOrder, value) || IsLoading || IsSaving)
                {
                    return;
                }
                if (workOrder != null && WorkStation == null)
                {
                    WorkStation = workOrder.AssignedWorkStation;
                }
                if (previousWorkOrder != null)
                {
                    previousWorkOrder.RecalculateTotals(this);
                }
                UpdateWorkOrderTotals();
            }
        }

        private Employee operatorEmployee;

        [RuleRequiredField]
        [Association("Employee-ProductionEntries")]
        public Employee Operator
        {
            get
            {
                return operatorEmployee;
            }
            set
            {
                SetPropertyValue(nameof(Operator), ref operatorEmployee, value);
            }
        }

        private WorkStation workStation;

        [RuleRequiredField]
        public WorkStation WorkStation
        {
            get
            {
                return workStation;
            }
            set
            {
                SetPropertyValue(nameof(WorkStation), ref workStation, value);
            }
        }

        private int realizedAmount;

        [RuleRange(0, int.MaxValue)]
        public int RealizedAmount
        {
            get
            {
                return realizedAmount;
            }
            set
            {
                if (SetPropertyValue(nameof(RealizedAmount), ref realizedAmount, value))
                    UpdateWorkOrderTotals();
            }
        }

        private void UpdateWorkOrderTotals()
        {
            if (IsLoading || IsSaving || IsDeleted || WorkOrder == null)
                return;

            WorkOrder.RecalculateTotals();
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            if (WorkOrder != null)
                WorkOrder.RecalculateTotals();
        }

        protected override void OnDeleting()
        {
            base.OnDeleting();

            if (WorkOrder != null)
                WorkOrder.RecalculateTotals(this);
        }
    }
}
