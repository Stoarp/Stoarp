using ReactiveUI.Avalonia;
using Stoarp.ViewModels;

namespace Stoarp.Views;

public partial class ShellView : ReactiveUserControl<ShellViewModel>
{
    public ShellView()
    {
        InitializeComponent();
    }
}
