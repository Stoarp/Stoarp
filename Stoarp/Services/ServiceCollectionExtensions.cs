using Microsoft.Extensions.DependencyInjection;
using Stoarp.ViewModels;

namespace Stoarp.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddStoarpServices(this IServiceCollection services)
    {
        services.AddSingleton<IStoatClientService, StoatClientService>();

        services.AddSingleton<MainWindowViewModel>();
        services.AddTransient<LoginViewModel>();
        services.AddTransient<ShellViewModel>();
        services.AddTransient<ChatViewModel>();

        return services;
    }
}
