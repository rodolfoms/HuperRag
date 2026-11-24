using OllamaSharp;

var uri = new Uri("http://localhost:11434");
var client = new OllamaApiClient(uri);

var models = await client.ListLocalModelsAsync();
foreach (var model in models)
{
    Console.WriteLine(model.Name);
}

client.SelectedModel = "phi3:latest";

var chat = new Chat(client);

var prompt = "Answer in just one setence";
prompt += Environment.NewLine;
prompt += Console.ReadLine();

await foreach (var answer in chat.SendAsync(prompt))
    Console.Write(answer);

Console.WriteLine();