using _03_SemanticKernel.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;

var builder = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(
        modelId: "llama3.1:latest",
        endpoint: new Uri("http://localhost:11434")
    );

builder.Services.AddLogging(x => 
x.AddConsole()
    .SetMinimumLevel(LogLevel.Trace));

var kernel = builder.Build();
var chatCompleteService = kernel.GetRequiredService<IChatCompletionService>();

kernel.Plugins.AddFromType<ProductsPlugin>("Products");

OllamaPromptExecutionSettings settings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

var history = new ChatHistory();
string? prompt;

do
{
    Console.Write("User: ");
    prompt = Console.ReadLine();

    history.AddUserMessage(prompt);

    var result = await chatCompleteService
        .GetChatMessageContentAsync(
            history,
            executionSettings: settings,
            kernel: kernel);

    Console.WriteLine("Assistant: " + result);
    history.AddMessage(result.Role, result.Content ?? string.Empty);

} while (prompt is not null);