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
    [XafDisplayName("Bakým Kaydý")]
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
            MaintenanceDate = DateTime.Now;
        }

        private DateTime maintenanceDate;

        public DateTime MaintenanceDate
        {
            get
            {
                return maintenanceDate;
            }
            set
            {
                SetPropertyValue(nameof(MaintenanceDate), ref maintenanceDate, value);
            }
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

        private Equipment equipment;

        [DataSourceProperty("WorkStation.Equipments")]
        public Equipment Equipment
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

        private Employee employee;

        [RuleRequiredField]
        public Employee Employee
        {
            get
            {
                return employee;
            }
            set
            {
                SetPropertyValue(nameof(Employee), ref employee, value);
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

        private double cost;

        [RuleRange(0.0, double.MaxValue)]
        public double Cost
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
