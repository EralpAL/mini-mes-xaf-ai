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
    [XafDisplayName("Duruþ Kaydý")]
    [NavigationItem("Production Operations")]
    [RuleCriteria("DowntimeLog_DurationGreaterThanZero", DefaultContexts.Save,"DurationMinutes > 0", CustomMessageTemplate = "Downtime duration must be greater than zero minutes.")]
    public class DowntimeLog : BaseObject
    { 
        public DowntimeLog(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            StartTime = DateTime.Now;
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

        private WorkStation workStation;

        [RuleRequiredField]
        [Association("WorkStation-Downtimes")]
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

        private WorkOrder workOrder;

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

        private StopCause stopCause;

        [RuleRequiredField]
        [Association("StopCause-DowntimeLogs")]
        public StopCause StopCause
        {
            get
            {
                return stopCause;
            }
            set
            {
                SetPropertyValue(nameof(StopCause), ref stopCause, value);
            }
        }

        private Employee operatorEmployee;

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

        private double durationMinutes;

        [RuleRange(0.0, double.MaxValue)]
        public double DurationMinutes
        {
            get
            {
                return durationMinutes;
            }
            set
            {
                SetPropertyValue(nameof(DurationMinutes), ref durationMinutes, value);
            }
        }

        private string description;

        [Size(SizeAttribute.Unlimited)]
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                SetPropertyValue(nameof(Description), ref description, value);
            }
        }
    }
}
