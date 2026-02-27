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
using AvaloniaWebView;

namespace Stoarp.ViewModels;

public class RegisterViewModel : RoutableViewModelBase
{
    
    private string _email = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;
    private string _errorMessage = string.Empty;
    private string _successMessage = string.Empty;
    private bool _isRegistering;
    private bool _isCaptchaRequired;
    private string _captchaToken = string.Empty;

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

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public string SuccessMessage
    {
        get => _successMessage;
        set => this.RaiseAndSetIfChanged(ref _successMessage, value);
    }

    public bool IsRegistering
    {
        get => _isRegistering;
        set => this.RaiseAndSetIfChanged(ref _isRegistering, value);
    }

    public bool IsCaptchaRequired
    {
        get => _isCaptchaRequired;
        set => this.RaiseAndSetIfChanged(ref _isCaptchaRequired, value);
    }

    public string CaptchaToken
    {
        get => _captchaToken;
        set
        {
            this.RaiseAndSetIfChanged(ref _captchaToken, value);
        }
    }

    public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
    public ReactiveCommand<Unit, Unit> BackToLoginCommand { get; }

    public override string UrlPathSegment => "register";

    public RegisterViewModel(IScreen hostScreen, IStoatClientService clientService)
        : base(hostScreen)
    {

        var canRegister = this.WhenAnyValue(
            x => x.Email, x => x.Password, x => x.ConfirmPassword, x => x.IsRegistering, x => x.CaptchaToken,
            (email, password, confirmPassword, registering, captchaToken) =>
                !string.IsNullOrWhiteSpace(email) &&
                !string.IsNullOrWhiteSpace(password) &&
                password == confirmPassword &&
                !registering &&
                (!IsCaptchaRequired || !string.IsNullOrWhiteSpace(captchaToken)));

        RegisterCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            IsRegistering = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            try
            {
                if (IsCaptchaRequired && !string.IsNullOrEmpty(CaptchaToken))
                {
                    ServiceLocator.GetRequiredService<LogService>().LogWarning("Captacha Needed");
                    return;
                }

                await clientService.RegisterAsync(Email, Password, CaptchaToken);
                SuccessMessage = "Registration successful! Please check your email to verify your account.";
                Email = string.Empty;
                Password = string.Empty;
                ConfirmPassword = string.Empty;
                CaptchaToken = string.Empty;
            }
            catch (Exception ex)
            {
                ServiceLocator.GetRequiredService<LogService>().LogError($"Registration failed: {ex.Message}");
            }
            finally
            {
                IsRegistering = false;
            }
        }, canRegister);

        BackToLoginCommand = ReactiveCommand.Create(() =>
        {
            var loginVm = new LoginViewModel(HostScreen, clientService);
            HostScreen.Router.Navigate.Execute(loginVm).Subscribe();
        });
    }
}