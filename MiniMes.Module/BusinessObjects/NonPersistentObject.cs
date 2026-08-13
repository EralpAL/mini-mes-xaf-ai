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
using System.Linq;
using System.Text;

namespace MiniMes.Module.BusinessObjects
{
    [DefaultClassOptions]
    // non-persistent 
    [NonPersistent]
    [XafDisplayName("İş Emri Başlat")]
    public class NonPersistentObject : BaseObject
    {
        public NonPersistentObject(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        private Employee selectedEmployee;

        [RuleRequiredField( "NonPersistentObject_EmployeeRequired",DefaultContexts.Save, "İş emrini başlatmak için çalışan seçmelisiniz.")]
        [XafDisplayName("Çalışan")]
        public Employee SelectedEmployee
        {
            get
            {
                return selectedEmployee;
            }
            set
            {
                SetPropertyValue(nameof(SelectedEmployee),ref selectedEmployee,value);
            }
        }



        private JobRole selectedRole;

        [RuleRequiredField( "NonPersistentObject_RoleRequired",DefaultContexts.Save,"İş emrindeki görevi seçmelisiniz.")]
        [XafDisplayName("Görev")]
        public JobRole SelectedRole
        {
            get
            {
                return selectedRole;
            }
            set
            {
                SetPropertyValue( nameof(SelectedRole),ref selectedRole,value);
            }
        }

        private Shift selectedShift;

        [RuleRequiredField("NonPersistentObject_ShiftRequired", DefaultContexts.Save,"Vardiya seçmelisiniz.")]
        [XafDisplayName("Vardiya")]
        public Shift SelectedShift
        {
            get
            {
                return selectedShift;
            }
            set
            {
                SetPropertyValue(nameof(SelectedShift),ref selectedShift,value);
            }
        }
    }
}