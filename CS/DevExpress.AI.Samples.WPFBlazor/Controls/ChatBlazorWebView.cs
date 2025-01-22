using System.IO;
using DevExpress.AIIntegration.Blazor.Chat.WebView;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using Microsoft.Extensions.FileProviders;

namespace WPF_AIChatControl {
    internal class ChatBlazorWebView : BlazorWebView {
        public override IFileProvider CreateFileProvider(string contentRootDir) {
            var embeddedProvider = new EmbeddedFileProvider(
                    typeof(ChatUIWrapper).Assembly,
                    typeof(ChatUIWrapper).Namespace);
            string wwwrootPath = Path.Combine(contentRootDir, "wwwroot");
            if(Directory.Exists(wwwrootPath))
                return new CompositeFileProvider([embeddedProvider, new PhysicalFileProvider(wwwrootPath)]);
            return embeddedProvider;
        }
    }
}
