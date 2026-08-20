using MiniMes.AI.Models;

namespace MiniMes.AI.Interfaces
{
    public interface IProductionOrderTool
    {
        Task<ProductionOrderDto?> GetAsync(string code, CancellationToken cancellationToken = default);
    }
}
