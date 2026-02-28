using System;
using System.Threading.Tasks;
using StoatSharp;

namespace Stoarp.Services;

public class StoatClientService : IStoatClientService
{
    private StoatClient? _client;

    public StoatClient? Client => _client;
    public bool IsConnected => _client?.IsLoginComplete ?? false;

    public async Task<AccountLogin> LoginAsync(string email, string password)
    {
        try
        {
            _client = new StoatClient(ClientMode.WebSocket, AppConfig.GetConfig());
            var result = await _client.LoginAsync(email, password, "Stoarp Desktop");
            if (result.ResponseType == LoginResponseType.Success)
            {
                await _client.StartAsync();
            }
            return result;
        }
        catch (Exception)
        {
            _client = null;
            return new AccountLogin
            {
                ResponseType = LoginResponseType.Failed
            };
        }
    }

    public async Task RegisterAsync(string email, string password, string? captchaToken = null)
    {
        var client = new StoatClient(ClientMode.Http, AppConfig.GetConfig());
        await AccountHelper.CreateAccountAsync(client.Rest, email, password, null, captchaToken);
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
