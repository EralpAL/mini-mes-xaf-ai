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

    public class Routings : BaseObject {
        private const string CodePrefix = "ROT";

        public Routings(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            Code = BusinessCodeGenerator.GenerateCode(Session, typeof(Routings), CodePrefix);
        }

        protected override void OnSaving() {
            base.OnSaving();
            if (string.IsNullOrEmpty(Code)) {
                Code = BusinessCodeGenerator.GenerateCode(Session, typeof(Routings), CodePrefix);
            }
        }

        private string routingCode;
        [RuleRequiredField]
        [Indexed(Unique = true)]
        [ModelDefault("AllowEdit", "False")]
        public string Code
        {
            get { return routingCode; }
            set { SetPropertyValue(nameof(Code), ref routingCode, value); }
        }

        private string routingName;
        [RuleRequiredField]
        public String Name
        {
            get { return routingName; }
            set { SetPropertyValue(nameof(Name), ref routingName, value); }
        }

        [DevExpress.Xpo.Aggregated]
        [Association("Routings-RoutingDetails")]
        public XPCollection<RoutingDetail> RoutingDetails
        {
            get { return GetCollection<RoutingDetail>(nameof(RoutingDetails)); }
        }

    }
}
