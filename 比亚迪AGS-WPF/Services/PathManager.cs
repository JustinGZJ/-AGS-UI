using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using 比亚迪AGS_WPF.Config;

namespace 比亚迪AGS_WPF.Services;

// 路径管理类，预设了一些常用的路径，如果不存在就主动创建
public static class AppPath
{
    public static string ExePath { get; } = AppDomain.CurrentDomain.BaseDirectory;
    public static string LogPath { get; } = Path.Combine(ExePath, "Logs");
    

    public static string ConfigPath { get; } = Path.Combine(ExePath, "Config");
    public static string? DataPath=>App.Current.Services.GetService<RootConfig>()?.DataPath;
    public static string? ScriptsPath=>App.Current.Services.GetService<RootConfig>()?.PythonScriptHome;
    
    public static string AppSettingsPath { get; } = Path.Combine(ConfigPath, "AppSettings.json");
    static AppPath()
    {
        if (!Directory.Exists(ExePath))
        {
            Directory.CreateDirectory(ExePath);
        }
        if (!Directory.Exists(LogPath))
        {
            Directory.CreateDirectory(LogPath);
        }
        if (!Directory.Exists(ConfigPath))
        {
            Directory.CreateDirectory(ConfigPath);
        }
        if (!Directory.Exists(DataPath))
        {
            Directory.CreateDirectory(DataPath);
        }
        if (!Directory.Exists(ScriptsPath))
        {
            Directory.CreateDirectory(ScriptsPath);
        }
    }
}