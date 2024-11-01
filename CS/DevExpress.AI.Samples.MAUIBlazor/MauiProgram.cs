using System.ClientModel;
using Azure;
using Azure.AI.OpenAI;
using DevExpress.Maui;
using DevExpress.Maui.Core;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

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

        string azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!;
        string azureOpenAIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY")!;

        var chatClient = new AzureOpenAIClient(
            new Uri(azureOpenAIEndpoint),
            new ApiKeyCredential(azureOpenAIKey)).AsChatClient("gpt4o");
        
        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddDevExpressBlazor();
        builder.Services.AddDevExpressAI((config) => {
            config.RegisterChatClient(chatClient);
        });
        builder.Services.AddSingleton<ISelfEncapsulationService, DxChatEncapsulationService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}