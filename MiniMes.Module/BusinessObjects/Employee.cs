using DevExpress.ExpressApp;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MiniMes.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Personnel and Shifts")]
    [DefaultProperty(nameof(FullName))]
    public class Employee : BaseObject
    {
        public Employee(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private string registrationNumber;

        [RuleRequiredField]
        [RuleUniqueValue]
        public string RegistrationNumber
        {
            get
            {
                return registrationNumber;
            }
            set
            {
                SetPropertyValue(nameof(RegistrationNumber),ref registrationNumber,value);
            }
        }

        private string fullName;

        [RuleRequiredField]
        public string FullName
        {
            get
            {
                return fullName;
            }
            set
            {
                SetPropertyValue( nameof(FullName),ref fullName,value);
            }
        }

        [Association("Employee-ProductionEntries")]
        public XPCollection<ProductionEntry> ProductionEntries
        {
            get
            {
                return GetCollection<ProductionEntry>(
                    nameof(ProductionEntries));
            }
        }
    }
}