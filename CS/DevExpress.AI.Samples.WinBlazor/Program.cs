using DevExpress.AIIntegration;
using Microsoft.Extensions.AI;

namespace DevExpress.AI.Samples.WinBlazor {
    static class Program {
        static string AzureOpenAIEndpoint {
            get {
                string endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
                return string.IsNullOrEmpty(endpoint) ? "https://api.devexpress.com/demo-openai" : endpoint;//DevExpress demo proxy-server
            }
        }
        static string AzureOpenAIKey {
            get {
                string key = Environment.GetEnvironmentVariable("AZURE_OPENAI_APIKEY");
                return string.IsNullOrEmpty(key) ? "DEMO" : key;//Demo key
            }
        }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            IChatClient asChatClient = new Azure.AI.OpenAI.AzureOpenAIClient(new Uri(AzureOpenAIEndpoint),
                    new System.ClientModel.ApiKeyCredential(AzureOpenAIKey))
                .GetChatClient("demo").AsIChatClient();
            AIExtensionsContainerDesktop.Default.RegisterChatClient(asChatClient);
            Application.Run(new Form1());
        }
    }
}