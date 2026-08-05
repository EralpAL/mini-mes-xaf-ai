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

    public class Routings : BaseObject { 
        public Routings(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
           
        }

        private StockCard stockCard;
        [Association("StockCard-Routings")]
        public StockCard StockCard
        {
            get { return stockCard; }
            set { SetPropertyValue(nameof(StockCard), ref stockCard, value); }
        }

        private int sequenceNumber;
        public int SequenceNumber
        {
            get { return sequenceNumber; }
            set { SetPropertyValue(nameof(SequenceNumber), ref sequenceNumber, value); }
        }
    
        private Operation operation;
        [Association("Operation-Routings")]
        public Operation Operation
        {
            get { return operation; }
            set { SetPropertyValue(nameof(Operation), ref operation, value); }
        }

    }
}