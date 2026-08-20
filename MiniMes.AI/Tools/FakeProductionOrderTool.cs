using MiniMes.AI.Interfaces;
using MiniMes.AI.Models;

namespace MiniMes.AI.Tools
{
    public class FakeProductionOrderTool : IProductionOrderTool
    {
        public Task<ProductionOrderDto?> GetAsync(string code, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrEmpty(code))
            {
                return Task.FromResult<ProductionOrderDto?>(null);
            }

            string normalizedCode = code.Trim().ToUpperInvariant();

            if (normalizedCode == "PO-107")
            {
                ProductionOrderDto productionOrder = new ProductionOrderDto();
                productionOrder.Code = "PO-107";
                productionOrder.StockCardName = "Metal Gövde";
                productionOrder.PlannedQuantity = 100;
                productionOrder.ProducedQuantity = 80;
                productionOrder.Status = "InProgress";
                return Task.FromResult<ProductionOrderDto?>(productionOrder);
            }

            return Task.FromResult<ProductionOrderDto?>(null);
        }
    }
}
