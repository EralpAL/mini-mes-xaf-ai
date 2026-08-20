namespace MiniMes.AI.Models
{
    public class ProductionOrderDto
    {
        private string code = string.Empty;
        private string stockCardName = string.Empty;
        private string routingName = string.Empty;
        private decimal plannedQuantity;
        private decimal producedQuantity;
        private decimal scrapQuantity;
        private decimal remainingQuantity;
        private decimal completionPercentage;
        private string status = string.Empty;
        private int workOrderCount;
        private int completedWorkOrderCount;
        private int inProgressWorkOrderCount;
        private int plannedWorkOrderCount;
        private int stoppedWorkOrderCount;
        private decimal productionEntryRealizedTotal;
        private decimal productionEntryScrapTotal;
        private List<WorkOrderDto> workOrders = new List<WorkOrderDto>();

        public string Code
        {
            get
            {
                return code;
            }
            set
            {
                code = value;
            }
        }

        public string StockCardName
        {
            get
            {
                return stockCardName;
            }
            set
            {
                stockCardName = value;
            }
        }

        public string RoutingName
        {
            get
            {
                return routingName;
            }
            set
            {
                routingName = value;
            }
        }

        public decimal PlannedQuantity
        {
            get
            {
                return plannedQuantity;
            }
            set
            {
                plannedQuantity = value;
            }
        }

        public decimal ProducedQuantity
        {
            get
            {
                return producedQuantity;
            }
            set
            {
                producedQuantity = value;
            }
        }

        public decimal ScrapQuantity
        {
            get
            {
                return scrapQuantity;
            }
            set
            {
                scrapQuantity = value;
            }
        }

        public decimal RemainingQuantity
        {
            get
            {
                return remainingQuantity;
            }
            set
            {
                remainingQuantity = value;
            }
        }

        public decimal CompletionPercentage
        {
            get
            {
                return completionPercentage;
            }
            set
            {
                completionPercentage = value;
            }
        }

        public string Status
        {
            get
            {
                return status;
            }
            set
            {
                status = value;
            }
        }

        public int WorkOrderCount
        {
            get
            {
                return workOrderCount;
            }
            set
            {
                workOrderCount = value;
            }
        }

        public int CompletedWorkOrderCount
        {
            get
            {
                return completedWorkOrderCount;
            }
            set
            {
                completedWorkOrderCount = value;
            }
        }

        public int InProgressWorkOrderCount
        {
            get
            {
                return inProgressWorkOrderCount;
            }
            set
            {
                inProgressWorkOrderCount = value;
            }
        }

        public int PlannedWorkOrderCount
        {
            get
            {
                return plannedWorkOrderCount;
            }
            set
            {
                plannedWorkOrderCount = value;
            }
        }

        public int StoppedWorkOrderCount
        {
            get
            {
                return stoppedWorkOrderCount;
            }
            set
            {
                stoppedWorkOrderCount = value;
            }
        }

        public decimal ProductionEntryRealizedTotal
        {
            get
            {
                return productionEntryRealizedTotal;
            }
            set
            {
                productionEntryRealizedTotal = value;
            }
        }

        public decimal ProductionEntryScrapTotal
        {
            get
            {
                return productionEntryScrapTotal;
            }
            set
            {
                productionEntryScrapTotal = value;
            }
        }

        public List<WorkOrderDto> WorkOrders
        {
            get
            {
                return workOrders;
            }
            set
            {
                if (value == null)
                {
                    workOrders = new List<WorkOrderDto>();
                    return;
                }

                workOrders = value;
            }
        }

        public void RecalculateAnalysisMetrics()
        {
            int totalWorkOrders = 0;
            int completedCount = 0;
            int inProgressCount = 0;
            int plannedCount = 0;
            int stoppedCount = 0;
            decimal realizedTotal = 0;
            decimal entryScrapTotal = 0;

            for (int i = 0; i < workOrders.Count; i++)
            {
                WorkOrderDto workOrder = workOrders[i];
                totalWorkOrders = totalWorkOrders + 1;

                if (string.Equals(workOrder.Status, "Completed", StringComparison.OrdinalIgnoreCase))
                {
                    completedCount = completedCount + 1;
                }
                else if (string.Equals(workOrder.Status, "InProgress", StringComparison.OrdinalIgnoreCase))
                {
                    inProgressCount = inProgressCount + 1;
                }
                else if (string.Equals(workOrder.Status, "Planned", StringComparison.OrdinalIgnoreCase))
                {
                    plannedCount = plannedCount + 1;
                }
                else if (string.Equals(workOrder.Status, "Stopped", StringComparison.OrdinalIgnoreCase))
                {
                    stoppedCount = stoppedCount + 1;
                }

                realizedTotal = realizedTotal + workOrder.ProductionEntryRealizedTotal;
                entryScrapTotal = entryScrapTotal + workOrder.ProductionEntryScrapTotal;
            }

            workOrderCount = totalWorkOrders;
            completedWorkOrderCount = completedCount;
            inProgressWorkOrderCount = inProgressCount;
            plannedWorkOrderCount = plannedCount;
            stoppedWorkOrderCount = stoppedCount;
            productionEntryRealizedTotal = realizedTotal;
            productionEntryScrapTotal = entryScrapTotal;
        }

        public ProductionOrderReportDto ToReport(string aiAnalysis)
        {
            ProductionOrderReportDto report = new ProductionOrderReportDto();
            report.ProductionOrderCode = Code;
            report.StockCard = StockCardName;
            report.PlannedQuantity = PlannedQuantity;
            report.ProducedQuantity = ProducedQuantity;
            report.RemainingQuantity = RemainingQuantity;
            report.CompletionPercentage = CompletionPercentage;
            report.Status = Status;
            report.WorkOrderCount = WorkOrderCount;
            report.CompletedWorkOrderCount = CompletedWorkOrderCount;
            report.InProgressWorkOrderCount = InProgressWorkOrderCount;
            report.PlannedWorkOrderCount = PlannedWorkOrderCount;
            report.StoppedWorkOrderCount = StoppedWorkOrderCount;
            report.ProductionEntryRealizedTotal = ProductionEntryRealizedTotal;
            report.ProductionEntryScrapTotal = ProductionEntryScrapTotal;
            report.ScrapTotal = ScrapQuantity;
            report.AiAnalysis = aiAnalysis ?? string.Empty;

            for (int i = 0; i < workOrders.Count; i++)
            {
                WorkOrderDto workOrder = workOrders[i];
                string step = workOrder.SequenceNumber + ". " + workOrder.Operation + " / " + workOrder.WorkStation;
                report.OperationSteps.Add(step);
            }

            return report;
        }
    }
}
