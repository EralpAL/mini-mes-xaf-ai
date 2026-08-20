namespace MiniMes.AI.Models
{
    public class ShiftDto
    {
        private string shiftName = string.Empty;
        private string startTime = string.Empty;
        private string endTime = string.Empty;
        private bool isActive;

        public string ShiftName
        {
            get
            {
                return shiftName;
            }
            set
            {
                shiftName = value;
            }
        }

        public string StartTime
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

        public string EndTime
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
    }
}
