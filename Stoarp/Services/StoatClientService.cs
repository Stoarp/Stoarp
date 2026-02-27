using System;
using System.Threading.Tasks;
using StoatSharp;

namespace Stoarp.Services;

public class StoatClientService : IStoatClientService
{
    private StoatClient? _client;

    public StoatClient? Client => _client;
    public bool IsConnected => _client?.IsLoginComplete ?? false;

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            _client = new StoatClient(ClientMode.WebSocket);
            var result = await _client.LoginAsync(email, password, "Stoarp Desktop");
            if (result != null)
            {
                await _client.StartAsync();
                return true;
            }
            return false;
        }
        catch (Exception)
        {
            _client = null;
            return false;
        }
    }

    public async Task DisconnectAsync()
    {
        if (_client != null)
        {
            await _client.StopAsync();
            _client = null;
        }
    }
}
