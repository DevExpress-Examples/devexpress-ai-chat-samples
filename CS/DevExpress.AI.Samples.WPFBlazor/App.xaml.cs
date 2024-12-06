using System;
using System.Windows;
using DevExpress.AIIntegration;
using Azure.AI.OpenAI;
using DevExpress.Data.Utils;
using DevExpress.Xpf.Core;
using Microsoft.Extensions.AI;

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
            string azureOpenAIKey = SafeEnvironment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");
            string deployment = "gpt-4o-mini";

            var openAIClient = new AzureOpenAIClient(
                new Uri(azureOpenAIEndpoint),
                new System.ClientModel.ApiKeyCredential(azureOpenAIKey)
            );
            var container = AIExtensionsContainerDesktop.Default;
            container.RegisterChatClient(openAIClient.AsChatClient(deployment));
            container.RegisterOpenAIAssistants(openAIClient, deployment);
        }
    }

}
