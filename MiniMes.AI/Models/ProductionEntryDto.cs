namespace MiniMes.AI.Models
{
    public class ProductionEntryDto
    {
        private decimal realizedAmount;
        private decimal scrapAmount;
        private string operatorName = string.Empty;
        private string workStation = string.Empty;

        public decimal RealizedAmount
        {
            get
            {
                return realizedAmount;
            }
            set
            {
                realizedAmount = value;
            }
        }

        public decimal ScrapAmount
        {
            get
            {
                return scrapAmount;
            }
            set
            {
                scrapAmount = value;
            }
        }

        public string Operator
        {
            get
            {
                return operatorName;
            }
            set
            {
                operatorName = value;
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
    }
}
