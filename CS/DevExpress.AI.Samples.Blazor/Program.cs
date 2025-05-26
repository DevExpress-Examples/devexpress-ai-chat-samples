using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using DevExpress.AI.Samples.Blazor.Components;
using DevExpress.AIIntegration;
using ReportingApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
.AddInteractiveServerComponents();

string azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string azureOpenAIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
string deploymentName = string.Empty;

var azureClient = new AzureOpenAIClient(
    new Uri(azureOpenAIEndpoint),
    new AzureKeyCredential(azureOpenAIKey));

IChatClient chatClient = azureClient.GetChatClient(deploymentName).AsIChatClient();

var assistantCreator = new AIAssistantCreator(azureClient, deploymentName);

builder.Services.AddDevExpressBlazor();
builder.Services.AddChatClient(chatClient);
builder.Services.AddSingleton(assistantCreator);
builder.Services.AddDevExpressAI(config => {
    config.RegisterOpenAIAssistants(azureClient, deploymentName);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
