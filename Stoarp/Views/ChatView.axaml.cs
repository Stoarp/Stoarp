using ReactiveUI.Avalonia;
using Stoarp.ViewModels;

namespace Stoarp.Views;

public partial class ChatView : ReactiveUserControl<ChatViewModel>
{
    public ChatView()
    {
        InitializeComponent();
    }
}
