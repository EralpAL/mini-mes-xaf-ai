namespace MiniMes.AI.Models
{
    public class DowntimeDto
    {
        private DateTime startTime;
        private DateTime? endTime;
        private double durationMinutes;
        private bool isOpen;
        private string workStation = string.Empty;
        private string workOrderCode = string.Empty;
        private string stopCause = string.Empty;
        private string stopCategory = string.Empty;
        private string operatorName = string.Empty;
        private string description = string.Empty;

        public DateTime StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                startTime = value;
            }
        }

        public DateTime? EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                endTime = value;
            }
        }

        public double DurationMinutes
        {
            get
            {
                return durationMinutes;
            }
            set
            {
                durationMinutes = value;
            }
        }

        public bool IsOpen
        {
            get
            {
                return isOpen;
            }
            set
            {
                isOpen = value;
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

        public string WorkOrderCode
        {
            get
            {
                return workOrderCode;
            }
            set
            {
                workOrderCode = value;
            }
        }

        public string StopCause
        {
            get
            {
                return stopCause;
            }
            set
            {
                stopCause = value;
            }
        }

        public string StopCategory
        {
            get
            {
                return stopCategory;
            }
            set
            {
                stopCategory = value;
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

        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
    }
}
