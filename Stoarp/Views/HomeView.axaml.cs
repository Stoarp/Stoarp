using ReactiveUI.Avalonia;
using Stoarp.ViewModels;

namespace Stoarp.Views;

public partial class HomeView : ReactiveUserControl<HomeViewModel>
{
    public HomeView()
    {
        InitializeComponent();
    }
}
