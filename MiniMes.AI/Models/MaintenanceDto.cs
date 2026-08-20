namespace MiniMes.AI.Models
{
    public class MaintenanceDto
    {
        private DateTime maintenanceDate;
        private string workStation = string.Empty;
        private string equipment = string.Empty;
        private string employee = string.Empty;
        private string maintenanceType = string.Empty;
        private double cost;

        public DateTime MaintenanceDate
        {
            get
            {
                return maintenanceDate;
            }
            set
            {
                maintenanceDate = value;
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

        public string Equipment
        {
            get
            {
                return equipment;
            }
            set
            {
                equipment = value;
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

        public string MaintenanceType
        {
            get
            {
                return maintenanceType;
            }
            set
            {
                maintenanceType = value;
            }
        }

        public double Cost
        {
            get
            {
                return cost;
            }
            set
            {
                cost = value;
            }
        }
    }
}
