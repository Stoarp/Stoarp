using System;
using ReactiveUI;
using Stoarp.Services;

namespace Stoarp.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    public RoutingState Router { get; } = new();

    public MainWindowViewModel(IStoatClientService clientService)
    {
        Router.Navigate.Execute(new LoginViewModel(this, clientService)).Subscribe();
    }
}
