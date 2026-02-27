using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using Stoarp.Services;
using StoatSharp;

namespace Stoarp.ViewModels;

public class LoginViewModel : RoutableViewModelBase
{
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _errorMessage = string.Empty;
    private bool _isLoggingIn;
    private bool _isMfaRequired;
    private string? _mfaTicket;
    private string? _mfaCode;

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

    public bool IsMfaRequired
    {
        get => _isMfaRequired;
        set => this.RaiseAndSetIfChanged(ref _isMfaRequired, value);
    }

    public string? MfaCode
    {
        get => _mfaCode;
        set => this.RaiseAndSetIfChanged(ref _mfaCode, value);
    }

    public ReactiveCommand<Unit, Unit> LoginCommand { get; }
    public ReactiveCommand<Unit, Unit> NavigateToRegisterCommand { get; }

    public override string UrlPathSegment => "login";

    public LoginViewModel(IScreen hostScreen, IStoatClientService clientService)
        : base(hostScreen)
    {
        var canLogin = this.WhenAnyValue(
            x => x.Email, x => x.Password, x => x.IsLoggingIn,
            (email, password, loggingIn) =>
                !string.IsNullOrWhiteSpace(email) &&
                !string.IsNullOrWhiteSpace(password) &&
                !loggingIn && !IsMfaRequired);

        LoginCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            IsLoggingIn = true;
            ErrorMessage = string.Empty;

            try
            {
                ServiceLocator.GetRequiredService<LogService>().LogInfo("Logging in into User Account");
                var result = await clientService.LoginAsync(Email, Password);
                await HandleLoginResult(result, clientService);
            }
            catch (Exception ex)
            {
                ServiceLocator.GetRequiredService<LogService>().LogError("Failed to Log in: " + ex.ToString());
            }
            finally
            {
                IsLoggingIn = false;
            }
        }, canLogin);

        NavigateToRegisterCommand = ReactiveCommand.Create(() =>
        {
            var registerVm = new RegisterViewModel(HostScreen, clientService);
            HostScreen.Router.Navigate.Execute(registerVm).Subscribe();
        });
    }

    private async Task HandleLoginResult(AccountLogin result, IStoatClientService clientService)
    {
        switch (result.ResponseType)
        {
            case LoginResponseType.Success:
                var shell = new ShellViewModel(HostScreen, clientService);
                await HostScreen.Router.Navigate.Execute(shell);
                break;

            case LoginResponseType.MFARequired:
                IsMfaRequired = true;
                _mfaTicket = result.MFATicket;
                ErrorMessage = "Multi-factor authentication required. Please enter your code.";
                break;

            case LoginResponseType.Disabled:
                ErrorMessage = "This account has been disabled.";
                break;

            case LoginResponseType.OnboardingRequired:
                ErrorMessage = "Onboarding is required. Please complete your profile.";
                break;

            case LoginResponseType.Failed:
            default:
                ErrorMessage = "Login failed. Check your credentials.";
                break;
        }
    }
}
