using System.ClientModel;
using Azure.AI.OpenAI;
using Azure;
using DevExpress.AI.Samples.Blazor.Components;
using DevExpress.AIIntegration;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
.AddInteractiveServerComponents();

string azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string azureOpenAIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

builder.Services.AddDevExpressBlazor();
builder.Services.AddDevExpressAI((config) => {
    var azureClient = new AzureOpenAIClient(
        new Uri(azureOpenAIEndpoint),
        new ApiKeyCredential(azureOpenAIKey));
    config.RegisterChatClient(azureClient.AsChatClient("gpt4o"));
    //Reference the DevExpress.AIIntegration.OpenAI NuGet package to use Open AI Asisstants
    config.RegisterOpenAIAssistants(azureClient, "gpt4o"); 
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
