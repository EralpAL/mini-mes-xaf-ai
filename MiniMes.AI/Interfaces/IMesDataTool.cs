using MiniMes.AI.Models;

namespace MiniMes.AI.Interfaces
{
    // Read-only access to the MiniMes database for the AI assistant. Every method maps the
    // persistent objects to DTOs and limits the number of returned records.
    public interface IMesDataTool
    {
        Task<MesSummaryDto> GetSummaryAsync(CancellationToken cancellationToken = default);

        Task<List<ProductionOrderSummaryDto>> FindProductionOrdersAsync(string? status, int maxCount, CancellationToken cancellationToken = default);

        Task<List<ProductionOrderSummaryDto>> GetHighestScrapProductionOrdersAsync(int maxCount, CancellationToken cancellationToken = default);

        Task<WorkOrderDto?> GetWorkOrderAsync(string code, CancellationToken cancellationToken = default);

        Task<List<WorkOrderSummaryDto>> FindWorkOrdersAsync(string? status, int maxCount, CancellationToken cancellationToken = default);

        Task<WorkStationDto?> GetWorkStationAsync(string codeOrName, CancellationToken cancellationToken = default);

        Task<List<WorkStationDto>> FindWorkStationsAsync(bool onlyActive, int maxCount, CancellationToken cancellationToken = default);

        Task<StockCardDto?> GetStockCardAsync(string codeOrName, CancellationToken cancellationToken = default);

        Task<RoutingDto?> GetRoutingAsync(string search, CancellationToken cancellationToken = default);

        Task<List<EmployeeDto>> FindEmployeesAsync(string? search, int maxCount, CancellationToken cancellationToken = default);

        Task<List<ShiftDto>> GetShiftsAsync(CancellationToken cancellationToken = default);

        Task<List<JobRoleDto>> GetJobRolesAsync(CancellationToken cancellationToken = default);

        Task<List<DowntimeDto>> FindDowntimeLogsAsync(int lastDays, int maxCount, CancellationToken cancellationToken = default);

        Task<List<MaintenanceDto>> FindMaintenanceLogsAsync(int lastDays, int maxCount, CancellationToken cancellationToken = default);

        Task<List<EquipmentDto>> FindEquipmentAsync(string? workStation, int maxCount, CancellationToken cancellationToken = default);

        Task<List<WarehouseDto>> GetWarehousesAsync(CancellationToken cancellationToken = default);
    }
}
