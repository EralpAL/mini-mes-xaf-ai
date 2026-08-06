using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
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
    [DefaultClassOptions]
    [NavigationItem("Production Operations")]
    [RuleCriteria("ProductionEntry_QuantityEntered", DefaultContexts.Save, "RealizedAmount + ScrapAmount > 0", CustomMessageTemplate = "Enter a realized quantity, a scrap quantity, or both.")]
    [RuleCriteria("ProductionEntry_ScrapReasonRequired", DefaultContexts.Save, "Not (ScrapAmount > 0 And IsNullOrEmpty(ScrapReason))", CustomMessageTemplate = "Specify a scrap reason when a scrap quantity is reported.")]
    public class ProductionEntry : BaseObject
    { 
        public ProductionEntry(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
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

        private decimal realizedAmount;

        [RuleRange(0.0, double.MaxValue)]
        public decimal RealizedAmount
        {
            get
            {
                return realizedAmount;
            }
            set
            {
                if (SetPropertyValue(nameof(RealizedAmount), ref realizedAmount, value))
                {
                    UpdateWorkOrderTotals();
                }
            }
        }

        private decimal scrapAmount;

        [RuleRange(0.0, double.MaxValue)]
        public decimal ScrapAmount
        {
            get
            {
                return scrapAmount;
            }
            set
            {
                if (SetPropertyValue(nameof(ScrapAmount), ref scrapAmount, value))
                {
                    UpdateWorkOrderTotals();
                }
            }
        }

        private string scrapReason;

        public string ScrapReason
        {
            get
            {
                return scrapReason;
            }
            set
            {
                SetPropertyValue(nameof(ScrapReason), ref scrapReason, value);
            }
        }

        private DateTime startTime;

        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                SetPropertyValue(nameof(StartTime), ref startTime, value);
            }
        }

        private DateTime endTime;

        [RuleValueComparison("ProductionEntry_EndTimeNotBeforeStartTime", DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, "StartTime", ParametersMode.Expression, CustomMessageTemplate = "End time must not be earlier than start time.")]
        public DateTime EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                SetPropertyValue(nameof(EndTime), ref endTime, value);
            }
        }

        // The parent totals are refreshed as soon as a value changes rather than only in
        // OnSaving, so the parent is already marked dirty when the commit starts and is written
        // in the same transaction. OnSaving stays as a safety net for programmatic changes; it
        // recomputes the same value and therefore does not mark anything dirty again.
        // See WorkOrder.RecalculateTotals().
        private void UpdateWorkOrderTotals()
        {
            if (IsLoading || IsSaving || IsDeleted || WorkOrder == null)
            {
                return;
            }
            WorkOrder.RecalculateTotals();
        }

        protected override void OnSaving()
        {
            base.OnSaving();
            if (WorkOrder != null)
            {
                WorkOrder.RecalculateTotals();
            }
        }

        protected override void OnDeleting()
        {
            WorkOrder affectedWorkOrder = WorkOrder;
            base.OnDeleting();
            if (affectedWorkOrder != null)
            {
                affectedWorkOrder.RecalculateTotals(this);
            }
        }
    }
}
