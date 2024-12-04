using Microsoft.AspNetCore.Components;

namespace WPF_AIChatControl {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DevExpress.Xpf.Core.ThemedWindow {
        public MainWindow() {
            InitializeComponent();
        }

        void AIChatControl_MarkdownConvert(object sender, DevExpress.AIIntegration.Blazor.Chat.WebView.AIChatControlMarkdownConvertEventArgs e) {
            e.HtmlText = (MarkupString)Markdig.Markdown.ToHtml(e.MarkdownText);
        }
    }
}