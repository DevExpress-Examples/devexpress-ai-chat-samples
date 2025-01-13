using System.ClientModel;
using Azure;
using Azure.AI.OpenAI;
using DevExpress.Maui;
using DevExpress.Maui.Core;
using Microsoft.Extensions.AI;

namespace DevExpress.AI.Samples.MAUIBlazor;

public static class MauiProgram {
    public static MauiApp CreateMauiApp() {
        ThemeManager.ApplyThemeToSystemBars = true;
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseDevExpress()
            .UseDevExpressCollectionView()
            .UseDevExpressControls()
            .UseDevExpressEditors()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        string azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        string azureOpenAIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddDevExpressBlazor();
        builder.Services.AddDevExpressAI();
        builder.Services.AddChatClient(
            new AzureOpenAIClient(
                new Uri(azureOpenAIEndpoint), 
                new ApiKeyCredential(azureOpenAIKey)
            ).AsChatClient("gpt4o")
        );
        builder.Services.AddSingleton<ISelfEncapsulationService, DxChatEncapsulationService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        return builder.Build();
    }
}