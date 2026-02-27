using System.Threading.Tasks;
using StoatSharp;

namespace Stoarp.Services;

public interface IStoatClientService
{
    StoatClient? Client { get; }
    bool IsConnected { get; }
    Task<bool> LoginAsync(string email, string password);
    Task DisconnectAsync();
}
