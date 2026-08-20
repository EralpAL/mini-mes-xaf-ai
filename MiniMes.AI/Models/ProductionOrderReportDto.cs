namespace MiniMes.AI.Models
{
    public class ProductionOrderReportDto
    {
        private string productionOrderCode = string.Empty;
        private string stockCard = string.Empty;
        private decimal plannedQuantity;
        private decimal producedQuantity;
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
        private decimal scrapTotal;
        private List<string> operationSteps = new List<string>();
        private string aiAnalysis = string.Empty;

        public string ProductionOrderCode
        {
            get
            {
                return productionOrderCode;
            }
            set
            {
                productionOrderCode = value;
            }
        }

        public string StockCard
        {
            get
            {
                return stockCard;
            }
            set
            {
                stockCard = value;
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

        public decimal ScrapTotal
        {
            get
            {
                return scrapTotal;
            }
            set
            {
                scrapTotal = value;
            }
        }

        public List<string> OperationSteps
        {
            get
            {
                return operationSteps;
            }
            set
            {
                if (value == null)
                {
                    operationSteps = new List<string>();
                    return;
                }

                operationSteps = value;
            }
        }

        public string AiAnalysis
        {
            get
            {
                return aiAnalysis;
            }
            set
            {
                aiAnalysis = value;
            }
        }
    }
}
