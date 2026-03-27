using System;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Stoarp.Services;

namespace Stoarp.ViewModels;

public class HomeViewModel : RoutableViewModelBase, IScreen
{
    public RoutingState Router { get; } = new();

    public override string UrlPathSegment => "home";

    public string? UserName { get; }
    public string? DisplayName { get; }
    public string? AvatarUrl { get; }
    public Bitmap? AvatarBitmap { get; private set; }
    public string UserInitial => !string.IsNullOrEmpty(UserName) ? UserName[0].ToString().ToUpper() : "?";

    public HomeViewModel(IScreen hostScreen, IStoatClientService clientService, CacheService cacheService)
        : base(hostScreen)
    {
        var client = clientService.Client;
        if (client?.CurrentUser != null)
        {
            UserName = string.Format("({0})", client.CurrentUser.Username);
            DisplayName = client.CurrentUser.DisplayName ?? client.CurrentUser.Username;
            AvatarUrl = client.CurrentUser.GetAvatarUrl();

            if (!string.IsNullOrEmpty(AvatarUrl))
            {
                _ = LoadAvatarAsync(cacheService, AvatarUrl);
            }
        }

        Router.Navigate.Execute(new ChatViewModel(this, clientService)).Subscribe();
    }

    private async Task LoadAvatarAsync(CacheService cacheService, string url)
    {
        try
        {
            AvatarBitmap = await cacheService.GetBitmapAsync(url);
            this.RaisePropertyChanged(nameof(AvatarBitmap));
        }
        catch { }
    }
}
