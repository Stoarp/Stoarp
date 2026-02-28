using StoatSharp;

public class AppConfig
{
    public static ClientConfig GetConfig()
    {
        var config = new ClientConfig();
        config.Debug.LogRestRequest = true;
        config.Debug.LogRestRequestJson = true;
        config.LogMode = StoatLogSeverity.Debug;
        return config;
    }
}