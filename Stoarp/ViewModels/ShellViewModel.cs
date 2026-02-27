using System;
using ReactiveUI;
using Stoarp.Services;

namespace Stoarp.ViewModels;

public class ShellViewModel : RoutableViewModelBase, IScreen
{
    public RoutingState Router { get; } = new();

    public override string UrlPathSegment => "shell";

    public ShellViewModel(IScreen hostScreen, IStoatClientService clientService)
        : base(hostScreen)
    {
        Router.Navigate.Execute(new ChatViewModel(this, clientService)).Subscribe();
    }
}
