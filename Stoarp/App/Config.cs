using StoatSharp;

public class AppConfig
{
    public static ClientConfig GetConfig()
    {
        var config = new ClientConfig();
        config.Debug.LogRestRequest = true;
        config.LogMode = StoatLogSeverity.Info;
        return config;
    }
}