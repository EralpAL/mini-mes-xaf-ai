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
    [RuleCriteria("ProductionOrder_PlannedQuantityGreaterThanZero", DefaultContexts.Save, "PlannedQuantity > 0", CustomMessageTemplate = "Planned quantity must be greater than zero.")]
    public class ProductionOrder : BaseObject
    {
        private const string CodePrefix = "PO";

        public ProductionOrder(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {

            base.AfterConstruction();
            Status = ProductionOrderStatus.Planned;
            Code = BusinessCodeGenerator.GenerateCode(Session, typeof(ProductionOrder), CodePrefix);
        }

        protected override void OnSaving()
        {
            base.OnSaving();

            if (string.IsNullOrEmpty(Code))
            {
                Code = BusinessCodeGenerator.GenerateCode(Session, typeof(ProductionOrder), CodePrefix);
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

        private StockCard targetStockCard;

        [RuleRequiredField]
        [Association("StockCard-ProductionOrders")]
        public StockCard TargetStockCard
        {
            get
            {
                return targetStockCard;
            }
            set
            {
                SetPropertyValue(nameof(TargetStockCard), ref targetStockCard, value);
            }
        }

        private int plannedQuantity;
        public int PlannedQuantity
        {
            get
            {
                return plannedQuantity;
            }
            set
            {
                SetPropertyValue(nameof(PlannedQuantity), ref plannedQuantity, value);
            }
        }

        private int producedQuantity;
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

        private ProductionOrderStatus status;

        [ModelDefault("AllowEdit", "False")]
        public ProductionOrderStatus Status
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

        private string aiDelayAnalysis;

        [Size(SizeAttribute.Unlimited)]
        public string AiDelayAnalysis
        {
            get
            {
                return aiDelayAnalysis;
            }
            set
            {
                SetPropertyValue(nameof(AiDelayAnalysis), ref aiDelayAnalysis, value);
            }
        }

        private string aiOptimizationRecommendation;

        [Size(SizeAttribute.Unlimited)]
        public string AiOptimizationRecommendation
        {
            get
            {
                return aiOptimizationRecommendation;
            }
            set
            {
                SetPropertyValue(nameof(AiOptimizationRecommendation), ref aiOptimizationRecommendation, value);
            }
        }

        [Association("ProductionOrder-WorkOrders")]
        public XPCollection<WorkOrder> WorkOrders
        {
            get
            {
                return GetCollection<WorkOrder>(nameof(WorkOrders));
            }
        }

        // Recomputed deterministically from the WorkOrders collection whenever a related
        // ProductionEntry changes. The order's produced quantity is the output of its last
        // routing step, because every step reports the same physical items again.
        // See WorkOrder.RecalculateTotals().
        public void RecalculateTotals()
        {
            int lastSequenceNumber = int.MinValue;

            // Son operasyonun sıra numarasını bul
            for (int i = 0; i < WorkOrders.Count; i++)
            {
                WorkOrder workOrder = WorkOrders[i];

                if (!workOrder.IsDeleted &&
                    workOrder.SequenceNumber > lastSequenceNumber)
                {
                    lastSequenceNumber = workOrder.SequenceNumber;
                }
            }

            int producedAtLastStep = 0;

            // Son operasyonda üretilen miktarları topla
            for (int i = 0; i < WorkOrders.Count; i++)
            {
                WorkOrder workOrder = WorkOrders[i];

                if (!workOrder.IsDeleted &&
                    workOrder.SequenceNumber == lastSequenceNumber)
                {
                    producedAtLastStep += workOrder.ProducedQuantity;
                }
            }

            ProducedQuantity = producedAtLastStep;
        }
    }
}
