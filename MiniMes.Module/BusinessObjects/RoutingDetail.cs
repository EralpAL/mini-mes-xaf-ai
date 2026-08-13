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
    [NavigationItem("Production Definitions")]
    [RuleCombinationOfPropertiesIsUnique("RoutingDetail_UniqueSequenceInRouting", DefaultContexts.Save, "Routings, SequenceNumber", CustomMessageTemplate = "Aynı rota içerisinde iki adım aynı sıra numarasına sahip olamaz.")]

    public class RoutingDetail : BaseObject {
        public RoutingDetail(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            SequenceNumber = 1;
        }

        protected override void OnSaving() {
            ApplyRoutingDefaults();
            base.OnSaving();
        }

        private void ApplyRoutingDefaults()
        {
            if (IsLoading || Routings == null)
            {
                return;
            }

            if (Routings.StockCard != null)
            {
                StockCard = Routings.StockCard;
            }

            int maxSequenceNumber = 0;
            bool sequenceUsedByOtherDetail = false;

            for (int i = 0; i < Routings.RoutingDetails.Count; i++)
            {
                RoutingDetail otherDetail = Routings.RoutingDetails[i];

                if (otherDetail == null || otherDetail == this || otherDetail.IsDeleted)
                {
                    continue;
                }

                if (otherDetail.SequenceNumber > maxSequenceNumber)
                {
                    maxSequenceNumber = otherDetail.SequenceNumber;
                }

                if (otherDetail.SequenceNumber == SequenceNumber)
                {
                    sequenceUsedByOtherDetail = true;
                }
            }

            if (SequenceNumber <= 0 || sequenceUsedByOtherDetail)
            {
                SequenceNumber = maxSequenceNumber + 1;
            }
        }

        private Routings routings;
        [RuleRequiredField]
        [Association("Routings-RoutingDetails")]
        public Routings Routings
        {
            get { return routings; }
            set
            {
                if (SetPropertyValue(nameof(Routings), ref routings, value))
                {
                    ApplyRoutingDefaults();
                }
            }
        }

        private StockCard stockCard;
        [RuleRequiredField("RoutingDetail_StockCardRequired", DefaultContexts.Save, TargetCriteria = "Routings is null OR Routings.StockCard is null")]
        [Association("StockCard-Routings")]
        public StockCard StockCard
        {
            get { return stockCard; }
            set { SetPropertyValue(nameof(StockCard), ref stockCard, value); }
        }

        private Operation operation;
        [RuleRequiredField]
        [Association("Operation-Routings")]
        public Operation Operation
        {
            get { return operation; }
            set { SetPropertyValue(nameof(Operation), ref operation, value); }
        }

        private WorkStation workStation;
        [RuleRequiredField]
        public WorkStation WorkStation
        {
            get { return workStation; }
            set { SetPropertyValue(nameof(WorkStation), ref workStation, value); }
        }

        private int sequenceNumber;
        [RuleRange(1, int.MaxValue, CustomMessageTemplate = "SequenceNumber sıfırdan büyük olmalıdır.")]
        public int SequenceNumber
        {
            get { return sequenceNumber; }
            set { SetPropertyValue(nameof(SequenceNumber), ref sequenceNumber, value); }
        }
    }
}
