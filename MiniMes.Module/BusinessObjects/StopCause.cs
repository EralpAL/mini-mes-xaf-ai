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
using MiniMes.Module.Services;

namespace MiniMes.Module.BusinessObjects {
    [DefaultClassOptions]
    [XafDisplayName("Duruş Nedeni Parametre Tablosu):")]
    [NavigationItem("Production Definitions")]
    [DefaultProperty(nameof(Name))]

    public class StopCause : BaseObject {

        private const string CodePrefix = "STP";

        public StopCause(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            Code = BusinessCodeGenerator.GenerateCode( Session,typeof(StopCause),CodePrefix);
        }

        protected override void OnSaving()
        {
            if (string.IsNullOrEmpty(Code))
            {
                Code = BusinessCodeGenerator.GenerateCode(Session,typeof(StopCause),CodePrefix);
            }

            base.OnSaving();
        }

        private string stopCauseCode;
        [RuleRequiredField]
        [Indexed(Unique = true)]
        [ModelDefault("AllowEdit", "False")]

        public string Code
        {
            get { return stopCauseCode; }
            set { SetPropertyValue(nameof(Code), ref stopCauseCode, value); }
        }

        private string stopCauseName;
        [RuleRequiredField]
        public String Name
        {
            get { return stopCauseName; }
            set { SetPropertyValue(nameof(Name), ref stopCauseName, value); }
        }

        private StopCategory category;

        public StopCategory Category
        {
            get
            {
                return category;
            }
            set
            {
                SetPropertyValue(nameof(Category), ref category, value);
            }
        }

        [Association("StopCause-DowntimeLogs")]
        public XPCollection<DowntimeLog> DowntimeLogs
        {
            get
            {
                return GetCollection<DowntimeLog>(nameof(DowntimeLogs));
            }
        }

    }
}