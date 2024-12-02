using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Azure;
using Azure.AI.OpenAI;
using DevExpress.Mvvm;
using DevExpress.AIIntegration;
using System.ClientModel;
using Microsoft.Extensions.AI;

namespace DevExpress.AI.Samples.WPFBlazor {
    class MainViewModel : BindableBase {
        readonly DxChatEncapsulationService service = new DxChatEncapsulationService();

        public MainViewModel()
        {
            InitializeCommand = new DelegateCommand(Initialize);
            SendMessageCommand = new DelegateCommand(SendMesssage, CanSendMessage);
        }

        public ServiceProvider ServiceProvider {
            get { return GetValue<ServiceProvider>(); }
            private set { SetValue(value); }
        }

        public string Message
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        public ICommand InitializeCommand { get; }

        public ICommand SendMessageCommand { get; }

        void Initialize()
        {
            var services = new ServiceCollection();

            string azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
            string azureOpenAIKey = Environment.GetEnvironmentVariable("AZURE_OPENAI_API_KEY");

            var chatClient = new AzureOpenAIClient(
                    new Uri(azureOpenAIEndpoint),
                    new ApiKeyCredential(azureOpenAIKey)).AsChatClient("gpt4o");

            services.AddWpfBlazorWebView();
            services.AddDevExpressBlazor();
            services.AddSingleton(chatClient);
            services.AddDevExpressAI((config) => {});
            services.AddSingleton<ISelfEncapsulationService>(service);

            ServiceProvider = services.BuildServiceProvider();
        }

        void SendMesssage()
        {
            service.DxChatUI?.SendMessage(Message, ChatRole.User);
        }

        bool CanSendMessage()
        {
            return !string.IsNullOrEmpty(Message);
        }
    }
}
