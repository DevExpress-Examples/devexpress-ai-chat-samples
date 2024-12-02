using DevExpress.AIIntegration.WinForms.Chat;
using DevExpress.Blazor.Internal;
using DevExpress.XtraEditors;
using Markdig;
using Microsoft.AspNetCore.Components;

namespace DevExpress.AI.Samples.WinBlazor
{
    public partial class Form1 : XtraForm
    {
        public Form1()
        {
            InitializeComponent();
            InitializeBlazorAIChat();
        }

        private void InitializeBlazorAIChat()
        {
            AIChatControl chat = new AIChatControl()
            {
                Name = "aiChatControl1",
                Dock = DockStyle.Fill,
                ContentFormat = AIIntegration.Blazor.Chat.ResponseContentFormat.Markdown,
                UseStreaming = DevExpress.Utils.DefaultBoolean.True
            };
            chat.MarkdownConvert += Chat_MarkdownConvert;
            Controls.Add(chat);
        }

        private void Chat_MarkdownConvert(object? sender, AIIntegration.Blazor.Chat.WebView.AIChatControlMarkdownConvertEventArgs e)
        {
            e.HtmlText = (MarkupString)Markdown.ToHtml(e.MarkdownText);
        }
    }
}