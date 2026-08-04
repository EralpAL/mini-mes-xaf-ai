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
using MiniMes.Module.Enums;

namespace MiniMes.Module.BusinessObjects {
    [DefaultClassOptions]
   
    public class Employee : BaseObject { 
        public Employee(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            
        }
        private string registrationNumber;
        [RuleRequiredField]
        
        public string RegistrationNumber
        {
            get { return registrationNumber; }
            set { SetPropertyValue(nameof(RegistrationNumber), ref registrationNumber, value); }
        }

        private string employeeName;
        [RuleRequiredField]
        public String Name
        {
            get { return employeeName; }
            set { SetPropertyValue(nameof(Name), ref employeeName, value); }
        }


        private EmployeeRole role;
        public EmployeeRole Role
        {
            get { return role; }
            set { SetPropertyValue(nameof(Role), ref role, value); }
        }


    }
}