using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System.ComponentModel;

namespace MiniMes.Module.BusinessObjects
{
    [NonPersistent]
    [XafDisplayName("Duruş Başlat")]
    [RuleCriteria("DowntimeStartParameters_WorkStationRequired", DefaultContexts.Save, "HasWorkStation", CustomMessageTemplate = "Duruş başlatmak için iş emrine iş istasyonu atanmış olmalıdır.")]
    public class DowntimeStartParameters : BaseObject
    {
        public DowntimeStartParameters(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
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

        private StopCause selectedStopCause;

        [RuleRequiredField("DowntimeStartParameters_StopCauseRequired", DefaultContexts.Save, "Duruş başlatmak için duruş nedeni seçmelisiniz.")]
        [XafDisplayName("Duruş Nedeni")]
        public StopCause SelectedStopCause
        {
            get
            {
                return selectedStopCause;
            }
            set
            {
                SetPropertyValue(nameof(SelectedStopCause), ref selectedStopCause, value);
            }
        }
    }
}
