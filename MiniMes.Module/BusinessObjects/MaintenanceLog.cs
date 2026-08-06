using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using MiniMes.Module.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MiniMes.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Production Operations")]
    public class MaintenanceLog : BaseObject
    { 
        public MaintenanceLog(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private WorkStation workStation;

        [RuleRequiredField]
        [Association("WorkStation-MaintenanceLogs")]
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

        private Equipments equipment;

        public Equipments Equipment
        {
            get
            {
                return equipment;
            }
            set
            {
                SetPropertyValue(nameof(Equipment), ref equipment, value);
            }
        }

        private Employee technician;

        [RuleRequiredField]
        public Employee Technician
        {
            get
            {
                return technician;
            }
            set
            {
                SetPropertyValue(nameof(Technician), ref technician, value);
            }
        }

        private MaintenanceType maintenanceType;

        public MaintenanceType MaintenanceType
        {
            get
            {
                return maintenanceType;
            }
            set
            {
                SetPropertyValue(nameof(MaintenanceType), ref maintenanceType, value);
            }
        }

        private decimal cost;

        [RuleRange(0.0, double.MaxValue)]
        public decimal Cost
        {
            get
            {
                return cost;
            }
            set
            {
                SetPropertyValue(nameof(Cost), ref cost, value);
            }
        }
    }
}
