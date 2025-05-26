using DevExpress.AIIntegration;
using Microsoft.Extensions.AI;

namespace DevExpress.AI.Samples.WinBlazor {
    static class Program {
        static string AzureOpenAIEndpoint { get { return Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT"); } }
        static string AzureOpenAIKey { get { return Environment.GetEnvironmentVariable("AZURE_OPENAI_APIKEY"); } }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var azureClient = new Azure.AI.OpenAI.AzureOpenAIClient(
                    new Uri(AzureOpenAIEndpoint),
                    new System.ClientModel.ApiKeyCredential(AzureOpenAIKey)
            );
                
            IChatClient asChatClient = azureClient.GetChatClient("GPT4o").AsIChatClient();
            AIExtensionsContainerDesktop.Default.RegisterChatClient(asChatClient);
            Application.Run(new Form1());
        }
    }
}