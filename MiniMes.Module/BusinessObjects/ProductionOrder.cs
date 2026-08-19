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
    [XafDisplayName("Üretim Emri")]
    [NavigationItem("Production Operations")]
    [DefaultProperty(nameof(Code))]
    [RuleCriteria("ProductionOrder_Rule1",DefaultContexts.Save,"PlannedQuantity > 0",CustomMessageTemplate ="Planned quantity must be greater than zero!!")]
    [RuleCriteria("ProductionOrder_RoutingMatchesStockCard", DefaultContexts.Save, "Routing is null OR StockCard is null OR Routing.StockCard = StockCard", CustomMessageTemplate = "Seçilen rota, üretim emrinin stok kartına ait olmalıdır.")]
    // Rule 2 sildim stockCard zaten rule ile kontrol ediliyor,
    // ayrıca stockCard boş olamaz çünkü requiredfield attribute var
    public class ProductionOrder : BaseObject
    {
        public ProductionOrder(Session session)
            : base(session)
        {
        }

        private const string CodePrefix = "PO";

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            Status = ProductionOrderStatus.Planned;

            Code = BusinessCodeGenerator.GenerateCode(Session,typeof(ProductionOrder),CodePrefix);
        }

        protected override void OnSaving()
        {
            if (string.IsNullOrEmpty(Code))
            {
                Code = BusinessCodeGenerator.GenerateCode( Session,typeof(ProductionOrder),CodePrefix);
            }

            base.OnSaving();
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
        [XafDisplayName("Stok Kartı")]
        [Association("StockCard-ProductionOrders")]
        public StockCard StockCard
        {
            get
            {
                return targetStockCard;
            }
            set
            {
                SetPropertyValue( nameof(StockCard), ref targetStockCard,value);
            }
        }

        private Routings routing;

        [RuleRequiredField]
        [XafDisplayName("Rota")]
        [DataSourceCriteria("StockCard = '@This.StockCard'")]
        public Routings Routing
        {
            get
            {
                return routing;
            }
            set
            {
                SetPropertyValue(nameof(Routing), ref routing, value);
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
                SetPropertyValue( nameof(PlannedQuantity),ref plannedQuantity,value);
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
                SetPropertyValue(nameof(ProducedQuantity),ref producedQuantity,value);
            }
        }

        // Üretim emrine bağlı bütün iş emirlerindeki
        // toplam fire miktarını tutar.
        private int scrapQuantity;

        [ModelDefault("AllowEdit", "False")]
        public int ScrapQuantity
        {
            get
            {
                return scrapQuantity;
            }
            set
            {
                SetPropertyValue( nameof(ScrapQuantity), ref scrapQuantity,value);
            }
        }

        // Henüz tamamlanmamış miktarı hesaplar.
        // Veritabanında ayrı bir kolon olarak tutulmaz.
        [NonPersistent]
        [ModelDefault("AllowEdit", "False")]
        public int RemainingQuantity
        {
            get
            {
                int remainingQuantity = PlannedQuantity -ProducedQuantity -ScrapQuantity;

                if (remainingQuantity < 0)
                {
                    return 0;
                }

                return remainingQuantity;
            }
        }

        // Üretilen ve fire olarak işlenen toplam miktarın
        // planlanan miktara oranını hesaplar.
        [NonPersistent]
        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("DisplayFormat", "{0:N2} %")]
        public double CompletionPercentage
        {
            get
            {
                if (PlannedQuantity <= 0)
                {
                    return 0;
                }

                double processedQuantity = ProducedQuantity + ScrapQuantity;

                double percentage = processedQuantity * 100 / PlannedQuantity;

                if (percentage > 100)
                {
                    return 100;
                }

                return percentage;
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
                SetPropertyValue(nameof(Status), ref status,value);
            }
        }

        private string aiDelayAnalysis;

        [Size(SizeAttribute.Unlimited)]
        // kullanıcı elle değiştirmesin
        [ModelDefault("AllowEdit", "False")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string AiDelayAnalysis
        {
            get
            {
                return aiDelayAnalysis;
            }
            set
            {
                SetPropertyValue( nameof(AiDelayAnalysis),ref aiDelayAnalysis,value);
            }
        }

        private string aiOptimizationRecommendation;

        [Size(SizeAttribute.Unlimited)]
        // kullanıcı elle değiştirmesin
        [ModelDefault("AllowEdit", "False")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string AiOptimizationRecommendation
        {
            get
            {
                return aiOptimizationRecommendation;
            }
            set
            {
                SetPropertyValue(  nameof(AiOptimizationRecommendation),  ref aiOptimizationRecommendation, value);
            }
        }

        [Association("ProductionOrder-WorkOrders")]
        public XPCollection<WorkOrder> WorkOrders
        {
            get
            {
                return GetCollection<WorkOrder>(
                    nameof(WorkOrders));
            }
        }

        [Association("ProductionOrder-AiAnalysisRecords")]
        [XafDisplayName("AI Analiz Geçmişi")]
        public XPCollection<AiAnalysisRecord> AiAnalysisRecords
        {
            get
            {
                return GetCollection<AiAnalysisRecord>(nameof(AiAnalysisRecords));
            }
        }

        // Üretim emrindeki en son operasyonu bulur.
        // Üretilen sağlam miktarı yalnızca son operasyondan alır.
        // Fire miktarını ise bütün iş emirlerinden toplar.
        public void RecalculateTotals()
        {
            int lastSequenceNumber = int.MinValue;
            int totalScrap = 0;

            // Son operasyonu bul ve bütün fireleri topla.
            for (int i = 0; i < WorkOrders.Count; i++)
            {
                WorkOrder workOrder = WorkOrders[i];

                if (workOrder.IsDeleted)
                {
                    continue;
                }

                totalScrap += workOrder.ScrapQuantity;

                if (workOrder.SequenceNumber > lastSequenceNumber)
                {
                    lastSequenceNumber =workOrder.SequenceNumber;
                }
            }

            int producedAtLastStep = 0;

            // Yalnızca son operasyondaki sağlam üretimi topla.
            for (int i = 0; i < WorkOrders.Count; i++)
            {
                WorkOrder workOrder = WorkOrders[i];

                if (!workOrder.IsDeleted &&workOrder.SequenceNumber ==lastSequenceNumber)
                {
                    producedAtLastStep +=workOrder.ProducedQuantity;
                }
            }

            ProducedQuantity = producedAtLastStep;
            ScrapQuantity = totalScrap;

            // Hesaplanan alanların ekranda yenilenmesini sağlar.
            OnChanged(nameof(RemainingQuantity));
            OnChanged(nameof(CompletionPercentage));
        }
    }
}