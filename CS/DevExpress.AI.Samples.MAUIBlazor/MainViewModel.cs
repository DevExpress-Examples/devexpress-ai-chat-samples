using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.AI;
namespace DevExpress.AI.Samples.MAUIBlazor;

partial class MainViewModel : ObservableObject {

    [ObservableProperty, NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    public string? message;

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    async Task SendMessageAsync() {
        var service = DxChatEncapsulationService.Instance;
        await service.DxChatUI?.SendMessageAsync(Message);
        Message = null;
    }

    bool CanSendMessage() {
        return !string.IsNullOrEmpty(Message);
    }
}
