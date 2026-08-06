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
    [NavigationItem("Stock and Warehouse")]


    public class StockCard : BaseObject { 
        
        public StockCard(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();

    }
        private string stockCode;
        [RuleRequiredField]
        [Indexed(Unique = true)]
        public string Code
        {
            get { return stockCode; }
            set { SetPropertyValue(nameof(Code), ref stockCode, value); }
        }

        private string stockName;
        [RuleRequiredField]
        

        public String Name
        {
            get { return stockName; }
            set { SetPropertyValue(nameof(Name), ref stockName, value); }
        }

        private StockType stockType;
        public StockType StockType
        {
            get { return stockType; }
            set { SetPropertyValue(nameof(StockType), ref stockType, value); }
        }

        private Warehouse warehouse;
        [Association("Warehouse-StockCards")]
        public Warehouse Warehouse
        {
            get { return warehouse; }
            set { SetPropertyValue(nameof(Warehouse), ref warehouse, value); }
        }
        [Association("StockCard-Routings")]
        public XPCollection<RoutingDetail> Routings
        {
            get { return GetCollection<RoutingDetail>(nameof(Routings)); }
        }

        [Association("StockCard-ProductionOrders")]
        public XPCollection<ProductionOrder> ProductionOrders
        {
            get
            {
                return GetCollection<ProductionOrder>(nameof(ProductionOrders));
            }
        }


    }
}