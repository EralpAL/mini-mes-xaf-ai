using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using MiniMes.Module.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MiniMes.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Production Operations")]
    public class ProductionOrder : BaseObject
    { 
        public ProductionOrder(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {

            base.AfterConstruction();
            Status = ProductionOrderStatus.Planned;
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

        private decimal plannedQuantity;

        [RuleRange(0.0, double.MaxValue)]
        public decimal PlannedQuantity
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

        private decimal producedQuantity;

        [RuleRange(0.0, double.MaxValue)]
        public decimal ProducedQuantity
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
        // ProductionEntry is saved or deleted. See WorkOrder.RecalculateTotals().
        public void RecalculateTotals()
        {
            decimal totalProduced = 0;
            foreach (WorkOrder workOrder in WorkOrders)
            {
                totalProduced += workOrder.ProducedQuantity;
            }
            ProducedQuantity = totalProduced;
        }
    }
}
