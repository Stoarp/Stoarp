using ReactiveUI.Avalonia;
using Stoarp.ViewModels;

namespace Stoarp.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
    }
}
