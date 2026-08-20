namespace MiniMes.AI.Models
{
    public class WorkOrderReportDto
    {
        private string workOrderCode = string.Empty;
        private string productionOrderCode = string.Empty;
        private int sequenceNumber;
        private string status = string.Empty;
        private string operation = string.Empty;
        private string workStation = string.Empty;
        private string employee = string.Empty;
        private string shift = string.Empty;
        private decimal producedQuantity;
        private decimal scrapQuantity;
        private int productionEntryCount;
        private decimal productionEntryRealizedTotal;
        private decimal productionEntryScrapTotal;
        private int downtimeCount;
        private int openDowntimeCount;
        private double totalDowntimeMinutes;
        private string aiSummary = string.Empty;

        public string WorkOrderCode
        {
            get
            {
                return workOrderCode;
            }
            set
            {
                workOrderCode = value;
            }
        }

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

        public int SequenceNumber
        {
            get
            {
                return sequenceNumber;
            }
            set
            {
                sequenceNumber = value;
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

        public string Operation
        {
            get
            {
                return operation;
            }
            set
            {
                operation = value;
            }
        }

        public string WorkStation
        {
            get
            {
                return workStation;
            }
            set
            {
                workStation = value;
            }
        }

        public string Employee
        {
            get
            {
                return employee;
            }
            set
            {
                employee = value;
            }
        }

        public string Shift
        {
            get
            {
                return shift;
            }
            set
            {
                shift = value;
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

        public int ProductionEntryCount
        {
            get
            {
                return productionEntryCount;
            }
            set
            {
                productionEntryCount = value;
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

        public int DowntimeCount
        {
            get
            {
                return downtimeCount;
            }
            set
            {
                downtimeCount = value;
            }
        }

        public int OpenDowntimeCount
        {
            get
            {
                return openDowntimeCount;
            }
            set
            {
                openDowntimeCount = value;
            }
        }

        public double TotalDowntimeMinutes
        {
            get
            {
                return totalDowntimeMinutes;
            }
            set
            {
                totalDowntimeMinutes = value;
            }
        }

        public string AiSummary
        {
            get
            {
                return aiSummary;
            }
            set
            {
                aiSummary = value;
            }
        }
    }
}
