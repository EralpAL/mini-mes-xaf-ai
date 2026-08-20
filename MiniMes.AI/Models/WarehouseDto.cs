namespace MiniMes.AI.Models
{
    public class WarehouseDto
    {
        private string code = string.Empty;
        private string name = string.Empty;
        private int stockCardCount;

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

        public int StockCardCount
        {
            get
            {
                return stockCardCount;
            }
            set
            {
                stockCardCount = value;
            }
        }
    }
}
