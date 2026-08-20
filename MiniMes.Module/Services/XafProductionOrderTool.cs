using DevExpress.ExpressApp;
using MiniMes.AI.Interfaces;
using MiniMes.AI.Models;
using MiniMes.Module.BusinessObjects;
using System.Collections.Generic;
using System.Linq;

namespace MiniMes.Module.Services
{
    public class XafProductionOrderTool : IProductionOrderTool
    {
        private const int NestedRecordLimit = 10;

        private readonly IObjectSpaceFactory _objectSpaceFactory;

        public XafProductionOrderTool(IObjectSpaceFactory objectSpaceFactory)
        {
            _objectSpaceFactory = objectSpaceFactory;
        }

        public Task<ProductionOrderDto?> GetAsync(string code, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(code))
            {
                return Task.FromResult<ProductionOrderDto?>(null);
            }

            string normalizedCode = code.Trim().ToUpperInvariant();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<ProductionOrder>())
            {
                ProductionOrder productionOrder = objectSpace.GetObjectsQuery<ProductionOrder>()
                    .FirstOrDefault(item => item.Code == normalizedCode);

                if (productionOrder == null)
                {
                    return Task.FromResult<ProductionOrderDto?>(null);
                }

                ProductionOrderDto dto = MapProductionOrder(productionOrder);
                return Task.FromResult<ProductionOrderDto?>(dto);
            }
        }

        private ProductionOrderDto MapProductionOrder(ProductionOrder productionOrder)
        {
            ProductionOrderDto dto = new ProductionOrderDto();
            dto.Code = productionOrder.Code ?? string.Empty;
            dto.StockCardName = productionOrder.StockCard == null ? string.Empty : productionOrder.StockCard.Name ?? string.Empty;
            dto.RoutingName = productionOrder.Routing == null ? string.Empty : productionOrder.Routing.Name ?? string.Empty;
            dto.PlannedQuantity = Convert.ToDecimal(productionOrder.PlannedQuantity);
            dto.ProducedQuantity = Convert.ToDecimal(productionOrder.ProducedQuantity);
            dto.ScrapQuantity = Convert.ToDecimal(productionOrder.ScrapQuantity);
            dto.RemainingQuantity = Convert.ToDecimal(productionOrder.RemainingQuantity);
            dto.CompletionPercentage = Convert.ToDecimal(productionOrder.CompletionPercentage);
            dto.Status = productionOrder.Status.ToString();

            List<WorkOrder> orderedWorkOrders = new List<WorkOrder>();
            for (int i = 0; i < productionOrder.WorkOrders.Count; i++)
            {
                WorkOrder workOrder = productionOrder.WorkOrders[i];
                if (workOrder.IsDeleted)
                {
                    continue;
                }

                orderedWorkOrders.Add(workOrder);
            }

            orderedWorkOrders.Sort(CompareWorkOrderSequence);

            for (int i = 0; i < orderedWorkOrders.Count; i++)
            {
                dto.WorkOrders.Add(MapWorkOrder(orderedWorkOrders[i]));
            }

            dto.RecalculateAnalysisMetrics();
            return dto;
        }

        private WorkOrderDto MapWorkOrder(WorkOrder workOrder)
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

        private int CompareWorkOrderSequence(WorkOrder left, WorkOrder right)
        {
            return left.SequenceNumber.CompareTo(right.SequenceNumber);
        }
    }
}
