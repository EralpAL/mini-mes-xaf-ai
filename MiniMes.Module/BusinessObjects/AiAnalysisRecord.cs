using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using System;
using System.ComponentModel;

namespace MiniMes.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("AI Sorgu Geçmişi")]
    [CreatableItem(false)]
    [DefaultProperty(nameof(AnalysisDate))]
    [XafDisplayName("AI Sorgu Geçmişi")]
    [ModelDefault("AllowEdit", "False")]
    [ModelDefault("AllowNew", "False")]
    [ModelDefault("AllowDelete", "False")]
    [ModelDefault("Caption", "AI Sorgu Geçmişi")]
    [Appearance("AiAnalysisRecord_HideNew", AppearanceItemType.Action, "1=1", TargetItems = "New", Visibility = ViewItemVisibility.Hide)]
    [Appearance("AiAnalysisRecord_HideDelete", AppearanceItemType.Action, "1=1", TargetItems = "Delete", Visibility = ViewItemVisibility.Hide)]
    [Appearance("AiAnalysisRecord_HideLinkUnlink", AppearanceItemType.Action, "1=1", TargetItems = "Link;Unlink", Visibility = ViewItemVisibility.Hide)]
    public class AiAnalysisRecord : BaseObject
    {
        public AiAnalysisRecord(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            AnalysisDate = DateTime.Now;
        }

        private ProductionOrder productionOrder;

        [Association("ProductionOrder-AiAnalysisRecords")]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
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

        private DateTime analysisDate;

        [Index(0)]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Sorgu Tarihi")]
        public DateTime AnalysisDate
        {
            get
            {
                return analysisDate;
            }
            set
            {
                SetPropertyValue(nameof(AnalysisDate), ref analysisDate, value);
            }
        }

        private string modelName;

        [Index(1)]
        [ModelDefault("AllowEdit", "False")]
        [XafDisplayName("Kullanılan Model")]
        public string ModelName
        {
            get
            {
                return modelName;
            }
            set
            {
                SetPropertyValue(nameof(ModelName), ref modelName, value);
            }
        }

        private string userQuestion;

        [Index(2)]
        [Size(SizeAttribute.Unlimited)]
        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("RowCount", "4")]
        [XafDisplayName("Kullanıcı Sorusu")]
        public string UserQuestion
        {
            get
            {
                return userQuestion;
            }
            set
            {
                SetPropertyValue(nameof(UserQuestion), ref userQuestion, value);
            }
        }

        private string analysisText;

        [Index(3)]
        [Size(SizeAttribute.Unlimited)]
        [ModelDefault("AllowEdit", "False")]
        [ModelDefault("RowCount", "9")]
        [XafDisplayName("AI Yanıtı")]
        public string AnalysisText
        {
            get
            {
                return analysisText;
            }
            set
            {
                SetPropertyValue(nameof(AnalysisText), ref analysisText, value);
            }
        }

        private string optimizationRecommendation;

        [Size(SizeAttribute.Unlimited)]
        [VisibleInDetailView(false)]
        [VisibleInListView(false)]
        [VisibleInLookupListView(false)]
        public string OptimizationRecommendation
        {
            get
            {
                return optimizationRecommendation;
            }
            set
            {
                SetPropertyValue(nameof(OptimizationRecommendation), ref optimizationRecommendation, value);
            }
        }
    }
}
