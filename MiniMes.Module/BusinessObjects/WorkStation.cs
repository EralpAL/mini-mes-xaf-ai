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

namespace MiniMes.Module.BusinessObjects {
    [DefaultClassOptions]
   
    public class WorkStation : BaseObject { 
        public WorkStation(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            
        }

        private string workStationCode;
        [RuleRequiredField]
        [Indexed(Unique = true)]
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

        private decimal hourlyCost;
        public decimal HourlyCost
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
        public XPCollection<Equipments> Equipments
        {
            get { return GetCollection<Equipments>(nameof(Equipments)); }
        }


    }
}