namespace MiniMes.AI.Models
{
    public class ProductionOrderDto
    {
        private string code = string.Empty;
        private string stockCardName = string.Empty;
        private decimal plannedQuantity;
        private decimal producedQuantity;
        private string status = string.Empty;

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
    }
}
