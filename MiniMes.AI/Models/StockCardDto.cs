namespace MiniMes.AI.Models
{
    public class StockCardDto
    {
        private string code = string.Empty;
        private string name = string.Empty;
        private string stockType = string.Empty;
        private string warehouse = string.Empty;
        private List<string> routings = new List<string>();
        private int productionOrderCount;
        private List<ProductionOrderSummaryDto> productionOrders = new List<ProductionOrderSummaryDto>();

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

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public string StockType
        {
            get
            {
                return stockType;
            }
            set
            {
                stockType = value;
            }
        }

        public string Warehouse
        {
            get
            {
                return warehouse;
            }
            set
            {
                warehouse = value;
            }
        }

        public List<string> Routings
        {
            get
            {
                return routings;
            }
            set
            {
                if (value == null)
                {
                    routings = new List<string>();
                    return;
                }

                routings = value;
            }
        }

        public int ProductionOrderCount
        {
            get
            {
                return productionOrderCount;
            }
            set
            {
                productionOrderCount = value;
            }
        }

        public List<ProductionOrderSummaryDto> ProductionOrders
        {
            get
            {
                return productionOrders;
            }
            set
            {
                if (value == null)
                {
                    productionOrders = new List<ProductionOrderSummaryDto>();
                    return;
                }

                productionOrders = value;
            }
        }
    }
}
