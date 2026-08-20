namespace MiniMes.AI.Models
{
    public class WorkOrderSummaryDto
    {
        private string code = string.Empty;
        private string productionOrderCode = string.Empty;
        private int sequenceNumber;
        private string status = string.Empty;
        private string operation = string.Empty;
        private string workStation = string.Empty;
        private decimal producedQuantity;
        private decimal scrapQuantity;

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
    }
}
