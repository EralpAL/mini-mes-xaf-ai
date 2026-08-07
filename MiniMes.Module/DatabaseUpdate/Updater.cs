using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.Xpo;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using MiniMes.Module.BusinessObjects;
using MiniMes.Module.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace MiniMes.Module.DatabaseUpdate;

// For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.Updating.ModuleUpdater
public class Updater : ModuleUpdater {
    public Updater(IObjectSpace objectSpace, Version currentDBVersion) :
        base(objectSpace, currentDBVersion) {
    }
    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();
        //string name = "MyName";
        //DomainObject1 theObject = ObjectSpace.FirstOrDefault<DomainObject1>(u => u.Name == name);
        //if(theObject == null) {
        //    theObject = ObjectSpace.CreateObject<DomainObject1>();
        //    theObject.Name = name;
        //}

        // The code below creates users and roles for testing purposes only.
        // In production code, you can create users and assign roles to them automatically, as described in the following help topic:
        // https://docs.devexpress.com/eXpressAppFramework/119064/data-security-and-safety/security-system/authentication
#if !RELEASE
        // If a role doesn't exist in the database, create this role
        var defaultRole = CreateDefaultRole();
        var adminRole = CreateAdminRole();

        ObjectSpace.CommitChanges(); //This line persists created object(s).

        UserManager userManager = ObjectSpace.ServiceProvider.GetRequiredService<UserManager>();
        // If a user named 'User' doesn't exist in the database, create this user
        if(userManager.FindUserByName<ApplicationUser>(ObjectSpace, "User") == null) {
            // Set a password if the standard authentication type is used
            string EmptyPassword = "";
            _ = userManager.CreateUser<ApplicationUser>(ObjectSpace, "User", EmptyPassword, (user) => {
                // Add the Users role to the user
                user.Roles.Add(defaultRole);
            });
        }

        // If a user named 'Admin' doesn't exist in the database, create this user
        if(userManager.FindUserByName<ApplicationUser>(ObjectSpace, "Admin") == null) {
            // Set a password if the standard authentication type is used
            string EmptyPassword = "";
            _ = userManager.CreateUser<ApplicationUser>(ObjectSpace, "Admin", EmptyPassword, (user) => {
                // Add the Administrators role to the user
                user.Roles.Add(adminRole);
            });
        }

        ObjectSpace.CommitChanges(); //This line persists created object(s).

        CreateDemoData();
        ObjectSpace.CommitChanges();
#endif
    }

    // Master data used to demonstrate the production flow. Every item is looked up by its
    // business code first, so running the updater again never duplicates a record and never
    // overwrites data that was edited in the application.
    private void CreateDemoData() {
        Warehouse mainWarehouse = EnsureWarehouse("WH-01", "Main Warehouse");

        EnsureStockCard("RM-001", "Steel Sheet 2 mm", EnumStockType.RawMaterial, mainWarehouse);
        StockCard cutBlank = EnsureStockCard("SF-001", "Cut Blank", EnumStockType.SemiFinished, mainWarehouse);
        StockCard bracket = EnsureStockCard("FP-001", "Steel Bracket", EnumStockType.FinishedProduct, mainWarehouse);

        WorkStation cuttingStation = EnsureWorkStation("CNC-01", "CNC Cutting Machine", 320.0);
        WorkStation pressStation = EnsureWorkStation("PRS-01", "Hydraulic Press", 210.0);
        WorkStation assemblyStation = EnsureWorkStation("ASM-01", "Assembly Line", 150.0);

        EnsureEquipment("EQ-01", "Cutting Head", cuttingStation);
        EnsureEquipment("EQ-02", "Bracket Mold", pressStation);
        EnsureEquipment("EQ-03", "Torque Screwdriver", assemblyStation);

        Operation cuttingOperation = EnsureOperation("OP-10", "Cutting", "Cut the raw sheet to blank size.");
        Operation pressingOperation = EnsureOperation("OP-20", "Pressing", "Form the blank in the press.");
        Operation assemblyOperation = EnsureOperation("OP-30", "Assembly", "Assemble and pack the bracket.");

        EnsureStopCause("STP-01", "Electrical Failure", StopCategory.Unplanned);
        EnsureStopCause("STP-02", "Material Shortage", StopCategory.Unplanned);
        EnsureStopCause("STP-03", "Setup / Adjustment", StopCategory.Planned);
        EnsureStopCause("STP-04", "Mold Change", StopCategory.Planned);

        Shift dayShift = EnsureShift("Day Shift", new TimeSpan(8, 0, 0), new TimeSpan(16, 0, 0));
        Shift eveningShift = EnsureShift("Evening Shift", new TimeSpan(16, 0, 0), new TimeSpan(0, 0, 0));

        EnsureEmployee("1001", "Ahmet Yilmaz", EnumEmployeeRole.Operator, dayShift, cuttingStation);
        EnsureEmployee("1002", "Elif Demir", EnumEmployeeRole.Operator, dayShift, pressStation);
        EnsureEmployee("1003", "Mehmet Kaya", EnumEmployeeRole.ShiftLeader, eveningShift, assemblyStation);
        EnsureEmployee("1004", "Zeynep Sahin", EnumEmployeeRole.QualityInspector, dayShift, assemblyStation);
        EnsureEmployee("1005", "Burak Aydin", EnumEmployeeRole.MaintenanceEngineer, eveningShift, cuttingStation);

        Routings bracketRouting = EnsureRouting("ROT-001", "Steel Bracket Routing");
        Routings blankRouting = EnsureRouting("ROT-002", "Cut Blank Routing");

        // Routing headers must have keys before their steps can be looked up by header code.
        ObjectSpace.CommitChanges();

        EnsureRoutingDetail(bracketRouting, 10, bracket, cuttingOperation, cuttingStation);
        EnsureRoutingDetail(bracketRouting, 20, bracket, pressingOperation, pressStation);
        EnsureRoutingDetail(bracketRouting, 30, bracket, assemblyOperation, assemblyStation);
        EnsureRoutingDetail(blankRouting, 10, cutBlank, cuttingOperation, cuttingStation);
    }

    private Warehouse EnsureWarehouse(string code, string name) {
        Warehouse warehouse = ObjectSpace.FirstOrDefault<Warehouse>(item => item.Code == code);
        if(warehouse == null) {
            warehouse = ObjectSpace.CreateObject<Warehouse>();
            warehouse.Code = code;
            warehouse.Name = name;
        }
        return warehouse;
    }

    private StockCard EnsureStockCard(string code, string name, EnumStockType stockType, Warehouse warehouse) {
        StockCard stockCard = ObjectSpace.FirstOrDefault<StockCard>(item => item.Code == code);
        if(stockCard == null) {
            stockCard = ObjectSpace.CreateObject<StockCard>();
            stockCard.Code = code;
            stockCard.Name = name;
            stockCard.StockType = stockType;
            stockCard.Warehouse = warehouse;
        }
        return stockCard;
    }

    private WorkStation EnsureWorkStation(string code, string name, double hourlyCost) {
        WorkStation workStation = ObjectSpace.FirstOrDefault<WorkStation>(item => item.Code == code);
        if(workStation == null) {
            workStation = ObjectSpace.CreateObject<WorkStation>();
            workStation.Code = code;
            workStation.Name = name;
            workStation.HourlyCost = hourlyCost;
            workStation.IsActive = true;
        }
        return workStation;
    }

    private Equipment EnsureEquipment(string code, string name, WorkStation workStation) {
        Equipment equipment = ObjectSpace.FirstOrDefault<Equipment>(item => item.Code == code);
        if(equipment == null) {
            equipment = ObjectSpace.CreateObject<Equipment>();
            equipment.Code = code;
            equipment.Name = name;
            equipment.WorkStation = workStation;
            equipment.IsActive = true;
        }
        return equipment;
    }

    private Operation EnsureOperation(string code, string name, string description) {
        Operation operation = ObjectSpace.FirstOrDefault<Operation>(item => item.Code == code);
        if(operation == null) {
            operation = ObjectSpace.CreateObject<Operation>();
            operation.Code = code;
            operation.Name = name;
            operation.Description = description;
            operation.IsActive = true;
        }
        return operation;
    }

    private StopCause EnsureStopCause(string code, string name, StopCategory category) {
        StopCause stopCause = ObjectSpace.FirstOrDefault<StopCause>(item => item.Code == code);
        if(stopCause == null) {
            stopCause = ObjectSpace.CreateObject<StopCause>();
            stopCause.Code = code;
            stopCause.Name = name;
            stopCause.Category = category;
        }
        return stopCause;
    }

    private Shift EnsureShift(string shiftName, TimeSpan startTime, TimeSpan endTime) {
        Shift shift = ObjectSpace.FirstOrDefault<Shift>(item => item.ShiftName == shiftName);
        if(shift == null) {
            shift = ObjectSpace.CreateObject<Shift>();
            shift.ShiftName = shiftName;
            shift.ShiftTime = startTime;
            shift.EndTime = endTime;
            shift.IsActive = true;
        }
        return shift;
    }

    private Employee EnsureEmployee(string registrationNumber, string fullName, EnumEmployeeRole role, Shift shift, WorkStation workStation) {
        Employee employee = ObjectSpace.FirstOrDefault<Employee>(item => item.RegistrationNumber == registrationNumber);
        if(employee == null) {
            employee = ObjectSpace.CreateObject<Employee>();
            employee.RegistrationNumber = registrationNumber;
            employee.FullName = fullName;
            employee.Role = role;
            employee.AssignedShift = shift;
            employee.WorkStation = workStation;
        }
        return employee;
    }

    private Routings EnsureRouting(string code, string name) {
        Routings routing = ObjectSpace.FirstOrDefault<Routings>(item => item.Code == code);
        if(routing == null) {
            routing = ObjectSpace.CreateObject<Routings>();
            routing.Code = code;
            routing.Name = name;
        }
        return routing;
    }

    private RoutingDetail EnsureRoutingDetail(Routings routing, int sequenceNumber, StockCard stockCard, Operation operation, WorkStation workStation) {
        string routingCode = routing.Code;
        RoutingDetail routingDetail = ObjectSpace.FirstOrDefault<RoutingDetail>(
            item => item.Routings.Code == routingCode && item.SequenceNumber == sequenceNumber);
        if(routingDetail == null) {
            routingDetail = ObjectSpace.CreateObject<RoutingDetail>();
            routingDetail.Routings = routing;
            routingDetail.SequenceNumber = sequenceNumber;
            routingDetail.StockCard = stockCard;
            routingDetail.Operation = operation;
            routingDetail.WorkStation = workStation;
        }
        return routingDetail;
    }
    public override void UpdateDatabaseBeforeUpdateSchema() {
        base.UpdateDatabaseBeforeUpdateSchema();
        //if(CurrentDBVersion < new Version("1.1.0.0") && CurrentDBVersion > new Version("0.0.0.0")) {
        //    RenameColumn("DomainObject1Table", "OldColumnName", "NewColumnName");
        //}
    }
    private PermissionPolicyRole CreateAdminRole() {
        PermissionPolicyRole adminRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(r => r.Name == "Administrators");
        if(adminRole == null) {
            adminRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            adminRole.Name = "Administrators";
            adminRole.IsAdministrative = true;
        }
        return adminRole;
    }
    private PermissionPolicyRole CreateDefaultRole() {
        PermissionPolicyRole defaultRole = ObjectSpace.FirstOrDefault<PermissionPolicyRole>(role => role.Name == "Default");
        if(defaultRole == null) {
            defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
            defaultRole.Name = "Default";

			defaultRole.AddObjectPermissionFromLambda<ApplicationUser>(SecurityOperations.Read, cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddNavigationPermission(@"Application/NavigationItems/Items/Default/Items/MyDetails", SecurityPermissionState.Allow);
			defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
			defaultRole.AddMemberPermissionFromLambda<ApplicationUser>(SecurityOperations.Write, "StoredPassword", cm => cm.Oid == (Guid)CurrentUserIdOperator.CurrentUserId(), SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<PermissionPolicyRole>(SecurityOperations.Read, SecurityPermissionState.Deny);
            defaultRole.AddObjectPermission<ModelDifference>(SecurityOperations.ReadWriteAccess, "UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
            defaultRole.AddObjectPermission<ModelDifferenceAspect>(SecurityOperations.ReadWriteAccess, "Owner.UserId = ToStr(CurrentUserId())", SecurityPermissionState.Allow);
			defaultRole.AddTypePermissionsRecursively<ModelDifference>(SecurityOperations.Create, SecurityPermissionState.Allow);
            defaultRole.AddTypePermissionsRecursively<ModelDifferenceAspect>(SecurityOperations.Create, SecurityPermissionState.Allow);
            defaultRole.AddTypePermission<AuditDataItemPersistent>(SecurityOperations.Read, SecurityPermissionState.Deny);
            defaultRole.AddObjectPermissionFromLambda<AuditDataItemPersistent>(SecurityOperations.Read, a => a.UserId == CurrentUserIdOperator.CurrentUserId().ToString(), SecurityPermissionState.Allow);
            defaultRole.AddTypePermission<AuditedObjectWeakReference>(SecurityOperations.Read, SecurityPermissionState.Allow);
        }
        return defaultRole;
    }
}
