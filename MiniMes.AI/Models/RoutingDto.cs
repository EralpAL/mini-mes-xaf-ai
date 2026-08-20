namespace MiniMes.AI.Models
{
    public class RoutingDto
    {
        private string code = string.Empty;
        private string name = string.Empty;
        private string stockCardName = string.Empty;
        private List<RoutingDetailDto> details = new List<RoutingDetailDto>();

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

        public List<RoutingDetailDto> Details
        {
            get
            {
                return details;
            }
            set
            {
                if (value == null)
                {
                    details = new List<RoutingDetailDto>();
                    return;
                }

                details = value;
            }
        }
    }
}
