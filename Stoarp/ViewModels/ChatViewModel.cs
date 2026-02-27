using System.Reactive;
using ReactiveUI;
using Stoarp.Services;

namespace Stoarp.ViewModels;

public class ChatViewModel : RoutableViewModelBase
{
    private string _messageInput = string.Empty;

    public string MessageInput
    {
        get => _messageInput;
        set => this.RaiseAndSetIfChanged(ref _messageInput, value);
    }

    public ReactiveCommand<Unit, Unit> SendMessageCommand { get; }

    public override string UrlPathSegment => "chat";

    public ChatViewModel(IScreen hostScreen, IStoatClientService clientService)
        : base(hostScreen)
    {
        var canSend = this.WhenAnyValue(x => x.MessageInput,
            msg => !string.IsNullOrWhiteSpace(msg));

        SendMessageCommand = ReactiveCommand.Create(() =>
        {
            MessageInput = string.Empty;
        }, canSend);
    }
}
