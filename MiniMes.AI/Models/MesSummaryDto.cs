namespace MiniMes.AI.Models
{
    public class MesSummaryDto
    {
        private int productionOrderCount;
        private int plannedProductionOrderCount;
        private int inProgressProductionOrderCount;
        private int completedProductionOrderCount;
        private int canceledProductionOrderCount;
        private decimal totalPlannedQuantity;
        private decimal totalProducedQuantity;
        private decimal totalScrapQuantity;
        private int workOrderCount;
        private int plannedWorkOrderCount;
        private int inProgressWorkOrderCount;
        private int stoppedWorkOrderCount;
        private int completedWorkOrderCount;
        private int workStationCount;
        private int activeWorkStationCount;
        private int openDowntimeCount;
        private int stockCardCount;
        private int employeeCount;

        public int ProductionOrderCount
        {
            get
            {
                return productionOrderCount;
            }
            set
            {
                productionOrderCount = value;
            }
        }

        public int PlannedProductionOrderCount
        {
            get
            {
                return plannedProductionOrderCount;
            }
            set
            {
                plannedProductionOrderCount = value;
            }
        }

        public int InProgressProductionOrderCount
        {
            get
            {
                return inProgressProductionOrderCount;
            }
            set
            {
                inProgressProductionOrderCount = value;
            }
        }

        public int CompletedProductionOrderCount
        {
            get
            {
                return completedProductionOrderCount;
            }
            set
            {
                completedProductionOrderCount = value;
            }
        }

        public int CanceledProductionOrderCount
        {
            get
            {
                return canceledProductionOrderCount;
            }
            set
            {
                canceledProductionOrderCount = value;
            }
        }

        public decimal TotalPlannedQuantity
        {
            get
            {
                return totalPlannedQuantity;
            }
            set
            {
                totalPlannedQuantity = value;
            }
        }

        public decimal TotalProducedQuantity
        {
            get
            {
                return totalProducedQuantity;
            }
            set
            {
                totalProducedQuantity = value;
            }
        }

        public decimal TotalScrapQuantity
        {
            get
            {
                return totalScrapQuantity;
            }
            set
            {
                totalScrapQuantity = value;
            }
        }

        public int WorkOrderCount
        {
            get
            {
                return workOrderCount;
            }
            set
            {
                workOrderCount = value;
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

        public int WorkStationCount
        {
            get
            {
                return workStationCount;
            }
            set
            {
                workStationCount = value;
            }
        }

        public int ActiveWorkStationCount
        {
            get
            {
                return activeWorkStationCount;
            }
            set
            {
                activeWorkStationCount = value;
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

        public int EmployeeCount
        {
            get
            {
                return employeeCount;
            }
            set
            {
                employeeCount = value;
            }
        }
    }
}
