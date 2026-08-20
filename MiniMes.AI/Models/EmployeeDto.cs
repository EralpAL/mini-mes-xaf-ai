namespace MiniMes.AI.Models
{
    public class EmployeeDto
    {
        private string registrationNumber = string.Empty;
        private string fullName = string.Empty;
        private int productionEntryCount;
        private decimal realizedTotal;
        private decimal scrapTotal;
        private int assignedWorkOrderCount;
        private List<string> assignedWorkOrderCodes = new List<string>();

        public string RegistrationNumber
        {
            get
            {
                return registrationNumber;
            }
            set
            {
                registrationNumber = value;
            }
        }

        public string FullName
        {
            get
            {
                return fullName;
            }
            set
            {
                fullName = value;
            }
        }

        public int ProductionEntryCount
        {
            get
            {
                return productionEntryCount;
            }
            set
            {
                productionEntryCount = value;
            }
        }

        public decimal RealizedTotal
        {
            get
            {
                return realizedTotal;
            }
            set
            {
                realizedTotal = value;
            }
        }

        public decimal ScrapTotal
        {
            get
            {
                return scrapTotal;
            }
            set
            {
                scrapTotal = value;
            }
        }

        public int AssignedWorkOrderCount
        {
            get
            {
                return assignedWorkOrderCount;
            }
            set
            {
                assignedWorkOrderCount = value;
            }
        }

        public List<string> AssignedWorkOrderCodes
        {
            get
            {
                return assignedWorkOrderCodes;
            }
            set
            {
                if (value == null)
                {
                    assignedWorkOrderCodes = new List<string>();
                    return;
                }

                assignedWorkOrderCodes = value;
            }
        }
    }
}
