using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MiniMes.Module.BusinessObjects
{
    // üretim fire girişi 
    [NonPersistent]
    [XafDisplayName("Üretim Girişi")]
    // user friendly execptionlar sildik yerine rule ekledik

    [Appearance("ProductionEntryParameters_HideScrapFields", AppearanceItemType.ViewItem, "!IsScrapEntry", TargetItems = "ScrapAmount;AvailableQuantity", Visibility = ViewItemVisibility.Hide)]
    [Appearance("ProductionEntryParameters_HideProductionFields", AppearanceItemType.ViewItem, "IsScrapEntry", TargetItems = "RealizedAmount", Visibility = ViewItemVisibility.Hide)]

    [RuleCriteria("ProductionEntryParameters_WorkStationRequired", DefaultContexts.Save, "HasWorkStation", CustomMessageTemplate = "Üretim veya fire girişi yapabilmek için iş emrine iş istasyonu atanmış olmalıdır.")]
    [RuleCriteria("ProductionEntryParameters_RealizedAmount", DefaultContexts.Save, "IsScrapEntry OR RealizedAmount > 0", CustomMessageTemplate = "Üretilen miktar sıfırdan büyük olmalıdır.")]
    [RuleCriteria("ProductionEntryParameters_ProductionExists", DefaultContexts.Save, "!IsScrapEntry OR AvailableQuantity > 0", CustomMessageTemplate = "Fire girişi yapabilmek için önce üretim girişi yapılmalıdır.")]
    [RuleCriteria("ProductionEntryParameters_ScrapAmountPositive", DefaultContexts.Save, "!IsScrapEntry OR ScrapAmount > 0", CustomMessageTemplate = "Fire miktarı sıfırdan büyük olmalıdır.")]
    [RuleCriteria("ProductionEntryParameters_ScrapAmountNotExceedProduced", DefaultContexts.Save, "!IsScrapEntry OR ScrapAmount <= AvailableQuantity", CustomMessageTemplate = "Fire miktarı, mevcut üretim miktarından fazla olamaz.")]
    public class ProductionEntryParameters : BaseObject
    {
        public ProductionEntryParameters(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private bool isScrapEntry;

        [Browsable(false)]
        public bool IsScrapEntry
        {
            get
            {
                return isScrapEntry;
            }
            set
            {
                SetPropertyValue(nameof(IsScrapEntry), ref isScrapEntry, value);
            }
        }

        private bool hasWorkStation;

        [Browsable(false)]
        public bool HasWorkStation
        {
            get
            {
                return hasWorkStation;
            }
            set
            {
                SetPropertyValue(nameof(HasWorkStation), ref hasWorkStation, value);
            }
        }

        private int realizedAmount;

        [XafDisplayName("Üretilen Miktar")]
        public int RealizedAmount
        {
            get
            {
                return realizedAmount;
            }
            set
            {
                SetPropertyValue(nameof(RealizedAmount), ref realizedAmount, value);
            }
        }

        private int availableQuantity;

        [XafDisplayName("Mevcut Üretim Miktarı")]
        [ModelDefault("AllowEdit", "False")]
        public int AvailableQuantity
        {
            get
            {
                return availableQuantity;
            }
            set
            {
                SetPropertyValue(nameof(AvailableQuantity), ref availableQuantity, value);
            }
        }

        private int scrapAmount;

        [XafDisplayName("Fire Miktarı")]
        public int ScrapAmount
        {
            get
            {
                return scrapAmount;
            }
            set
            {
                SetPropertyValue(nameof(ScrapAmount), ref scrapAmount, value);
            }
        }
    }
}
