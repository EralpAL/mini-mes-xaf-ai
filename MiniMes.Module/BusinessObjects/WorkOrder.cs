using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using MiniMes.Module.Enums;
using MiniMes.Module.Services;
using System.ComponentModel;

namespace MiniMes.Module.BusinessObjects
{
    [DefaultClassOptions]
    [XafDisplayName("İş Emri")]
    [NavigationItem("Production Operations")]
    [DefaultProperty(nameof(Code))]
    [Appearance("Appearance-1", AppearanceItemType.Action, "1=1", TargetItems ="New", Visibility = DevExpress.ExpressApp.Editors.ViewItemVisibility.Hide )]
    [RuleCriteria("WorkOrder_ScrapNotExceedProduced", DefaultContexts.Save,"ScrapQuantity <= ProducedQuantity", CustomMessageTemplate ="Fire miktarı üretilen miktardan fazla olamaz.")]

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

            Code = BusinessCodeGenerator.GenerateCode( Session,typeof(WorkOrder),CodePrefix);
        }

        protected override void OnSaving()
        {
            if (string.IsNullOrEmpty(Code))
            {
                Code = BusinessCodeGenerator.GenerateCode(Session,typeof(WorkOrder),CodePrefix);
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

        /* operasyonun üretim sırasındaki kaçıncı adım olduğunu gösterir.
         SequenceNumber     Operasyon
                1	        Kesim
                2	        Montaj
                3	        Boyama
                4	        Paketleme 
        */

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

        // İş emri başlatılırken seçilen görev.
        
        private JobRole assignedRole;

        [Persistent("AssignedJobRole")]
        public JobRole AssignedRole
        {
            get
            {
                return assignedRole;
            }
            set
            {
                SetPropertyValue(nameof(AssignedRole),ref assignedRole,value);
            }
        }

        private Shift assignedShift;

        public Shift AssignedShift
        {
            get
            {
                return assignedShift;
            }
            set
            {
                SetPropertyValue(nameof(AssignedShift), ref assignedShift, value);
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

        [Association("WorkOrder-DowntimeLogs")]
        public XPCollection<DowntimeLog> DowntimeLogs
        {
            get
            {
                return GetCollection<DowntimeLog>(nameof(DowntimeLogs));
            }
        }

        public void CloseOpenDowntime()
        {
            DateTime endTime = DateTime.Now;

            for (int i = 0; i < DowntimeLogs.Count; i++)
            {
                DowntimeLog downtimeLog = DowntimeLogs[i];

                if (downtimeLog.IsDeleted || downtimeLog.EndTime.HasValue)
                {
                    continue;
                }

                downtimeLog.EndTime = endTime;
            }
        }


        // Üretim girişleri değiştiğinde toplamları baştan hesaplar.
        // Böylece tekrar kaydetme veya düzenleme sırasında çift sayım oluşmaz.

        public void RecalculateTotals(ProductionEntry excludeEntry = null)
        {
            int totalRealized = 0;
            int totalScrap = 0;

            for (int i = 0; i < ProductionEntries.Count; i++)
            {
                ProductionEntry entry = ProductionEntries[i];

                if (entry == excludeEntry || entry.IsDeleted)
                {
                    continue;
                }

                totalRealized = totalRealized + entry.RealizedAmount;
                totalScrap = totalScrap + entry.ScrapAmount;
            }

            int netProduced = totalRealized - totalScrap;

            if (netProduced < 0)
            {
                return;
            }

            ProducedQuantity = netProduced;
            ScrapQuantity = totalScrap;

            if (Status == WorkOrderStatus.Planned && (totalRealized > 0))
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
