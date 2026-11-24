using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel.Embeddings;
using _05_PgVector.Data;
using _05_PgVector.Models;
using _05_PgVector.ViewModels;
using OllamaSharp;
using Pgvector;
using Pgvector.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString, o => o.UseVector());
});

builder.Services.AddTransient<OllamaApiClient>(x => new OllamaApiClient(
    uriString: "http://localhost:11434",
    defaultModel: "mxbai-embed-large"
));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/seed", async (
    ApplicationDbContext db,
    OllamaApiClient client) =>
{
    var products = await db.Products.ToListAsync();

    foreach (var product in products)
    {
        var textEmbeddingGenerationService = client.AsTextEmbeddingGenerationService();
        var embeddings = await textEmbeddingGenerationService.GenerateEmbeddingAsync(product.Category);

        var recomendation = new Recomendation
        {
            Title = product.Title,
            Category = product.Category,
            Embedding = new Vector(embeddings)
        };

        await db.Recomendations.AddAsync(recomendation);
        await db.SaveChangesAsync();
    }
    return Results.Ok(new { message = "Seeded" });
});

app.MapPost("/products", async (
    CreateProductViewModel model,
    ApplicationDbContext db,
    OllamaApiClient client) =>
{
    var product = new Product
    {
        Title = model.Title,
        Summary = model.Summary,
        Category = model.Category,
        Description = model.Description
    };
    await db.Products.AddAsync(product);

    var textEmbeddingGenerationService = client.AsTextEmbeddingGenerationService();
    var embeddings = await textEmbeddingGenerationService.GenerateEmbeddingAsync(model.Category);

    var recomendation = new Recomendation
    {
        Title = model.Title,
        Category = model.Category,
        Embedding = new Vector(embeddings)
    };

    await db.Recomendations.AddAsync(recomendation);
    await db.SaveChangesAsync();

    return Results.Created($"/products", null);
});

app.MapPost("/prompt", async (
    QuestionViewModel model,
    ApplicationDbContext db,
    OllamaApiClient client) =>
{
    var textEmbeddingGenerationService = client.AsTextEmbeddingGenerationService();
    var embeddings = await textEmbeddingGenerationService.GenerateEmbeddingAsync(model.Prompt);

    var recomendations = await db.Recomendations
        .OrderBy(d => d.Embedding.CosineDistance(new Vector(embeddings.ToArray())))
        .Take(3)
        .Select(x => new
        {
            x.Title,
            x.Category
        })
        .ToListAsync();

    return Results.Ok(recomendations);
});


app.Run();
