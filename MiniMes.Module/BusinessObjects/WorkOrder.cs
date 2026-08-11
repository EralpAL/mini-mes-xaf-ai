using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using MiniMes.Module.Enums;
using MiniMes.Module.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MiniMes.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Production Operations")]
    [DefaultProperty(nameof(Code))]
    public class WorkOrder : BaseObject
    { 
        private const string CodePrefix = "WO";

        public WorkOrder(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            Status = WorkOrderStatus.Planned;

            // The Approve action of a Production Order replaces this code with one derived from
            // the order code. A Work Order created directly by a user keeps the generated code.
            Code = BusinessCodeGenerator.GenerateCode(Session, typeof(WorkOrder), CodePrefix);
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            if (string.IsNullOrEmpty(Code))
            {
                Code = BusinessCodeGenerator.GenerateCode(Session, typeof(WorkOrder), CodePrefix);
            }
        }

        private string code;

        [RuleRequiredField]
        [RuleUniqueValue]
        [ModelDefault("AllowEdit", "False")]
        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                SetPropertyValue(nameof(Code), ref code, value);
            }
        }

        private int sequenceNumber;

        [RuleRange(0, int.MaxValue)]
        public int SequenceNumber
        {
            get
            {
                return sequenceNumber;
            }
            set
            {
                SetPropertyValue(nameof(SequenceNumber), ref sequenceNumber, value);
            }
        }

        private ProductionOrder productionOrder;

        [RuleRequiredField]
        [Association("ProductionOrder-WorkOrders")]
        public ProductionOrder ProductionOrder
        {
            get
            {
                return productionOrder;
            }
            set
            {
                SetPropertyValue(nameof(ProductionOrder), ref productionOrder, value);
            }
        }

        private WorkStation assignedWorkStation;

        public WorkStation AssignedWorkStation
        {
            get
            {
                return assignedWorkStation;
            }
            set
            {
                SetPropertyValue(nameof(AssignedWorkStation), ref assignedWorkStation, value);
            }
        }

        private Operation operation;

        public Operation Operation
        {
            get
            {
                return operation;
            }
            set
            {
                SetPropertyValue(nameof(Operation), ref operation, value);
            }
        }

        private Employee assignedEmployee;

        public Employee AssignedEmployee
        {
            get
            {
                return assignedEmployee;
            }
            set
            {
                SetPropertyValue(nameof(AssignedEmployee), ref assignedEmployee, value);
            }
        }

        private WorkOrderStatus status;

        public WorkOrderStatus Status
        {
            get
            {
                return status;
            }
            set
            {
                SetPropertyValue(nameof(Status), ref status, value);
            }
        }

        private int producedQuantity;

        [RuleRange(0, int.MaxValue)]
        [ModelDefault("AllowEdit", "False")]
        public int ProducedQuantity
        {
            get
            {
                return producedQuantity;
            }
            set
            {
                SetPropertyValue(nameof(ProducedQuantity), ref producedQuantity, value);
            }
        }

        private int scrapQuantity;

        [RuleRange(0, int.MaxValue)]
        [ModelDefault("AllowEdit", "False")]
        public int ScrapQuantity
        {
            get
            {
                return scrapQuantity;
            }
            set
            {
                SetPropertyValue(nameof(ScrapQuantity), ref scrapQuantity, value);
            }
        }

        [Association("WorkOrder-ProductionEntries")]
        public XPCollection<ProductionEntry> ProductionEntries
        {
            get
            {
                return GetCollection<ProductionEntry>(nameof(ProductionEntries));
            }
        }

        // Deterministic aggregation: always recalculated from the complete ProductionEntries
        // collection instead of incrementally adding/subtracting values, so repeated saves or
        // edits of an existing entry never double-count. Called by ProductionEntry whenever one
        // of its quantity/parent properties changes, and again on save and delete.
        public void RecalculateTotals(ProductionEntry excludeEntry = null)
        {
            int totalProduced = 0;
            int totalScrap = 0;

            for (int i = 0; i < ProductionEntries.Count; i++)
            {
                ProductionEntry entry = ProductionEntries[i];

                if (entry == excludeEntry || entry.IsDeleted)
                {
                    continue;
                }

                totalProduced = totalProduced + entry.RealizedAmount;
                totalScrap = totalScrap + entry.ScrapAmount;
            }

            ProducedQuantity = totalProduced;
            ScrapQuantity = totalScrap;

            if (Status == WorkOrderStatus.Planned && (totalProduced > 0 || totalScrap > 0))
            {
                Status = WorkOrderStatus.InProgress;
            }

            if (ProductionOrder != null)
            {
                ProductionOrder.RecalculateTotals();
            }
        }
    }
}
