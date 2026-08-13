using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;


namespace MiniMes.Module.BusinessObjects
{
    [NonPersistent]
    [XafDisplayName("Üretim Girişi")]
    [RuleCriteria("ProductionEntryParameters_RealizedAmount", DefaultContexts.Save, "RealizedAmount > 0", CustomMessageTemplate = "Üretilen miktar sıfırdan büyük olmalıdır.")]
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

        private int realizedAmount;

        [XafDisplayName("Üretilen Miktar")]
        public int RealizedAmount
        {
            get => realizedAmount;
            set => SetPropertyValue(nameof(RealizedAmount), ref realizedAmount, value);
        }
    }
}