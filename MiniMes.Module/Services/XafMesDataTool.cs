using DevExpress.ExpressApp;
using MiniMes.AI.Interfaces;
using MiniMes.AI.Models;
using MiniMes.Module.BusinessObjects;
using MiniMes.Module.Enums;
using System.Collections.Generic;
using System.Linq;

namespace MiniMes.Module.Services
{
    // Read-only MiniMes database access for the AI assistant. Every method opens its own
    // Object Space, maps the persistent objects to DTOs and never commits changes.
    public class XafMesDataTool : IMesDataTool
    {
        private const int DefaultMaxCount = 10;

        private const int HighestMaxCount = 25;

        private const int NestedRecordLimit = 10;

        private readonly IObjectSpaceFactory _objectSpaceFactory;

        public XafMesDataTool(IObjectSpaceFactory objectSpaceFactory)
        {
            _objectSpaceFactory = objectSpaceFactory;
        }

        public Task<MesSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<ProductionOrder>())
            {
                MesSummaryDto summary = new MesSummaryDto();

                List<ProductionOrder> productionOrders = objectSpace.GetObjectsQuery<ProductionOrder>().ToList();

                for (int i = 0; i < productionOrders.Count; i++)
                {
                    ProductionOrder productionOrder = productionOrders[i];

                    summary.ProductionOrderCount = summary.ProductionOrderCount + 1;
                    summary.TotalPlannedQuantity = summary.TotalPlannedQuantity + productionOrder.PlannedQuantity;
                    summary.TotalProducedQuantity = summary.TotalProducedQuantity + productionOrder.ProducedQuantity;
                    summary.TotalScrapQuantity = summary.TotalScrapQuantity + productionOrder.ScrapQuantity;

                    if (productionOrder.Status == ProductionOrderStatus.Planned)
                    {
                        summary.PlannedProductionOrderCount = summary.PlannedProductionOrderCount + 1;
                    }
                    else if (productionOrder.Status == ProductionOrderStatus.InProgress)
                    {
                        summary.InProgressProductionOrderCount = summary.InProgressProductionOrderCount + 1;
                    }
                    else if (productionOrder.Status == ProductionOrderStatus.Completed)
                    {
                        summary.CompletedProductionOrderCount = summary.CompletedProductionOrderCount + 1;
                    }
                    else if (productionOrder.Status == ProductionOrderStatus.Canceled)
                    {
                        summary.CanceledProductionOrderCount = summary.CanceledProductionOrderCount + 1;
                    }
                }

                List<WorkOrder> workOrders = objectSpace.GetObjectsQuery<WorkOrder>().ToList();

                for (int i = 0; i < workOrders.Count; i++)
                {
                    WorkOrder workOrder = workOrders[i];

                    summary.WorkOrderCount = summary.WorkOrderCount + 1;

                    if (workOrder.Status == WorkOrderStatus.Planned)
                    {
                        summary.PlannedWorkOrderCount = summary.PlannedWorkOrderCount + 1;
                    }
                    else if (workOrder.Status == WorkOrderStatus.InProgress)
                    {
                        summary.InProgressWorkOrderCount = summary.InProgressWorkOrderCount + 1;
                    }
                    else if (workOrder.Status == WorkOrderStatus.Completed)
                    {
                        summary.CompletedWorkOrderCount = summary.CompletedWorkOrderCount + 1;
                    }
                    else if (workOrder.Status == WorkOrderStatus.Stopped)
                    {
                        summary.StoppedWorkOrderCount = summary.StoppedWorkOrderCount + 1;
                    }
                }

                List<WorkStation> workStations = objectSpace.GetObjectsQuery<WorkStation>().ToList();

                for (int i = 0; i < workStations.Count; i++)
                {
                    summary.WorkStationCount = summary.WorkStationCount + 1;

                    if (workStations[i].IsActive)
                    {
                        summary.ActiveWorkStationCount = summary.ActiveWorkStationCount + 1;
                    }
                }

                summary.OpenDowntimeCount = objectSpace.GetObjectsQuery<DowntimeLog>()
                    .Count(item => item.EndTime == null);
                summary.StockCardCount = objectSpace.GetObjectsQuery<StockCard>().Count();
                summary.EmployeeCount = objectSpace.GetObjectsQuery<Employee>().Count();

                return Task.FromResult(summary);
            }
        }

        public Task<List<ProductionOrderSummaryDto>> FindProductionOrdersAsync(string status, int maxCount, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int limit = NormalizeMaxCount(maxCount);
            ProductionOrderStatus parsedStatus = ProductionOrderStatus.Planned;
            bool hasStatusFilter = TryParseProductionOrderStatus(status, out parsedStatus);

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<ProductionOrder>())
            {
                List<ProductionOrder> productionOrders = objectSpace.GetObjectsQuery<ProductionOrder>().ToList();
                List<ProductionOrderSummaryDto> result = new List<ProductionOrderSummaryDto>();

                for (int i = 0; i < productionOrders.Count; i++)
                {
                    ProductionOrder productionOrder = productionOrders[i];

                    if (hasStatusFilter && productionOrder.Status != parsedStatus)
                    {
                        continue;
                    }

                    result.Add(MapProductionOrderSummary(productionOrder));

                    if (result.Count >= limit)
                    {
                        break;
                    }
                }

                return Task.FromResult(result);
            }
        }

        public Task<List<ProductionOrderSummaryDto>> GetHighestScrapProductionOrdersAsync(int maxCount, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int limit = NormalizeMaxCount(maxCount);

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<ProductionOrder>())
            {
                List<ProductionOrder> productionOrders = objectSpace.GetObjectsQuery<ProductionOrder>()
                    .Where(item => item.ScrapQuantity > 0)
                    .ToList();

                productionOrders.Sort(CompareScrapQuantityDescending);

                List<ProductionOrderSummaryDto> result = new List<ProductionOrderSummaryDto>();

                for (int i = 0; i < productionOrders.Count && result.Count < limit; i++)
                {
                    result.Add(MapProductionOrderSummary(productionOrders[i]));
                }

                return Task.FromResult(result);
            }
        }

        public Task<WorkOrderDto> GetWorkOrderAsync(string code, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(code))
            {
                return Task.FromResult<WorkOrderDto>(null);
            }

            string normalizedCode = code.Trim().ToUpperInvariant();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<WorkOrder>())
            {
                WorkOrder workOrder = objectSpace.GetObjectsQuery<WorkOrder>()
                    .FirstOrDefault(item => item.Code == normalizedCode);

                if (workOrder == null)
                {
                    return Task.FromResult<WorkOrderDto>(null);
                }

                return Task.FromResult(MapWorkOrder(workOrder, true));
            }
        }

        public Task<List<WorkOrderSummaryDto>> FindWorkOrdersAsync(string status, int maxCount, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int limit = NormalizeMaxCount(maxCount);
            WorkOrderStatus parsedStatus = WorkOrderStatus.Planned;
            bool hasStatusFilter = TryParseWorkOrderStatus(status, out parsedStatus);

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<WorkOrder>())
            {
                List<WorkOrder> workOrders = objectSpace.GetObjectsQuery<WorkOrder>().ToList();
                List<WorkOrderSummaryDto> result = new List<WorkOrderSummaryDto>();

                for (int i = 0; i < workOrders.Count; i++)
                {
                    WorkOrder workOrder = workOrders[i];

                    if (hasStatusFilter && workOrder.Status != parsedStatus)
                    {
                        continue;
                    }

                    result.Add(MapWorkOrderSummary(workOrder));

                    if (result.Count >= limit)
                    {
                        break;
                    }
                }

                return Task.FromResult(result);
            }
        }

        public Task<WorkStationDto> GetWorkStationAsync(string codeOrName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(codeOrName))
            {
                return Task.FromResult<WorkStationDto>(null);
            }

            string search = codeOrName.Trim();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<WorkStation>())
            {
                WorkStation workStation = objectSpace.GetObjectsQuery<WorkStation>()
                    .FirstOrDefault(item => item.Code == search || item.Name == search);

                if (workStation == null)
                {
                    workStation = objectSpace.GetObjectsQuery<WorkStation>()
                        .FirstOrDefault(item => item.Name.Contains(search));
                }

                if (workStation == null)
                {
                    return Task.FromResult<WorkStationDto>(null);
                }

                return Task.FromResult(MapWorkStation(objectSpace, workStation));
            }
        }

        public Task<List<WorkStationDto>> FindWorkStationsAsync(bool onlyActive, int maxCount, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int limit = NormalizeMaxCount(maxCount);

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<WorkStation>())
            {
                List<WorkStation> workStations = objectSpace.GetObjectsQuery<WorkStation>().ToList();
                List<WorkStationDto> result = new List<WorkStationDto>();

                for (int i = 0; i < workStations.Count; i++)
                {
                    WorkStation workStation = workStations[i];

                    if (onlyActive && !workStation.IsActive)
                    {
                        continue;
                    }

                    result.Add(MapWorkStation(objectSpace, workStation));

                    if (result.Count >= limit)
                    {
                        break;
                    }
                }

                return Task.FromResult(result);
            }
        }

        public Task<StockCardDto> GetStockCardAsync(string codeOrName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(codeOrName))
            {
                return Task.FromResult<StockCardDto>(null);
            }

            string search = codeOrName.Trim();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<StockCard>())
            {
                StockCard stockCard = objectSpace.GetObjectsQuery<StockCard>()
                    .FirstOrDefault(item => item.Code == search || item.Name == search);

                if (stockCard == null)
                {
                    stockCard = objectSpace.GetObjectsQuery<StockCard>()
                        .FirstOrDefault(item => item.Name.Contains(search));
                }

                if (stockCard == null)
                {
                    return Task.FromResult<StockCardDto>(null);
                }

                StockCardDto dto = new StockCardDto();
                dto.Code = stockCard.Code ?? string.Empty;
                dto.Name = stockCard.Name ?? string.Empty;
                dto.StockType = stockCard.StockType.ToString();
                dto.Warehouse = stockCard.Warehouse == null ? string.Empty : stockCard.Warehouse.Name ?? string.Empty;

                for (int i = 0; i < stockCard.RoutingHeaders.Count && dto.Routings.Count < NestedRecordLimit; i++)
                {
                    Routings routing = stockCard.RoutingHeaders[i];

                    if (routing.IsDeleted)
                    {
                        continue;
                    }

                    dto.Routings.Add(routing.Code + " - " + routing.Name);
                }

                for (int i = 0; i < stockCard.ProductionOrders.Count; i++)
                {
                    ProductionOrder productionOrder = stockCard.ProductionOrders[i];

                    if (productionOrder.IsDeleted)
                    {
                        continue;
                    }

                    dto.ProductionOrderCount = dto.ProductionOrderCount + 1;

                    if (dto.ProductionOrders.Count < NestedRecordLimit)
                    {
                        dto.ProductionOrders.Add(MapProductionOrderSummary(productionOrder));
                    }
                }

                return Task.FromResult(dto);
            }
        }

        public Task<RoutingDto> GetRoutingAsync(string search, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(search))
            {
                return Task.FromResult<RoutingDto>(null);
            }

            string normalizedSearch = search.Trim();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<Routings>())
            {
                Routings routing = objectSpace.GetObjectsQuery<Routings>()
                    .FirstOrDefault(item => item.Code == normalizedSearch || item.Name == normalizedSearch);

                // The routing of a product is also asked by the stock card code or name.
                if (routing == null)
                {
                    routing = objectSpace.GetObjectsQuery<Routings>()
                        .FirstOrDefault(item => item.StockCard != null &&
                            (item.StockCard.Code == normalizedSearch || item.StockCard.Name == normalizedSearch));
                }

                if (routing == null)
                {
                    routing = objectSpace.GetObjectsQuery<Routings>()
                        .FirstOrDefault(item => item.Name.Contains(normalizedSearch) ||
                            (item.StockCard != null && item.StockCard.Name.Contains(normalizedSearch)));
                }

                if (routing == null)
                {
                    return Task.FromResult<RoutingDto>(null);
                }

                RoutingDto dto = new RoutingDto();
                dto.Code = routing.Code ?? string.Empty;
                dto.Name = routing.Name ?? string.Empty;
                dto.StockCardName = routing.StockCard == null ? string.Empty : routing.StockCard.Name ?? string.Empty;

                List<RoutingDetail> details = new List<RoutingDetail>();

                for (int i = 0; i < routing.RoutingDetails.Count; i++)
                {
                    RoutingDetail detail = routing.RoutingDetails[i];

                    if (detail.IsDeleted)
                    {
                        continue;
                    }

                    details.Add(detail);
                }

                details.Sort(CompareRoutingDetailSequence);

                for (int i = 0; i < details.Count; i++)
                {
                    RoutingDetail detail = details[i];

                    RoutingDetailDto detailDto = new RoutingDetailDto();
                    detailDto.SequenceNumber = detail.SequenceNumber;
                    detailDto.Operation = detail.Operation == null ? string.Empty : detail.Operation.Name ?? string.Empty;
                    detailDto.WorkStation = detail.WorkStation == null ? string.Empty : detail.WorkStation.Name ?? string.Empty;
                    detailDto.WorkStationHourlyCost = detail.WorkStation == null ? 0 : detail.WorkStation.HourlyCost;
                    dto.Details.Add(detailDto);
                }

                return Task.FromResult(dto);
            }
        }

        public Task<List<EmployeeDto>> FindEmployeesAsync(string search, int maxCount, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int limit = NormalizeMaxCount(maxCount);
            string normalizedSearch = string.IsNullOrWhiteSpace(search) ? string.Empty : search.Trim();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<Employee>())
            {
                List<Employee> employees;

                if (normalizedSearch.Length == 0)
                {
                    employees = objectSpace.GetObjectsQuery<Employee>().ToList();
                }
                else
                {
                    employees = objectSpace.GetObjectsQuery<Employee>()
                        .Where(item => item.RegistrationNumber == normalizedSearch ||
                            item.FullName == normalizedSearch ||
                            item.FullName.Contains(normalizedSearch))
                        .ToList();
                }

                List<EmployeeDto> result = new List<EmployeeDto>();

                for (int i = 0; i < employees.Count && result.Count < limit; i++)
                {
                    result.Add(MapEmployee(objectSpace, employees[i]));
                }

                return Task.FromResult(result);
            }
        }

        public Task<List<ShiftDto>> GetShiftsAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<Shift>())
            {
                List<Shift> shifts = objectSpace.GetObjectsQuery<Shift>().ToList();
                List<ShiftDto> result = new List<ShiftDto>();

                for (int i = 0; i < shifts.Count && result.Count < HighestMaxCount; i++)
                {
                    Shift shift = shifts[i];

                    ShiftDto dto = new ShiftDto();
                    dto.ShiftName = shift.ShiftName ?? string.Empty;
                    dto.StartTime = FormatTime(shift.ShiftTime);
                    dto.EndTime = FormatTime(shift.EndTime);
                    dto.IsActive = shift.IsActive;
                    result.Add(dto);
                }

                return Task.FromResult(result);
            }
        }

        public Task<List<JobRoleDto>> GetJobRolesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<JobRole>())
            {
                List<JobRole> jobRoles = objectSpace.GetObjectsQuery<JobRole>().ToList();
                List<JobRoleDto> result = new List<JobRoleDto>();

                for (int i = 0; i < jobRoles.Count && result.Count < HighestMaxCount; i++)
                {
                    JobRoleDto dto = new JobRoleDto();
                    dto.Name = jobRoles[i].Name ?? string.Empty;
                    result.Add(dto);
                }

                return Task.FromResult(result);
            }
        }

        public Task<List<DowntimeDto>> FindDowntimeLogsAsync(int lastDays, int maxCount, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int limit = NormalizeMaxCount(maxCount);

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<DowntimeLog>())
            {
                List<DowntimeLog> downtimeLogs;

                if (lastDays > 0)
                {
                    // "lastDays = 1" means the current day only.
                    DateTime startDate = DateTime.Today.AddDays(-(lastDays - 1));
                    downtimeLogs = objectSpace.GetObjectsQuery<DowntimeLog>()
                        .Where(item => item.StartTime >= startDate)
                        .ToList();
                }
                else
                {
                    downtimeLogs = objectSpace.GetObjectsQuery<DowntimeLog>().ToList();
                }

                downtimeLogs.Sort(CompareDowntimeStartDescending);

                List<DowntimeDto> result = new List<DowntimeDto>();

                for (int i = 0; i < downtimeLogs.Count && result.Count < limit; i++)
                {
                    result.Add(MapDowntime(downtimeLogs[i]));
                }

                return Task.FromResult(result);
            }
        }

        public Task<List<MaintenanceDto>> FindMaintenanceLogsAsync(int lastDays, int maxCount, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int limit = NormalizeMaxCount(maxCount);

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<MaintenanceLog>())
            {
                List<MaintenanceLog> maintenanceLogs;

                if (lastDays > 0)
                {
                    DateTime startDate = DateTime.Today.AddDays(-(lastDays - 1));
                    maintenanceLogs = objectSpace.GetObjectsQuery<MaintenanceLog>()
                        .Where(item => item.MaintenanceDate >= startDate)
                        .ToList();
                }
                else
                {
                    maintenanceLogs = objectSpace.GetObjectsQuery<MaintenanceLog>().ToList();
                }

                maintenanceLogs.Sort(CompareMaintenanceDateDescending);

                List<MaintenanceDto> result = new List<MaintenanceDto>();

                for (int i = 0; i < maintenanceLogs.Count && result.Count < limit; i++)
                {
                    MaintenanceLog maintenanceLog = maintenanceLogs[i];

                    MaintenanceDto dto = new MaintenanceDto();
                    dto.MaintenanceDate = maintenanceLog.MaintenanceDate;
                    dto.WorkStation = maintenanceLog.WorkStation == null ? string.Empty : maintenanceLog.WorkStation.Name ?? string.Empty;
                    dto.Equipment = maintenanceLog.Equipment == null ? string.Empty : maintenanceLog.Equipment.Name ?? string.Empty;
                    dto.Employee = maintenanceLog.Employee == null ? string.Empty : maintenanceLog.Employee.FullName ?? string.Empty;
                    dto.MaintenanceType = maintenanceLog.MaintenanceType.ToString();
                    dto.Cost = maintenanceLog.Cost;
                    result.Add(dto);
                }

                return Task.FromResult(result);
            }
        }

        public Task<List<EquipmentDto>> FindEquipmentAsync(string workStation, int maxCount, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int limit = NormalizeMaxCount(maxCount);
            string normalizedSearch = string.IsNullOrWhiteSpace(workStation) ? string.Empty : workStation.Trim();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<Equipment>())
            {
                List<Equipment> equipments;

                if (normalizedSearch.Length == 0)
                {
                    equipments = objectSpace.GetObjectsQuery<Equipment>().ToList();
                }
                else
                {
                    equipments = objectSpace.GetObjectsQuery<Equipment>()
                        .Where(item => item.WorkStation != null &&
                            (item.WorkStation.Code == normalizedSearch ||
                            item.WorkStation.Name == normalizedSearch ||
                            item.WorkStation.Name.Contains(normalizedSearch)))
                        .ToList();
                }

                List<EquipmentDto> result = new List<EquipmentDto>();

                for (int i = 0; i < equipments.Count && result.Count < limit; i++)
                {
                    Equipment equipment = equipments[i];

                    EquipmentDto dto = new EquipmentDto();
                    dto.Code = equipment.Code ?? string.Empty;
                    dto.Name = equipment.Name ?? string.Empty;
                    dto.IsActive = equipment.IsActive;
                    dto.WorkStation = equipment.WorkStation == null ? string.Empty : equipment.WorkStation.Name ?? string.Empty;
                    result.Add(dto);
                }

                return Task.FromResult(result);
            }
        }

        public Task<List<WarehouseDto>> GetWarehousesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<Warehouse>())
            {
                List<Warehouse> warehouses = objectSpace.GetObjectsQuery<Warehouse>().ToList();
                List<WarehouseDto> result = new List<WarehouseDto>();

                for (int i = 0; i < warehouses.Count && result.Count < HighestMaxCount; i++)
                {
                    Warehouse warehouse = warehouses[i];

                    WarehouseDto dto = new WarehouseDto();
                    dto.Code = warehouse.Code ?? string.Empty;
                    dto.Name = warehouse.Name ?? string.Empty;
                    dto.StockCardCount = warehouse.StockCards.Count;
                    result.Add(dto);
                }

                return Task.FromResult(result);
            }
        }

        private ProductionOrderSummaryDto MapProductionOrderSummary(ProductionOrder productionOrder)
        {
            ProductionOrderSummaryDto dto = new ProductionOrderSummaryDto();
            dto.Code = productionOrder.Code ?? string.Empty;
            dto.StockCardName = productionOrder.StockCard == null ? string.Empty : productionOrder.StockCard.Name ?? string.Empty;
            dto.Status = productionOrder.Status.ToString();
            dto.PlannedQuantity = Convert.ToDecimal(productionOrder.PlannedQuantity);
            dto.ProducedQuantity = Convert.ToDecimal(productionOrder.ProducedQuantity);
            dto.ScrapQuantity = Convert.ToDecimal(productionOrder.ScrapQuantity);
            dto.CompletionPercentage = Convert.ToDecimal(productionOrder.CompletionPercentage);
            return dto;
        }

        private WorkOrderSummaryDto MapWorkOrderSummary(WorkOrder workOrder)
        {
            WorkOrderSummaryDto dto = new WorkOrderSummaryDto();
            dto.Code = workOrder.Code ?? string.Empty;
            dto.ProductionOrderCode = workOrder.ProductionOrder == null ? string.Empty : workOrder.ProductionOrder.Code ?? string.Empty;
            dto.SequenceNumber = workOrder.SequenceNumber;
            dto.Status = workOrder.Status.ToString();
            dto.Operation = workOrder.Operation == null ? string.Empty : workOrder.Operation.Name ?? string.Empty;
            dto.WorkStation = workOrder.AssignedWorkStation == null ? string.Empty : workOrder.AssignedWorkStation.Name ?? string.Empty;
            dto.ProducedQuantity = Convert.ToDecimal(workOrder.ProducedQuantity);
            dto.ScrapQuantity = Convert.ToDecimal(workOrder.ScrapQuantity);
            return dto;
        }

        private WorkOrderDto MapWorkOrder(WorkOrder workOrder, bool includeDowntimeDetails)
        {
            WorkOrderDto dto = new WorkOrderDto();
            dto.Code = workOrder.Code ?? string.Empty;
            dto.ProductionOrderCode = workOrder.ProductionOrder == null ? string.Empty : workOrder.ProductionOrder.Code ?? string.Empty;
            dto.SequenceNumber = workOrder.SequenceNumber;
            dto.Status = workOrder.Status.ToString();
            dto.ProducedQuantity = Convert.ToDecimal(workOrder.ProducedQuantity);
            dto.ScrapQuantity = Convert.ToDecimal(workOrder.ScrapQuantity);
            dto.WorkStation = workOrder.AssignedWorkStation == null ? string.Empty : workOrder.AssignedWorkStation.Name ?? string.Empty;
            dto.Operation = workOrder.Operation == null ? string.Empty : workOrder.Operation.Name ?? string.Empty;
            dto.Employee = workOrder.AssignedEmployee == null ? string.Empty : workOrder.AssignedEmployee.FullName ?? string.Empty;
            dto.JobRole = workOrder.AssignedRole == null ? string.Empty : workOrder.AssignedRole.Name ?? string.Empty;
            dto.Shift = workOrder.AssignedShift == null ? string.Empty : workOrder.AssignedShift.ShiftName ?? string.Empty;

            decimal realizedTotal = 0;
            decimal entryScrapTotal = 0;

            for (int i = 0; i < workOrder.ProductionEntries.Count; i++)
            {
                ProductionEntry productionEntry = workOrder.ProductionEntries[i];

                if (productionEntry.IsDeleted)
                {
                    continue;
                }

                realizedTotal = realizedTotal + Convert.ToDecimal(productionEntry.RealizedAmount);
                entryScrapTotal = entryScrapTotal + Convert.ToDecimal(productionEntry.ScrapAmount);

                if (dto.ProductionEntries.Count < NestedRecordLimit)
                {
                    dto.ProductionEntries.Add(MapProductionEntry(productionEntry));
                }
            }

            dto.ProductionEntryRealizedTotal = realizedTotal;
            dto.ProductionEntryScrapTotal = entryScrapTotal;

            for (int i = 0; i < workOrder.DowntimeLogs.Count; i++)
            {
                DowntimeLog downtimeLog = workOrder.DowntimeLogs[i];

                if (downtimeLog.IsDeleted)
                {
                    continue;
                }

                dto.DowntimeCount = dto.DowntimeCount + 1;
                dto.TotalDowntimeMinutes = dto.TotalDowntimeMinutes + downtimeLog.DurationMinutes;

                if (!downtimeLog.EndTime.HasValue)
                {
                    dto.OpenDowntimeCount = dto.OpenDowntimeCount + 1;
                }

                if (includeDowntimeDetails && dto.Downtimes.Count < NestedRecordLimit)
                {
                    dto.Downtimes.Add(MapDowntime(downtimeLog));
                }
            }

            return dto;
        }

        private ProductionEntryDto MapProductionEntry(ProductionEntry productionEntry)
        {
            ProductionEntryDto dto = new ProductionEntryDto();
            dto.RealizedAmount = Convert.ToDecimal(productionEntry.RealizedAmount);
            dto.ScrapAmount = Convert.ToDecimal(productionEntry.ScrapAmount);
            dto.Operator = productionEntry.Operator == null ? string.Empty : productionEntry.Operator.FullName ?? string.Empty;
            dto.WorkStation = productionEntry.WorkStation == null ? string.Empty : productionEntry.WorkStation.Name ?? string.Empty;
            return dto;
        }

        private DowntimeDto MapDowntime(DowntimeLog downtimeLog)
        {
            DowntimeDto dto = new DowntimeDto();
            dto.StartTime = downtimeLog.StartTime;
            dto.EndTime = downtimeLog.EndTime;
            dto.DurationMinutes = downtimeLog.DurationMinutes;
            dto.IsOpen = !downtimeLog.EndTime.HasValue;
            dto.WorkStation = downtimeLog.WorkStation == null ? string.Empty : downtimeLog.WorkStation.Name ?? string.Empty;
            dto.WorkOrderCode = downtimeLog.WorkOrder == null ? string.Empty : downtimeLog.WorkOrder.Code ?? string.Empty;
            dto.StopCause = downtimeLog.StopCause == null ? string.Empty : downtimeLog.StopCause.Name ?? string.Empty;
            dto.StopCategory = downtimeLog.StopCause == null ? string.Empty : downtimeLog.StopCause.Category.ToString();
            dto.Operator = downtimeLog.Operator == null ? string.Empty : downtimeLog.Operator.FullName ?? string.Empty;
            dto.Description = downtimeLog.Description ?? string.Empty;
            return dto;
        }

        private WorkStationDto MapWorkStation(IObjectSpace objectSpace, WorkStation workStation)
        {
            WorkStationDto dto = new WorkStationDto();
            dto.Code = workStation.Code ?? string.Empty;
            dto.Name = workStation.Name ?? string.Empty;
            dto.IsActive = workStation.IsActive;
            dto.HourlyCost = workStation.HourlyCost;

            for (int i = 0; i < workStation.Equipments.Count; i++)
            {
                Equipment equipment = workStation.Equipments[i];

                if (equipment.IsDeleted)
                {
                    continue;
                }

                dto.EquipmentCount = dto.EquipmentCount + 1;

                if (equipment.IsActive)
                {
                    dto.ActiveEquipmentCount = dto.ActiveEquipmentCount + 1;
                }
            }

            for (int i = 0; i < workStation.Downtime.Count; i++)
            {
                DowntimeLog downtimeLog = workStation.Downtime[i];

                if (downtimeLog.IsDeleted)
                {
                    continue;
                }

                dto.DowntimeCount = dto.DowntimeCount + 1;
                dto.TotalDowntimeMinutes = dto.TotalDowntimeMinutes + downtimeLog.DurationMinutes;

                if (!downtimeLog.EndTime.HasValue)
                {
                    dto.OpenDowntimeCount = dto.OpenDowntimeCount + 1;
                }
            }

            for (int i = 0; i < workStation.MaintenanceLogs.Count; i++)
            {
                MaintenanceLog maintenanceLog = workStation.MaintenanceLogs[i];

                if (maintenanceLog.IsDeleted)
                {
                    continue;
                }

                dto.MaintenanceCount = dto.MaintenanceCount + 1;

                if (!dto.LastMaintenanceDate.HasValue || maintenanceLog.MaintenanceDate > dto.LastMaintenanceDate.Value)
                {
                    dto.LastMaintenanceDate = maintenanceLog.MaintenanceDate;
                }
            }

            // WorkStation has no work order collection, so the work orders are read through the
            // AssignedWorkStation reference of WorkOrder.
            List<WorkOrder> workOrders = objectSpace.GetObjectsQuery<WorkOrder>()
                .Where(item => item.AssignedWorkStation == workStation)
                .ToList();

            for (int i = 0; i < workOrders.Count; i++)
            {
                WorkOrder workOrder = workOrders[i];

                if (workOrder.Status == WorkOrderStatus.Planned)
                {
                    dto.PlannedWorkOrderCount = dto.PlannedWorkOrderCount + 1;
                }
                else if (workOrder.Status == WorkOrderStatus.InProgress)
                {
                    dto.InProgressWorkOrderCount = dto.InProgressWorkOrderCount + 1;
                }
                else if (workOrder.Status == WorkOrderStatus.Completed)
                {
                    dto.CompletedWorkOrderCount = dto.CompletedWorkOrderCount + 1;
                }
                else if (workOrder.Status == WorkOrderStatus.Stopped)
                {
                    dto.StoppedWorkOrderCount = dto.StoppedWorkOrderCount + 1;
                }
            }

            return dto;
        }

        private EmployeeDto MapEmployee(IObjectSpace objectSpace, Employee employee)
        {
            EmployeeDto dto = new EmployeeDto();
            dto.RegistrationNumber = employee.RegistrationNumber ?? string.Empty;
            dto.FullName = employee.FullName ?? string.Empty;

            for (int i = 0; i < employee.ProductionEntries.Count; i++)
            {
                ProductionEntry productionEntry = employee.ProductionEntries[i];

                if (productionEntry.IsDeleted)
                {
                    continue;
                }

                dto.ProductionEntryCount = dto.ProductionEntryCount + 1;
                dto.RealizedTotal = dto.RealizedTotal + Convert.ToDecimal(productionEntry.RealizedAmount);
                dto.ScrapTotal = dto.ScrapTotal + Convert.ToDecimal(productionEntry.ScrapAmount);
            }

            // Employee has no work order collection, so the assignments are read through the
            // AssignedEmployee reference of WorkOrder.
            List<WorkOrder> workOrders = objectSpace.GetObjectsQuery<WorkOrder>()
                .Where(item => item.AssignedEmployee == employee)
                .ToList();

            for (int i = 0; i < workOrders.Count; i++)
            {
                dto.AssignedWorkOrderCount = dto.AssignedWorkOrderCount + 1;

                if (dto.AssignedWorkOrderCodes.Count < NestedRecordLimit)
                {
                    dto.AssignedWorkOrderCodes.Add(workOrders[i].Code ?? string.Empty);
                }
            }

            return dto;
        }

        private int NormalizeMaxCount(int maxCount)
        {
            if (maxCount <= 0)
            {
                return DefaultMaxCount;
            }

            if (maxCount > HighestMaxCount)
            {
                return HighestMaxCount;
            }

            return maxCount;
        }

        private string FormatTime(TimeSpan value)
        {
            return value.Hours.ToString("00") + ":" + value.Minutes.ToString("00");
        }

        private bool TryParseProductionOrderStatus(string status, out ProductionOrderStatus result)
        {
            result = ProductionOrderStatus.Planned;

            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            string normalized = status.Trim().ToLowerInvariant();

            if (normalized.Contains("planl") || normalized == "planned")
            {
                result = ProductionOrderStatus.Planned;
                return true;
            }

            if (normalized.Contains("devam") || normalized == "inprogress")
            {
                result = ProductionOrderStatus.InProgress;
                return true;
            }

            if (normalized.Contains("tamam") || normalized == "completed")
            {
                result = ProductionOrderStatus.Completed;
                return true;
            }

            if (normalized.Contains("iptal") || normalized == "canceled" || normalized == "cancelled")
            {
                result = ProductionOrderStatus.Canceled;
                return true;
            }

            return false;
        }

        private bool TryParseWorkOrderStatus(string status, out WorkOrderStatus result)
        {
            result = WorkOrderStatus.Planned;

            if (string.IsNullOrWhiteSpace(status))
            {
                return false;
            }

            string normalized = status.Trim().ToLowerInvariant();

            if (normalized.Contains("planl") || normalized == "planned")
            {
                result = WorkOrderStatus.Planned;
                return true;
            }

            if (normalized.Contains("devam") || normalized == "inprogress")
            {
                result = WorkOrderStatus.InProgress;
                return true;
            }

            if (normalized.Contains("tamam") || normalized == "completed")
            {
                result = WorkOrderStatus.Completed;
                return true;
            }

            if (normalized.Contains("duru") || normalized.Contains("durdur") || normalized == "stopped")
            {
                result = WorkOrderStatus.Stopped;
                return true;
            }

            return false;
        }

        private int CompareScrapQuantityDescending(ProductionOrder left, ProductionOrder right)
        {
            return right.ScrapQuantity.CompareTo(left.ScrapQuantity);
        }

        private int CompareRoutingDetailSequence(RoutingDetail left, RoutingDetail right)
        {
            return left.SequenceNumber.CompareTo(right.SequenceNumber);
        }

        private int CompareDowntimeStartDescending(DowntimeLog left, DowntimeLog right)
        {
            return right.StartTime.CompareTo(left.StartTime);
        }

        private int CompareMaintenanceDateDescending(MaintenanceLog left, MaintenanceLog right)
        {
            return right.MaintenanceDate.CompareTo(left.MaintenanceDate);
        }
    }
}
