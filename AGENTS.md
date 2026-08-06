# AGENTS.md — Mini-MES (DevExpress XAF)

This document is the highest-priority instruction set for any AI agent or developer working in this repository. Read it fully before making any code change.

## 1. Project Overview

Mini-MES is a Mini Manufacturing Execution System built on DevExpress XAF. It manages stock, warehouses, work stations, routings, production orders, work orders, production entries, downtime, and maintenance tracking. An AI layer (delay analysis, scrap analysis, natural-language filtering, etc.) will be layered on top of the completed domain model.

Technology stack:
- DevExpress XAF 24.1
- XPO ORM (no Entity Framework)
- ASP.NET Core Blazor Server (UI)
- WinForms (UI)
- .NET 8 / C#

The `MiniMes.Module` project contains the shared Business Objects, enums, database updater, and controllers. It is referenced by both the Blazor and WinForms host applications, which contain no domain logic of their own.

## 2. Solution Structure

```
MiniMes.sln
├── MiniMes.Module/                  Shared XAF module (business logic layer)
│   ├── BusinessObjects/             XPO persistent classes
│   ├── Enums/                       Enumerations used by Business Objects
│   ├── Controllers/                 ViewControllers / Actions
│   ├── DatabaseUpdate/Updater.cs    ModuleUpdater (seed data, schema updates)
│   ├── Module.cs                    MiniMesModule (ModuleBase descendant)
│   └── Model.DesignedDiffs.xafml    Application Model customizations
├── MiniMes.Win/                     WinForms host application
└── MiniMes.Blazor.Server/           Blazor Server host application
```

Business Objects currently implemented in `MiniMes.Module.BusinessObjects`:

| Class | Purpose |
|---|---|
| `StockCard` | Product / material master data |
| `Warehouse` | Storage location |
| `WorkStation` | Production resource / machine center |
| `Equipments` | Physical equipment assigned to a `WorkStation` |
| `StopCause` | Reason catalog for downtime |
| `Employee` | Personnel record |
| `Shift` | Work shift definition |
| `Operation` | Reusable manufacturing operation definition |
| `Routings` | Routing header for a product (owns `RoutingDetail` steps) |
| `RoutingDetail` | A single routing step (Operation + WorkStation + sequence) |
| `ProductionOrder` | Planned production quantity for a `StockCard` |
| `WorkOrder` | One routing step's execution unit, generated from a `ProductionOrder` |
| `ProductionEntry` | Realized/scrap quantity reported against a `WorkOrder` |
| `DowntimeLog` | Downtime event on a `WorkStation` |
| `MaintenanceLog` | Maintenance activity on a `WorkStation` / `Equipments` |
| `ApplicationUser` / `ApplicationUserLoginInfo` | XAF Security module users |

Enums in `MiniMes.Module.Enums`: `StockType`, `EmployeeRole`, `StopCategory`, `ProductionOrderStatus`, `WorkOrderStatus`, `MaintenanceType`.

> Note: `Equipments` (plural) is the existing, intentional class name for the Equipment entity. `StockType` (not `EnumStockType`) is the existing enum name. Do not rename either — extend them instead.

## 3. DevExpress XAF 24.1 Architecture

Always use:
- DevExpress XAF
- XPO
- `BaseObject`
- `XPCollection<T>`
- `SetPropertyValue`
- `GetCollection<T>()`
- `Association` attributes
- `RuleRequiredField`, `RuleRange`, `RuleValueComparison`, `RuleCriteria`
- `ObjectViewController<TView, TObject>`
- `ViewController`
- `SimpleAction`
- `DetailView` / `ListView` (XAF-generated, never hand-written Razor/MVC views)

Never use:
- Entity Framework Core
- `DbContext` / `DbSet`
- EF migrations
- ASP.NET MVC Controllers
- Razor Pages
- Minimal APIs
- Generic Repository Pattern
- ASP.NET Identity (the project uses XAF's own Security module — see `ApplicationUser`)
- Plain POCO entities as replacements for existing XPO Business Objects

Never replace XPO with EF Core, in any project (Module, Win, or Blazor.Server).

## 4. XPO Conventions

- Every persistent class inherits from `BaseObject` (or, for security entities, from the appropriate `PermissionPolicy*` base class).
- Every persistent class has a `public ClassName(Session session) : base(session) { }` constructor and overrides `AfterConstruction()` (even if the override currently just calls `base.AfterConstruction();`) — this is the established pattern in this codebase and should be preserved for new classes.
- Business classes are decorated with `[DefaultClassOptions]`.
- Persistent classes are grouped into the navigation menu with `[NavigationItem("Group Name")]`. Existing groups: `"Stock and Warehouse"`, `"Production Definitions"`, `"Personnel and Shifts"`, `"Production Operations"`. Reuse an existing group before inventing a new one.
- `Code` + `Name` is the established master-data pattern (`StockCard`, `Warehouse`, `WorkStation`, `Operation`, `StopCause`, `Equipments`, `Routings`). Follow it for new master-data classes.

## 5. Business Object Conventions

Persistent properties use classic, explicit C# property syntax — a private backing field plus a full `get`/`set` block calling `SetPropertyValue`:

```csharp
private string name;

public string Name
{
    get
    {
        return name;
    }
    set
    {
        SetPropertyValue(nameof(Name), ref name, value);
    }
}
```

Rules:
- Do not use expression-bodied members, auto-properties, primary constructors, or records for persistent XPO properties.
- Always use `nameof(...)` in `SetPropertyValue` calls — never hard-coded strings.
- Collections are always `XPCollection<T>` exposed through a read-only property backed by `GetCollection<T>(nameof(Property))`.
- Use `[Indexed(Unique = true)]` for uniqueness (e.g. `Code` fields).
- Use `[Size(SizeAttribute.Unlimited)]` for long free-text fields (AI-generated text, descriptions).
- Preserve the existing coding style of whatever file you are editing (brace placement, using order) rather than reformatting it wholesale.

## 6. Association Conventions

- Every relationship uses a shared `Association("TypeA-TypeB")` string. Both the reference side and the collection side must use the exact same string.
- The reference side is a normal property using `SetPropertyValue`; the collection side is an `XPCollection<T>` using `GetCollection<T>()`.
- Association names in this codebase read `"OwnerType-CollectionPropertyName"`, e.g. `"Warehouse-StockCards"`, `"StockCard-Routings"`, `"WorkStation-MaintenanceLogs"`, `"ProductionOrder-WorkOrders"`, `"WorkOrder-ProductionEntries"`, `"Employee-ProductionEntries"`, `"StopCause-DowntimeLogs"`.
- Only declare a two-sided `Association` when a real, useful collection exists on the other side. A simple foreign-key-style reference (e.g. `WorkOrder.AssignedWorkStation`, `DowntimeLog.Operator`) does not need an invented collection on the other class just to "complete" the pair — do not add speculative collections.
- Use `[Aggregated]` only where the parent truly owns the child's lifecycle (e.g. `Routings.RoutingDetails` — deleting a routing header should delete its steps). Do not add `[Aggregated]` by default.

## 7. Validation Conventions

Prefer built-in DevExpress Validation Module attributes over hand-written validation code:

| Requirement | Attribute |
|---|---|
| Required reference or string | `[RuleRequiredField]` |
| Unique value | `[Indexed(Unique = true)]` |
| Numeric value must be `>= 0` | `[RuleRange(0.0, double.MaxValue)]` (works for `decimal`/`int` properties per DevExpress docs — the bound value only needs to implement `IComparable`) |
| One property must be `>=`/`>`/etc. another property on the same object | `[RuleValueComparison(id, DefaultContexts.Save, ValueComparisonType.GreaterThanOrEqual, "OtherProperty", ParametersMode.Expression)]` |
| Object-level cross-property/cross-reference rule | `[RuleCriteria(id, DefaultContexts.Save, "criteria string")]` at the class level |

Do not write manual `if` validation in setters or overridden `OnSaving()` when one of the above attributes can express the same rule.

## 8. Controller Conventions

- User-triggered business actions live in `MiniMes.Module/Controllers` as `ObjectViewController<DetailView, TBusinessObject>` descendants with a `SimpleAction`.
- Guard actions with `Active[...]` in `OnActivated()` based on business state (e.g. only allow "Approve" while `Status == Planned`).
- Inside `Execute`, validate business rules and `throw new DevExpress.ExpressApp.UserFriendlyException("message")` for a validation failure — XAF turns this into a friendly toast/message instead of a crash dialog.
- Use `View.ObjectSpace.CreateObject<T>()` to create related objects and `ObjectSpace.CommitChanges()` to persist. Do not open a new `UnitOfWork` unless there is no alternative.
- Show success feedback with `Application.ShowViewStrategy.ShowMessage(text, InformationType.Success)`.
- Never use ASP.NET MVC controllers, Minimal API endpoints, or Razor Pages for XAF UI actions.

## 9. Coding Style (Classic C#)

- PascalCase for classes, properties, enums, and methods.
- camelCase for private backing fields.
- `nameof(...)` instead of hard-coded property name strings.
- Full `get`/`set` blocks with explicit `return`/`SetPropertyValue` statements — no expression-bodied members.
- No records, no `dynamic`, no reflection, no unnecessary LINQ complexity in business objects.
- Minimal comments — only where the business rule is not obvious from the code itself.
- Keep Business Objects in `MiniMes.Module.BusinessObjects`, enums in `MiniMes.Module.Enums`, controllers in `MiniMes.Module.Controllers`.
- Prefer consistency with the surrounding file over introducing a new style; do not reformat unrelated code while making a targeted change.

## 10. AI Roadmap

AI features are introduced only after the core domain model (Week 1 domain + Week 2 business logic) is complete and buildable. Planned AI features:

- Production order delay analysis
- Workstation downtime and fault analysis
- Scrap and waste analysis
- Smart Summarize
- Rephrase
- Grammar correction
- Natural-language filtering
- XAF `CriteriaOperator` generation from natural language

`ProductionOrder.AiDelayAnalysis` and `ProductionOrder.AiOptimizationRecommendation` are `[Size(SizeAttribute.Unlimited)]` string fields reserved for AI-generated content. They currently have no AI logic behind them — they are populated manually or left empty until the AI layer is implemented.

## 11. Future LLM Integration

AI must never be implemented as an unrelated standalone chat window. The required flow is:

```
XPO Business Objects
   → XAF ViewController / SimpleAction
   → AI Service (plain C# class, not a Business Object)
   → Model Provider (LLM API call)
   → Structured Result
   → XPO Result Field (e.g. ProductionOrder.AiDelayAnalysis)
```

Rules:
- LLM API calls, prompt construction, and AI orchestration must live in dedicated service classes — never inside Business Objects.
- Business Objects only expose the result fields; they never call an AI service themselves.
- Trigger AI analysis through a `SimpleAction` on the relevant `DetailView`, which calls the AI service and writes the result back onto the object via `ObjectSpace`.

## 12. Development Workflow

Before making any code change:
1. Read this file (`AGENTS.md`).
2. Inspect the relevant existing files in `MiniMes.Module`.
3. Confirm whether the requested class, property, enum, relationship, or controller already exists.
4. Extend existing code instead of replacing it; keep changes minimal and XAF/XPO-compatible.
5. Do not silently introduce EF Core or plain ASP.NET patterns.
6. After changing code, build the solution (`dotnet build MiniMes.sln`) and fix only errors caused by your own change.

## 13. Best Practices / Existing Code Protection

- Do not delete existing properties, rename existing classes/properties, replace associations, or refactor working code without an explicit request.
- When a task explicitly requires correcting a genuinely inconsistent or incomplete implementation (e.g. a commented-out collection, an empty class, a wrong default status), fix it — but keep the change targeted and describe it in your summary.
- Do not change namespaces.
- Do not introduce another ORM.
- Do not add NuGet packages without a clear requirement.
- Do not change the framework stack (XAF 24.1 / XPO / Blazor Server / WinForms / .NET 8).
- Use small, descriptive commits, e.g. "Add ProductionEntry business object", "Implement production order approval action", "Add production quantity validation".

## 14. Highest Priority Instruction

For every coding task in this repository:
- Follow this file.
- Follow the existing source code and its established conventions.
- Follow DevExpress XAF and XPO conventions.
- Preserve all existing work unless a change is explicitly requested or required to fix a genuine defect.
- Never use Entity Framework Core.
- Never replace a working implementation without explicit permission.
