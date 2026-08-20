using DevExpress.ExpressApp;
using MiniMes.AI.Interfaces;
using MiniMes.AI.Models;
using MiniMes.Module.BusinessObjects;
using System.Linq;

namespace MiniMes.Module.Services
{
    public class XafProductionOrderTool : IProductionOrderTool
    {
        private readonly IObjectSpaceFactory _objectSpaceFactory;

        public XafProductionOrderTool(IObjectSpaceFactory objectSpaceFactory)
        {
            _objectSpaceFactory = objectSpaceFactory;
        }

        public Task<ProductionOrderDto?> GetAsync(string code, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(code))
            {
                return Task.FromResult<ProductionOrderDto?>(null);
            }

            string normalizedCode = code.Trim().ToUpperInvariant();

            using (IObjectSpace objectSpace = _objectSpaceFactory.CreateObjectSpace<ProductionOrder>())
            {
                ProductionOrder productionOrder = objectSpace.GetObjectsQuery<ProductionOrder>()
                    .FirstOrDefault(item => item.Code == normalizedCode);

                if (productionOrder == null)
                {
                    return Task.FromResult<ProductionOrderDto?>(null);
                }

                ProductionOrderDto dto = new ProductionOrderDto();
                dto.Code = productionOrder.Code;
                dto.StockCardName = productionOrder.StockCard?.Name ?? string.Empty;
                dto.PlannedQuantity = Convert.ToDecimal(productionOrder.PlannedQuantity);
                dto.ProducedQuantity = Convert.ToDecimal(productionOrder.ProducedQuantity);
                dto.Status = productionOrder.Status.ToString();

                return Task.FromResult<ProductionOrderDto?>(dto);
            }
        }
    }
}
