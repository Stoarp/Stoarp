using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using ReactiveUI;
using Stoarp.Services;
using Stoarp.Views;

namespace Stoarp.ViewModels;

public class RegisterViewModel : RoutableViewModelBase
{
    public override string UrlPathSegment => "register";

    public ReactiveCommand<Unit, Unit> BackToLoginCommand { get; }

    public RegisterViewModel(IScreen hostScreen, IStoatClientService clientService)
        : base(hostScreen)
    {
        BackToLoginCommand = ReactiveCommand.Create(() =>
        {
            var loginVm = new LoginViewModel(HostScreen, clientService);
            HostScreen.Router.Navigate.Execute(loginVm).Subscribe();
        });
    }
}
