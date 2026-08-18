using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;

namespace MiniMes.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDisplayName("Duru? Kayd?")]
    [NavigationItem("Production Operations")]
    [RuleCriteria("DowntimeLog_EndTimeNotBeforeStartTime", DefaultContexts.Save, "EndTime is null OR EndTime >= StartTime", CustomMessageTemplate = "Biti? zaman? ba?lang?ç zaman?ndan önce olamaz.")]
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
                if (SetPropertyValue(nameof(StartTime), ref startTime, value))
                {
                    UpdateDuration();
                }
            }
        }

        private DateTime? endTime;

        public DateTime? EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                if (SetPropertyValue(nameof(EndTime), ref endTime, value))
                {
                    UpdateDuration();
                }
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

        [Association("WorkOrder-DowntimeLogs")]
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
        [ModelDefault("AllowEdit", "False")]
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

        private void UpdateDuration()
        {
            if (IsLoading)
            {
                return;
            }

            if (!EndTime.HasValue)
            {
                DurationMinutes = 0;
                return;
            }

            TimeSpan duration = EndTime.Value - StartTime;
            double totalMinutes = duration.TotalMinutes;

            if (totalMinutes < 0)
            {
                totalMinutes = 0;
            }

            DurationMinutes = totalMinutes;
        }
    }
}
