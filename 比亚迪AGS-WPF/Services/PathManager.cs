using System;
using System.IO;

namespace 比亚迪AGS_WPF.Services;

// 路径管理类，预设了一些常用的路径，如果不存在就主动创建
public static class AppPath
{
    public static string ExePath { get; } = AppDomain.CurrentDomain.BaseDirectory;
    public static string LogPath { get; } = Path.Combine(ExePath, "Logs");
    

    public static string ConfigPath { get; } = Path.Combine(ExePath, "Config");
    public static string DataPath { get; } = Path.Combine(ExePath, "Data");
    public static string ScriptsPath { get; } = Path.Combine(ExePath, "Scripts");
    
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