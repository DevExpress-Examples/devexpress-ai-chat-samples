using DevExpress.AIIntegration.Blazor.Chat.WebView;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.FileProviders;

namespace WPF_AIChatControl {
    internal class ChatBlazorWebView : BlazorWebView {
        public override IFileProvider CreateFileProvider(string contentRootDir) {
            return new EmbeddedFileProvider(
                typeof(IChatUIWrapper).Assembly,
                typeof(ChatUIWrapper).Namespace);
        }
    }
}
