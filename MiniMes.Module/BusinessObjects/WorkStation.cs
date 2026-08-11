using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using MiniMes.Module.Services;

namespace MiniMes.Module.BusinessObjects {
    [DefaultClassOptions]
    [NavigationItem("Production Definitions")]
    [DefaultProperty(nameof(Name))]

    public class WorkStation : BaseObject { 
        private const string CodePrefix = "WS";

        public WorkStation(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            IsActive = true;
            Code = BusinessCodeGenerator.GenerateCode(Session, typeof(WorkStation), CodePrefix);
        }

        protected override void OnSaving() {
            base.OnSaving();
            if (string.IsNullOrEmpty(Code)) {
                Code = BusinessCodeGenerator.GenerateCode(Session, typeof(WorkStation), CodePrefix);
            }
        }

        private string workStationCode;
        [RuleRequiredField]
        [Indexed(Unique = true)]
        [ModelDefault("AllowEdit", "False")]
        public string Code
        {
            get { return workStationCode; }
            set { SetPropertyValue(nameof(Code), ref workStationCode, value); }
        }
        private string workStationName;
        [RuleRequiredField]
        public String Name
        {
            get { return workStationName; }
            set { SetPropertyValue(nameof(Name), ref workStationName, value); }
        }

        private  double hourlyCost;
        [RuleRange(0.0, double.MaxValue)]
        public double HourlyCost
        {
            get { return hourlyCost; }
            set { SetPropertyValue(nameof(HourlyCost), ref hourlyCost, value); }
        }
        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { SetPropertyValue(nameof(IsActive), ref isActive, value); }
        }


        [Association("WorkStation-Equipments")]
        public XPCollection<Equipment> Equipments
        {
            get { return GetCollection<Equipment>(nameof(Equipments)); }
        }


        [Association("WorkStation-Employees")]
        public XPCollection<Employee> Employees
        {
            get
            {
                return GetCollection<Employee>(nameof(Employees));
            }
        }

        [Association("WorkStation-MaintenanceLogs")]
        public XPCollection<MaintenanceLog> MaintenanceLogs
        {
            get
            {
                return GetCollection<MaintenanceLog>(nameof(MaintenanceLogs));
            }
        }

        [Association("WorkStation-Downtimes")]
        public XPCollection<DowntimeLog> Downtimes
        {
            get
            {
                return GetCollection<DowntimeLog>(nameof(Downtimes));
            }
        }

    }
}