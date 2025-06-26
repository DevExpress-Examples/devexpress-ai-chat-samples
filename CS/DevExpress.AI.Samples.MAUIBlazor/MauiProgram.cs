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
        string deploymentName = string.Empty;

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddDevExpressBlazor();
        builder.Services.AddDevExpressAI();

        IChatClient azureClient = new AzureOpenAIClient(
                                        new Uri(azureOpenAIEndpoint),
                                        new AzureKeyCredential(azureOpenAIKey)).GetChatClient(deploymentName).AsIChatClient();
        builder.Services.AddChatClient(azureClient);
        builder.Services.AddSingleton<ISelfEncapsulationService, DxChatEncapsulationService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        return builder.Build();
    }
}
