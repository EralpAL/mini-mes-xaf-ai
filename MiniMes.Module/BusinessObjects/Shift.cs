using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace MiniMes.Module.BusinessObjects
{
    [DefaultClassOptions]
    [NavigationItem("Personnel and Shifts")]
    [DefaultProperty(nameof(ShiftName))]
    public class Shift : BaseObject
    {
        public Shift(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            IsActive = true;
        }

        private string shiftName;

        [RuleRequiredField]
        [RuleUniqueValue]
        public string ShiftName
        {
            get
            {
                return shiftName;
            }
            set
            {
                SetPropertyValue(nameof(ShiftName), ref shiftName, value);
            }
        }

        private TimeSpan shiftTime;

        [XafDisplayName("Start Time")]
        public TimeSpan ShiftTime
        {
            get
            {
                return shiftTime;
            }
            set
            {
                SetPropertyValue(nameof(ShiftTime), ref shiftTime, value);
            }
        }

        private TimeSpan endTime;

        public TimeSpan EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                SetPropertyValue(nameof(EndTime), ref endTime, value);
            }
        }

        private bool isActive;

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                SetPropertyValue(nameof(IsActive), ref isActive, value);
            }
        }


    }
}