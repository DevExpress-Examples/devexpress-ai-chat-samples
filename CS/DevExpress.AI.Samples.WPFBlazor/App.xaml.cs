using System;
using System.Windows;
using DevExpress.AIIntegration;
using Azure.AI.OpenAI;
using DevExpress.Xpf.Core;
using Microsoft.Extensions.AI;
using DevExpress.Data.Utils;

namespace WPF_AIChatControl {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        static App() {
            CompatibilitySettings.UseLightweightThemes = true;
            ApplicationThemeHelper.ApplicationThemeName = Theme.Win11Light.Name;
                
            SetupAzureOpenAI();
        }
        static void SetupAzureOpenAI() {

            string azureOpenAIEndpoint = SafeEnvironment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            if (string.IsNullOrEmpty(azureOpenAIEndpoint))
                azureOpenAIEndpoint = "https://api.devexpress.com/demo-openai";//DevExpress demo proxy-server
            string azureOpenAIKey = SafeEnvironment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
            if (string.IsNullOrEmpty(azureOpenAIKey))
                azureOpenAIKey = "DEMO";//Demo key
            string deployment = "demo";

            IChatClient asChatClient = new AzureOpenAIClient(new Uri(azureOpenAIEndpoint),
                    new System.ClientModel.ApiKeyCredential(azureOpenAIKey))
                .GetChatClient(deployment).AsIChatClient();
            AIExtensionsContainerDesktop.Default.RegisterChatClient(asChatClient);
        }
    }

}
