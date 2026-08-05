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

    public class Equipments : BaseObject { 
        
        public Equipments(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            
        }

        private WorkStation workStation;
        [Association("WorkStation-Equipments")]
        public WorkStation WorkStation
        {
            get { return workStation; }
            set { SetPropertyValue(nameof(WorkStation), ref workStation, value); }
        }

    }
}