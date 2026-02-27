using System;
using Microsoft.Extensions.DependencyInjection;

namespace Stoarp.Services;

public static class ServiceLocator
{
    private static IServiceProvider? _provider;

    public static IServiceProvider Provider =>
        _provider ?? throw new InvalidOperationException("ServiceLocator has not been initialized.");

    public static void Initialize(IServiceProvider provider)
    {
        _provider = provider;
    }

    public static T GetRequiredService<T>() where T : notnull =>
        Provider.GetRequiredService<T>();
}
