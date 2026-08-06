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
    [NavigationItem("Production Definitions")]
    [DefaultProperty(nameof(Name))]

    public class Equipment : BaseObject { 
        
        public Equipment(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            IsActive = true;
        }

        private WorkStation workStation;
        [Association("WorkStation-Equipments")]
        public WorkStation WorkStation
        {
            get { return workStation; }
            set { SetPropertyValue(nameof(WorkStation), ref workStation, value); }
        }

        private string equipmentCode;
        [RuleRequiredField]
        [Indexed(Unique = true)]
        public string Code
        {
            get { return equipmentCode; }
            set { SetPropertyValue(nameof(Code), ref equipmentCode, value); }
        }

        private string equipmentName;
        [RuleRequiredField]
        public string Name
        {
            get { return equipmentName; }
            set { SetPropertyValue(nameof(Name), ref equipmentName, value); }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set { SetPropertyValue(nameof(IsActive), ref isActive, value); }
        }

    }
}
