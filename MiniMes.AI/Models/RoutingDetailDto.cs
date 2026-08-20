namespace MiniMes.AI.Models
{
    public class RoutingDetailDto
    {
        private int sequenceNumber;
        private string operation = string.Empty;
        private string workStation = string.Empty;
        private double workStationHourlyCost;

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

        public double WorkStationHourlyCost
        {
            get
            {
                return workStationHourlyCost;
            }
            set
            {
                workStationHourlyCost = value;
            }
        }
    }
}
