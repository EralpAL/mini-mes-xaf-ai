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

    public class RoutingDetail : BaseObject {
        public RoutingDetail(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
        }

        private Routings routings;
        [RuleRequiredField]
        [Association("Routings-RoutingDetails")]
        public Routings Routings
        {
            get { return routings; }
            set { SetPropertyValue(nameof(Routings), ref routings, value); }
        }

        private StockCard stockCard;
        [RuleRequiredField]
        [Association("StockCard-Routings")]
        public StockCard StockCard
        {
            get { return stockCard; }
            set { SetPropertyValue(nameof(StockCard), ref stockCard, value); }
        }

        private Operation operation;
        [RuleRequiredField]
        [Association("Operation-Routings")]
        public Operation Operation
        {
            get { return operation; }
            set { SetPropertyValue(nameof(Operation), ref operation, value); }
        }

        private WorkStation workStation;
        public WorkStation WorkStation
        {
            get { return workStation; }
            set { SetPropertyValue(nameof(WorkStation), ref workStation, value); }
        }

        private int sequenceNumber;
        [RuleRange(0, int.MaxValue)]
        public int SequenceNumber
        {
            get { return sequenceNumber; }
            set { SetPropertyValue(nameof(SequenceNumber), ref sequenceNumber, value); }
        }
    }
}
