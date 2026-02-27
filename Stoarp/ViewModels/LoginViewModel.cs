using System;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Stoarp.Services;

namespace Stoarp.ViewModels;

public class LoginViewModel : RoutableViewModelBase
{
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoggingIn;

    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public bool IsLoggingIn
    {
        get => _isLoggingIn;
        set => this.RaiseAndSetIfChanged(ref _isLoggingIn, value);
    }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }

    public override string UrlPathSegment => "login";

    public LoginViewModel(IScreen hostScreen, IStoatClientService clientService)
        : base(hostScreen)
    {
        var canLogin = this.WhenAnyValue(
            x => x.Email, x => x.Password, x => x.IsLoggingIn,
            (email, password, loggingIn) =>
                !string.IsNullOrWhiteSpace(email) &&
                !string.IsNullOrWhiteSpace(password) &&
                !loggingIn);

        LoginCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            IsLoggingIn = true;
            ErrorMessage = string.Empty;

            try
            {
                var success = await clientService.LoginAsync(Email, Password);
                if (success)
                {
                    var shell = new ShellViewModel(HostScreen, clientService);
                    await HostScreen.Router.Navigate.Execute(shell);
                }
                else
                {
                    ErrorMessage = "Login failed. Check your credentials.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsLoggingIn = false;
            }
        }, canLogin);
    }
}
