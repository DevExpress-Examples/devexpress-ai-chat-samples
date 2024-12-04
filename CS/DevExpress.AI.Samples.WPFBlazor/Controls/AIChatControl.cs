using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Blazor.Internal;
using Microsoft.AspNetCore.Components.WebView.Wpf;
using DevExpress.AIIntegration.Blazor.Chat;
using DevExpress.AIIntegration.Services.Assistant;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using DevExpress.AIIntegration.Blazor.Chat.WebView;
using DevExpress.Utils;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.AIIntegration;
using Microsoft.Extensions.AI;
using System.Threading.Tasks;
using DevExpress.Xpf.Printing.Native;

namespace WPF_AIChatControl {
    public class AIChatControl : Control {
        
        public static readonly DependencyProperty UseStreamingProperty;
        public static readonly DependencyProperty ContentFormatProperty;
        public static readonly DependencyProperty EmptyStateTextProperty;
        public static readonly DependencyProperty TemperatureProperty;
        public static readonly DependencyProperty MaxTokensProperty;
        public static readonly DependencyProperty FrequencyPenaltyProperty;
        public static readonly DependencyProperty ControlBackgroundProperty;
        public static readonly DependencyProperty ItemBackgroundProperty;

        static AIChatControl() {
            var ownerType = typeof(AIChatControl);
            UseStreamingProperty = DependencyProperty.Register(nameof(UseStreaming), typeof(bool), ownerType, 
                new PropertyMetadata(false, (d, e) => ((AIChatControl)d).OnUseStreamingChanged()));
            ContentFormatProperty = DependencyProperty.Register(nameof(ContentFormat), typeof(ResponseContentFormat), ownerType,
                new PropertyMetadata(ResponseContentFormat.PlainText, (d, e) => ((AIChatControl)d).OnContentFormatChanged()));
            EmptyStateTextProperty = DependencyProperty.Register(nameof(EmptyStateText), typeof(string), ownerType,
                new PropertyMetadata(string.Empty, (d, e) => ((AIChatControl)d).OnEmptyStateTextChanged()));
            TemperatureProperty = DependencyProperty.Register(nameof(Temperature), typeof(float?), ownerType, 
                new PropertyMetadata(null, (d, e) => ((AIChatControl)d).OnTemperatureChanged()));
            MaxTokensProperty = DependencyProperty.Register(nameof(MaxTokens), typeof(int?), ownerType,
                new PropertyMetadata(null, (d, e) => ((AIChatControl)d).OnMaxTokensChanged()));
            FrequencyPenaltyProperty = DependencyProperty.Register(nameof(FrequencyPenalty), typeof(float?), ownerType,
                new PropertyMetadata(null, (d, e) => ((AIChatControl)d).OnFrequencyPenaltyChanged()));
            ControlBackgroundProperty = DependencyProperty.Register(nameof(ControlBackground), typeof(System.Windows.Media.Brush), ownerType,
                new PropertyMetadata(null, (d, e) => ((AIChatControl)d).OnControlBackgroundChanged()));
            ItemBackgroundProperty = DependencyProperty.Register(nameof(ItemBackground), typeof(System.Windows.Media.Brush), ownerType,
                new PropertyMetadata(null, (d, e) => ((AIChatControl)d).OnItemBackgroundChanged()));
        }

        DxChatIncapsulationService incapsulationService;
        RootComponent blazorChatComponent;
        ChatBlazorWebView chatWebView;
        
        public bool UseStreaming {
            get => (bool)GetValue(UseStreamingProperty);
            set => SetValue(UseStreamingProperty, value);
        }
        public ResponseContentFormat ContentFormat {
            get => (ResponseContentFormat)GetValue(ContentFormatProperty);
            set => SetValue(ContentFormatProperty, value);
        }
        public string EmptyStateText {
            get => (string)GetValue(EmptyStateTextProperty);
            set => SetValue(EmptyStateTextProperty, value);
        }
        public float? Temperature {
            get => (float?)GetValue(TemperatureProperty);
            set => SetValue(TemperatureProperty, value);
        }
        public int? MaxTokens {
            get => (int?)GetValue(MaxTokensProperty);
            set => SetValue(MaxTokensProperty, value);
        }
        public float? FrequencyPenalty {
            get => (float?)GetValue(FrequencyPenaltyProperty);
            set => SetValue(FrequencyPenaltyProperty, value);
        }
        public System.Windows.Media.Brush ControlBackground {
            get => (System.Windows.Media.Brush)GetValue(ControlBackgroundProperty);
            set => SetValue(ControlBackgroundProperty, value);
        }
        public System.Windows.Media.Brush ItemBackground {
            get => (System.Windows.Media.Brush)GetValue(ItemBackgroundProperty);
            set => SetValue(ItemBackgroundProperty, value);
        }

        IChatUIWrapper Chat => incapsulationService?.DxChatUI;
        
        EventHandler<AIChatControlMarkdownConvertEventArgs> markdownConvert;
        public event EventHandler<AIChatControlMarkdownConvertEventArgs> MarkdownConvert {
            add => markdownConvert += value;
            remove => markdownConvert -= value;
        }
        EventHandler<AIChatControlMessageSentEventArgs> messageSent;
        public event EventHandler<AIChatControlMessageSentEventArgs> MessageSent {
            add {
                if(messageSent == null && Chat != null)
                    Chat.SetMessageSentCallback(RaiseMessageSent);
                messageSent += value;
            }
            remove {
                messageSent -= value;
                if(messageSent == null && Chat != null)
                    Chat.SetMessageSentCallback(null);
            }
        }
        
        public async Task SendMessage(string text, ChatRole role) {
            if (Chat != null) {
                await Chat.SendMessage(text, role);
                Chat.Update();
            }
        }
        public IEnumerable<BlazorChatMessage> SaveMessages() {
            return Chat?.SaveMessages() ?? [];
        }
        public void LoadMessages(IEnumerable<BlazorChatMessage> messages) {
            if (Chat != null) {
                Chat.LoadMessages(messages);
                Chat.Update();
            }
        }
        public override void OnApplyTemplate() {
            base.OnApplyTemplate();
            this.chatWebView = GetTemplateChild("PART_ChatWebView") as ChatBlazorWebView;
            ConfigureBlazorWebView();
        }
        
        void RaiseMessageSent(MessageSentEventArgs args) {
            messageSent?.Invoke(this, new AIChatControlMessageSentEventArgs(Chat, args.Content));
        }
        MarkupString RaiseMarkdownConvert(string text) {
            if(markdownConvert != null) {
                var e = new AIChatControlMarkdownConvertEventArgs(text);
                markdownConvert(this, e);
                return e.HtmlText ?? new MarkupString();
            }
            return new MarkupString();
        }
        void OnUseStreamingChanged() {
            if(Chat != null) {
                Chat.UseStreaming = UseStreaming;
                Chat.Update();
            }
        }
        void OnContentFormatChanged() {
            if(Chat != null) {
                Chat.ResponseContentFormat = ContentFormat;
                Chat.SetMarkdownConvertCallback(ContentFormat == ResponseContentFormat.Markdown ? RaiseMarkdownConvert : null);
                Chat.Update();
            }
        }
        void OnEmptyStateTextChanged() {
            if(Chat != null) {
                Chat.SetEmptyStateText(EmptyStateText);
                Chat.Update();
            }
        }
        void OnTemperatureChanged() {
            if(Chat != null)
                Chat.Temperature = Temperature;
        }
        void OnMaxTokensChanged() {
            if(Chat != null)
                Chat.MaxTokens = MaxTokens;
        }
        void OnFrequencyPenaltyChanged() {
            if(Chat != null)
                Chat.FrequencyPenalty = FrequencyPenalty;
        }
        void OnItemBackgroundChanged() {
            if(Chat == null)
                return;
            Chat.Colors[ChatUIColor.UserMessageBackground] = ItemBackground.ToColor();
        }
        void OnControlBackgroundChanged() {
            if(Chat == null)
                return;
            var controlBackground = ControlBackground.ToColor();
            Chat.Colors[ChatUIColor.SubmitAreaBackground] = controlBackground;
            Chat.Colors[ChatUIColor.InputBackground] = controlBackground;
            Chat.Colors[ChatUIColor.AssistantMessageBackground] = controlBackground;
            Chat.Colors[ChatUIColor.ButtonNormalBackground] = controlBackground;
            Chat.Colors[ChatUIColor.ButtonDisabledBackground] = controlBackground;
            
        }
        void OnBackgroundChanged() {
            if(Chat == null)
                return;
            var background = Background.ToColor();
            Chat.Colors[ChatUIColor.Background] = background;
            Chat.Colors[ChatUIColor.ButtonHoverBackground] = background;

        }
        void OnForegroundChanged() {
            if(Chat == null)
                return;
            var foreground = Foreground.ToColor();
            Chat.Colors[ChatUIColor.EmptyForeground] = foreground;
            Chat.Colors[ChatUIColor.ScrollViewer] = foreground;
            Chat.Colors[ChatUIColor.InputForeground] = foreground;
            Chat.Colors[ChatUIColor.InputBorder] = Color.FromArgb((int)(255 * 0.25), foreground);
            Chat.Colors[ChatUIColor.InputFocusShadow] = Color.FromArgb((int)(255 * 0.1), foreground);
            Chat.Colors[ChatUIColor.MessageBorder] = Color.FromArgb((int)(255 * 0.25), foreground);
            Chat.Colors[ChatUIColor.UserMessageForeground] = foreground;
            Chat.Colors[ChatUIColor.AssistantMessageForeground] = foreground;

        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
            base.OnPropertyChanged(e);
            if(e.Property == BackgroundProperty)
                OnBackgroundChanged();
            if(e.Property == ForegroundProperty)
                OnForegroundChanged();
        }
        void ConfigureBlazorWebView() {
            incapsulationService = new DxChatIncapsulationService();
            chatWebView.HostPage = StaticResourceIdentifiers.HostPageFilePath;
            chatWebView.Services = GetServiceProvider(incapsulationService);
            chatWebView.RootComponents.Add(new RootComponent() {
                Selector = StaticResourceIdentifiers.AppDivId,
                ComponentType = typeof(ChatUIWrapper),
                Parameters = new Dictionary<string, object>() {
                    { nameof(DxAIChat.Initialized), new EventCallback<IAIChat>(null, OnChatInitialized)}
                }
            });
            blazorChatComponent = chatWebView.RootComponents[0];
        }
        ServiceProvider GetServiceProvider(DxChatIncapsulationService incapsulationService) {
            var chatClientAIService = AIExtensionsContainerDesktop.Default.GetService<IChatClient>();
            if(chatClientAIService == null)
                throw new InvalidOperationException("There is no registered service of type Microsoft.Extensions.AI.IChatClient");
            var aiAssistantFactory = AIExtensionsContainerDesktop.Default.GetService<IAIAssistantFactory>();
            return ServiceProviderBuildHelper.BuildServiceProvider(incapsulationService,
                s => s.AddWpfBlazorWebView(),
                chatClientAIService, aiAssistantFactory);
        }
        void OnChatInitialized(IAIChat chat) {
            var chatWrapper = chat as IChatUIWrapper;
            if(chatWrapper == null) 
                return;
            chatWrapper.UseStreaming = UseStreaming;
            chatWrapper.ResponseContentFormat = ContentFormat;
            chatWrapper.SetEmptyStateText(EmptyStateText);
            chatWrapper.Temperature = Temperature;
            chatWrapper.FrequencyPenalty = FrequencyPenalty;
            chatWrapper.MaxTokens = MaxTokens;
            if(messageSent != null)
                chatWrapper.SetMessageSentCallback(RaiseMessageSent);
            if(ContentFormat == ResponseContentFormat.Markdown)
                chatWrapper.SetMarkdownConvertCallback(RaiseMarkdownConvert);
            OnBackgroundChanged();
            OnForegroundChanged();
            OnControlBackgroundChanged();
            OnItemBackgroundChanged();
        }
    }
    static class ColorExtensions {
        public static Color ToColor(this System.Windows.Media.Brush brush) {
            return (brush as System.Windows.Media.SolidColorBrush)?.Color.ToColor() ?? Color.Black;
        }
    }
}
