using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Updating;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using Microsoft.Extensions.DependencyInjection;
using MiniMes.Module.BusinessObjects;
using System;

namespace MiniMes.Module.DatabaseUpdate
{
    public class Updater : ModuleUpdater
    {
        public Updater(
            IObjectSpace objectSpace,
            Version currentDBVersion)
            : base(objectSpace, currentDBVersion)
        {
        }

        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();

#if !RELEASE
            PermissionPolicyRole defaultRole =
                CreateDefaultRole();

            PermissionPolicyRole adminRole =
                CreateAdminRole();

            ObjectSpace.CommitChanges();

            UserManager userManager =
                ObjectSpace.ServiceProvider
                    .GetRequiredService<UserManager>();

            if (userManager.FindUserByName<ApplicationUser>(
                ObjectSpace,
                "User") == null)
            {
                string emptyPassword = "";

                userManager.CreateUser<ApplicationUser>(
                    ObjectSpace,
                    "User",
                    emptyPassword,
                    user =>
                    {
                        user.Roles.Add(defaultRole);
                    });
            }

            if (userManager.FindUserByName<ApplicationUser>(
                ObjectSpace,
                "Admin") == null)
            {
                string emptyPassword = "";

                userManager.CreateUser<ApplicationUser>(
                    ObjectSpace,
                    "Admin",
                    emptyPassword,
                    user =>
                    {
                        user.Roles.Add(adminRole);
                    });
            }

            ObjectSpace.CommitChanges();
#endif
        }

        public override void UpdateDatabaseBeforeUpdateSchema()
        {
            base.UpdateDatabaseBeforeUpdateSchema();
        }

        private PermissionPolicyRole CreateAdminRole()
        {
            PermissionPolicyRole adminRole =
                ObjectSpace.FirstOrDefault<PermissionPolicyRole>(
                    role => role.Name == "Administrators");

            if (adminRole == null)
            {
                adminRole =
                    ObjectSpace.CreateObject<PermissionPolicyRole>();

                adminRole.Name = "Administrators";
                adminRole.IsAdministrative = true;
            }

            return adminRole;
        }

        private PermissionPolicyRole CreateDefaultRole()
        {
            PermissionPolicyRole defaultRole =
                ObjectSpace.FirstOrDefault<PermissionPolicyRole>(
                    role => role.Name == "Default");

            if (defaultRole == null)
            {
                defaultRole =
                    ObjectSpace.CreateObject<PermissionPolicyRole>();

                defaultRole.Name = "Default";

                defaultRole.AddObjectPermissionFromLambda<ApplicationUser>(
                    SecurityOperations.Read,
                    user => user.Oid ==
                        (Guid)CurrentUserIdOperator.CurrentUserId(),
                    SecurityPermissionState.Allow);

                defaultRole.AddNavigationPermission(
                    @"Application/NavigationItems/Items/Default/Items/MyDetails",
                    SecurityPermissionState.Allow);

                defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(
                    SecurityOperations.Write,
                    "ChangePasswordOnFirstLogon",
                    user => user.Oid ==
                        (Guid)CurrentUserIdOperator.CurrentUserId(),
                    SecurityPermissionState.Allow);

                defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(
                    SecurityOperations.Write,
                    "StoredPassword",
                    user => user.Oid ==
                        (Guid)CurrentUserIdOperator.CurrentUserId(),
                    SecurityPermissionState.Allow);

                defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(
                    SecurityOperations.Read,
                    SecurityPermissionState.Deny);

                defaultRole.AddObjectPermission<ModelDifference>(
                    SecurityOperations.ReadWriteAccess,
                    "UserId = ToStr(CurrentUserId())",
                    SecurityPermissionState.Allow);

                defaultRole.AddObjectPermission<ModelDifferenceAspect>(
                    SecurityOperations.ReadWriteAccess,
                    "Owner.UserId = ToStr(CurrentUserId())",
                    SecurityPermissionState.Allow);

                defaultRole.AddTypePermissionsRecursively<ModelDifference>(
                    SecurityOperations.Create,
                    SecurityPermissionState.Allow);

                defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(
                    SecurityOperations.Create,
                    SecurityPermissionState.Allow);

                defaultRole.AddTypePermission<AuditDataItemPersistent>(
                    SecurityOperations.Read,
                    SecurityPermissionState.Deny);

                defaultRole.AddObjectPermissionFromLambda<AuditDataItemPersistent>(
                    SecurityOperations.Read,
                    item => item.UserId ==
                        CurrentUserIdOperator.CurrentUserId().ToString(),
                    SecurityPermissionState.Allow);

                defaultRole.AddTypePermission<AuditedObjectWeakReference>(
                    SecurityOperations.Read,
                    SecurityPermissionState.Allow);
            }

            return defaultRole;
        }
    }
}