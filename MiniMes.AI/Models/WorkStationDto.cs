namespace MiniMes.AI.Models
{
    public class WorkStationDto
    {
        private string code = string.Empty;
        private string name = string.Empty;
        private bool isActive;
        private double hourlyCost;
        private int equipmentCount;
        private int activeEquipmentCount;
        private int plannedWorkOrderCount;
        private int inProgressWorkOrderCount;
        private int stoppedWorkOrderCount;
        private int completedWorkOrderCount;
        private int downtimeCount;
        private int openDowntimeCount;
        private double totalDowntimeMinutes;
        private int maintenanceCount;
        private DateTime? lastMaintenanceDate;

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

        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
            }
        }

        public double HourlyCost
        {
            get
            {
                return hourlyCost;
            }
            set
            {
                hourlyCost = value;
            }
        }

        public int EquipmentCount
        {
            get
            {
                return equipmentCount;
            }
            set
            {
                equipmentCount = value;
            }
        }

        public int ActiveEquipmentCount
        {
            get
            {
                return activeEquipmentCount;
            }
            set
            {
                activeEquipmentCount = value;
            }
        }

        public int PlannedWorkOrderCount
        {
            get
            {
                return plannedWorkOrderCount;
            }
            set
            {
                plannedWorkOrderCount = value;
            }
        }

        public int InProgressWorkOrderCount
        {
            get
            {
                return inProgressWorkOrderCount;
            }
            set
            {
                inProgressWorkOrderCount = value;
            }
        }

        public int StoppedWorkOrderCount
        {
            get
            {
                return stoppedWorkOrderCount;
            }
            set
            {
                stoppedWorkOrderCount = value;
            }
        }

        public int CompletedWorkOrderCount
        {
            get
            {
                return completedWorkOrderCount;
            }
            set
            {
                completedWorkOrderCount = value;
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

        public int MaintenanceCount
        {
            get
            {
                return maintenanceCount;
            }
            set
            {
                maintenanceCount = value;
            }
        }

        public DateTime? LastMaintenanceDate
        {
            get
            {
                return lastMaintenanceDate;
            }
            set
            {
                lastMaintenanceDate = value;
            }
        }
    }
}
