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
  
    public class Operation : BaseObject { 
        public Operation(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
           
        }

        private string operationCode;
        [RuleRequiredField]
        [Indexed(Unique = true)]
        public string Code
        {
            get { return operationCode; }
            set { SetPropertyValue(nameof(Code), ref operationCode, value); }
        }

        private string operationName;
        [RuleRequiredField]
        public String Name
        {
            get { return operationName; }
            set { SetPropertyValue(nameof(Name), ref operationName, value); }
        }



    }
}