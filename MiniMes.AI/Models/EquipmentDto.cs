namespace MiniMes.AI.Models
{
    public class EquipmentDto
    {
        private string code = string.Empty;
        private string name = string.Empty;
        private bool isActive;
        private string workStation = string.Empty;

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
