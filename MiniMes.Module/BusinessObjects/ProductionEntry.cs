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
                SetPropertyValue(nameof(WorkOrder), ref workOrder, value);
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
                SetPropertyValue(nameof(RealizedAmount), ref realizedAmount, value);
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
                SetPropertyValue(nameof(ScrapAmount), ref scrapAmount, value);
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

        // Aggregation is recalculated from the complete ProductionEntries collection on every
        // save (create or edit) and every delete, so totals stay correct and deterministic
        // without double-counting. See WorkOrder.RecalculateTotals().
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
            base.OnDeleting();
            if (WorkOrder != null)
            {
                WorkOrder.RecalculateTotals(this);
            }
        }
    }
}
