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
    [NavigationItem("Personnel and Shifts")]
    [DefaultProperty(nameof(FullName))]
    public class Employee : BaseObject { 
        public Employee(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            
        }
        private string registrationNumber;
        [RuleRequiredField]
        [RuleUniqueValue]
        public string RegistrationNumber
        {
            get { return registrationNumber; }
            set { SetPropertyValue(nameof(RegistrationNumber), ref registrationNumber, value); }
        }

        private string fullName;
        [RuleRequiredField]
        public String FullName
        {
            get { return fullName; }
            set { SetPropertyValue(nameof(FullName), ref fullName, value); }
        }


        private EnumEmployeeRole role;
        public EnumEmployeeRole Role
        {
            get { return role; }
            set { SetPropertyValue(nameof(Role), ref role, value); }
        }

        private Shift assignedShift;

        [Association("Shift-Employees")]
        public Shift AssignedShift
        {
            get
            {
                return assignedShift;
            }
            set
            {
                SetPropertyValue(nameof(AssignedShift), ref assignedShift, value);
            }
        }

        private WorkStation workStation;

        [Association("WorkStation-Employees")]
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

        [Association("Employee-ProductionEntries")]
        public XPCollection<ProductionEntry> ProductionEntries
        {
            get
            {
                return GetCollection<ProductionEntry>(nameof(ProductionEntries));
            }
        }

    }

    }
