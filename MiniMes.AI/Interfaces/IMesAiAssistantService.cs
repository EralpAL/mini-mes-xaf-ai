using MiniMes.AI.Models;

namespace MiniMes.AI.Interfaces
{
    public interface IMesAiAssistantService
    {
        Task<AssistantResponse> SendAsync(AssistantRequest request, CancellationToken cancellationToken = default);
    }
}
