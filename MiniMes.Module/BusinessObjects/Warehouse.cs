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
    [NavigationItem("Stock and Warehouse")]


    public class Warehouse : BaseObject { 
       
        public Warehouse(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
          
        }


        private String warehouseCode;
        [RuleRequiredField]
        [Indexed(Unique = true)]
        public String Code
        {
            get { return warehouseCode; }
            set { SetPropertyValue(nameof(Code), ref warehouseCode, value); }
        }

        private String warehouseName;
        [RuleRequiredField]
       
        public String Name
        {
            get { return warehouseName; }
            set { SetPropertyValue(nameof(Name), ref warehouseName, value); }
        }

        [Association("Warehouse-StockCards")]
        public XPCollection<StockCard> StockCards
        {
            get { return GetCollection<StockCard>(nameof(StockCards)); }
        }


    }
}