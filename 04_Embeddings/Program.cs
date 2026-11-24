using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Embeddings;
using OllamaSharp;

//var services = new ServiceCollection();
//var serviceProvider = services.BuildServiceProvider();

//services.AddOllamaEmbeddingGenerator(
//    modelId: "mxbai-embed-large",
//    endpoint: new Uri("http://localhost:11434"),
//    serviceId: "OllamaApiClient"
//    );

//services.AddTransient(x-> new Kernel(x));

//using var scope = serviceProvider.CreateScope();

using var client = new OllamaApiClient(
    uriString: "http://localhost:11434",
    defaultModel: "mxbai-embed-large");

var textEmbeddingGenerationService = client.AsTextEmbeddingGenerationService();
var embeddings = await textEmbeddingGenerationService.GenerateEmbeddingsAsync(
[
    "asd",
    "qwe"
]);

foreach (var item in embeddings)
{
    Console.WriteLine(item);
}

