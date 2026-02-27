using System;
using System.IO;

namespace Stoarp.Services;

public class LogService : IDisposable
{
    enum LogLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    private readonly string _root;
    private readonly TextWriter logFile;

    public LogService(string? rootPath = null, string? log = null)
    {
        _root = rootPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Stoarp"
        );
        logFile = File.CreateText(Path.Combine(_root, "Stoarp.log"));
    }

    private void Log(string msg, LogLevel lvl)
    {
        string message = $"[{lvl.ToString()}] {msg}";
        Console.WriteLine(message);
        logFile.WriteLine(message);
    }

    public void LogDebug(string msg)
    {
        Log(msg, LogLevel.Debug);
    }

    public void LogInfo(string msg)
    {
        Log(msg, LogLevel.Info);
    }

    public void LogWarning(string msg)
    {
        Log(msg, LogLevel.Warning);
    }

    public void LogError(string msg)
    {
        Log(msg, LogLevel.Error);
    }

    public void Dispose()
    {
        logFile.Dispose();   
    }
}