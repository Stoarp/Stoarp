using System.Threading.Tasks;
using StoatSharp;

namespace Stoarp.Services;

public interface IStoatClientService
{
    StoatClient? Client { get; }
    bool IsConnected { get; }
    Task<AccountLogin> LoginAsync(string email, string password);
    Task RegisterAsync(string email, string password, string? captchaToken = null);
    Task DisconnectAsync();
}
