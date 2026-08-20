using Microsoft.Extensions.AI;
using MiniMes.AI.Interfaces;
using MiniMes.AI.Models;
using MiniMes.AI.Services;
using MiniMes.AI.Tools;
using OllamaSharp;

OllamaApiClient ollamaClient = new OllamaApiClient(new Uri("http://localhost:11434"), "qwen3:1.7b");
IChatClient chatClient = new ChatClientBuilder(ollamaClient).UseFunctionInvocation().Build();
FakeProductionOrderTool productionOrderTool = new FakeProductionOrderTool();
MesAiAssistantService assistantService = new MesAiAssistantService(chatClient, productionOrderTool);

while (true)
{
    Console.Write("Sen: ");
    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        continue;
    }

    if (input == "çıkış")
    {
        break;
    }

    AssistantRequest request = new AssistantRequest();
    request.Message = input;

    AssistantResponse response = await assistantService.SendAsync(request);

    if (response.IsSuccessful)
    {
        Console.WriteLine("AI: " + response.Message);
    }
    else
    {
        Console.WriteLine("Hata: " + response.ErrorMessage);
    }

    if (!string.IsNullOrEmpty(response.UsedToolName))
    {
        Console.WriteLine("Kullanılan Tool: " + response.UsedToolName);
    }
}
