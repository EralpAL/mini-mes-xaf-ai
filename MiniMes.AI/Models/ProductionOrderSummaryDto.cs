namespace MiniMes.AI.Models
{
    public class ProductionOrderSummaryDto
    {
        private string code = string.Empty;
        private string stockCardName = string.Empty;
        private string status = string.Empty;
        private decimal plannedQuantity;
        private decimal producedQuantity;
        private decimal scrapQuantity;
        private decimal completionPercentage;

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
    }
}
