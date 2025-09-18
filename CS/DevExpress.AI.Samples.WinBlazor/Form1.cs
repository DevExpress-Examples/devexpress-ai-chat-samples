using DevExpress.AIIntegration.WinForms.Chat;
using DevExpress.AIIntegration.Blazor.Chat.WebView;
using DevExpress.XtraEditors;
using DevExpress.Utils;
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

        void InitializeBlazorAIChat()
        {
            AIChatControl chat = new AIChatControl()
            {
                Name = "aiChatControl1",
                Dock = DockStyle.Fill,
                ContentFormat = AIIntegration.Blazor.Chat.ResponseContentFormat.Markdown,
                UseStreaming = DefaultBoolean.True,
                FileUploadEnabled = DefaultBoolean.True
            };

            chat.MarkdownConvert += Chat_MarkdownConvert;

            chat.OptionsFileUpload.FileTypeFilter.AddRange(new List<string> { "text/plain", "application/pdf", "image/png" }); // Allowed MIME types.
            chat.OptionsFileUpload.AllowedFileExtensions.AddRange(new List<string> { ".txt", ".pdf", ".png" });
            chat.OptionsFileUpload.MaxFileCount = 5;
            chat.OptionsFileUpload.MaxFileSize = 5 * 1024 * 1024; // 5 MB

            chat.SetPromptSuggestions(new List<PromptSuggestion>(){
                new PromptSuggestion(
                    title: "Birthday Wish",
                    text: "A warm and cheerful birthday greeting message.",
                    prompt: "Write a heartfelt birthday message for a close friend.",
                    sendOnClick: false),

                new PromptSuggestion(
                    "Thank You Note",
                    "A polite thank you note to express gratitude.",
                    "Compose a short thank you note to a colleague who helped with a project.",
                    false)
            });

            Controls.Add(chat);
        }

        void Chat_MarkdownConvert(object? sender, AIChatControlMarkdownConvertEventArgs e)
        {
            e.HtmlText = (MarkupString)Markdown.ToHtml(e.MarkdownText);
        }
    }
}