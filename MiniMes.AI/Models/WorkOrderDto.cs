namespace MiniMes.AI.Models
{
    public class WorkOrderDto
    {
        private string code = string.Empty;
        private string productionOrderCode = string.Empty;
        private int sequenceNumber;
        private string status = string.Empty;
        private decimal producedQuantity;
        private decimal scrapQuantity;
        private string workStation = string.Empty;
        private string operation = string.Empty;
        private string employee = string.Empty;
        private string jobRole = string.Empty;
        private string shift = string.Empty;
        private decimal productionEntryRealizedTotal;
        private decimal productionEntryScrapTotal;
        private int downtimeCount;
        private int openDowntimeCount;
        private double totalDowntimeMinutes;
        private List<ProductionEntryDto> productionEntries = new List<ProductionEntryDto>();
        private List<DowntimeDto> downtimes = new List<DowntimeDto>();

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

        public string JobRole
        {
            get
            {
                return jobRole;
            }
            set
            {
                jobRole = value;
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

        public List<ProductionEntryDto> ProductionEntries
        {
            get
            {
                return productionEntries;
            }
            set
            {
                if (value == null)
                {
                    productionEntries = new List<ProductionEntryDto>();
                    return;
                }

                productionEntries = value;
            }
        }

        public List<DowntimeDto> Downtimes
        {
            get
            {
                return downtimes;
            }
            set
            {
                if (value == null)
                {
                    downtimes = new List<DowntimeDto>();
                    return;
                }

                downtimes = value;
            }
        }

        public WorkOrderReportDto ToReport(string aiSummary)
        {
            WorkOrderReportDto report = new WorkOrderReportDto();
            report.WorkOrderCode = Code;
            report.ProductionOrderCode = ProductionOrderCode;
            report.SequenceNumber = SequenceNumber;
            report.Status = Status;
            report.Operation = Operation;
            report.WorkStation = WorkStation;
            report.Employee = Employee;
            report.Shift = Shift;
            report.ProducedQuantity = ProducedQuantity;
            report.ScrapQuantity = ScrapQuantity;
            report.ProductionEntryCount = ProductionEntries.Count;
            report.ProductionEntryRealizedTotal = ProductionEntryRealizedTotal;
            report.ProductionEntryScrapTotal = ProductionEntryScrapTotal;
            report.DowntimeCount = DowntimeCount;
            report.OpenDowntimeCount = OpenDowntimeCount;
            report.TotalDowntimeMinutes = TotalDowntimeMinutes;
            report.AiSummary = aiSummary ?? string.Empty;
            return report;
        }
    }
}
