using Google.GenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using OllamaSharp;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
string apiKey = config["Gemini:ApiKey"] ??
    throw new InvalidOperationException("Gemini API key is not configured.");

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var client =
    app.Environment.IsDevelopment()
        ? new OllamaApiClient(new Uri("http://localhost:11434/"), "phi3:latest")
        : new Client(apiKey: apiKey).AsIChatClient("gemini-2.5-flash"); ;

IDistributedCache cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));
var cachedClient = new ChatClientBuilder(client)
    .UseDistributedCache(cache)
    .Build();

app.MapPost("/", async (Question question) =>
{
    var result = await client.GetResponseAsync(question.Prompt);
    return Results.Ok(result.Text);
});

app.MapPost("/v2", async (Question question) =>
{
    var result = await client.GetResponseAsync(
    [
        new ChatMessage(ChatRole.System, "Answer in just one setence"),
        new ChatMessage(ChatRole.User, question.Prompt),
    ]);
    return Results.Ok(result.Text);
});

app.MapPost("/v3", async (Question question) =>
{
    var result = await cachedClient.GetResponseAsync(
    [
        new ChatMessage(ChatRole.System, "Answer in just one setence"),
        new ChatMessage(ChatRole.User, question.Prompt),
    ]);
    return Results.Ok(result.Text);
});

app.Run();

public record Question(string Prompt);