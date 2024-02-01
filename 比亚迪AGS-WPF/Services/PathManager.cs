using System;
using System.IO;

namespace 比亚迪AGS_WPF.Services;

// 路径管理类，预设了一些常用的路径，如果不存在就主动创建
public static class AppPath
{
    public static string AppDataPath { get; } = AppDomain.CurrentDomain.BaseDirectory;
    public static string LogPath { get; } = Path.Combine(AppDataPath, "Logs");
    

    public static string ConfigPath { get; } = Path.Combine(AppDataPath, "Config");
    public static string DataPath { get; } = Path.Combine(AppDataPath, "Data");
    
    public static string AppSettingsPath { get; } = Path.Combine(ConfigPath, "AppSettings.json");
    static AppPath()
    {
        if (!Directory.Exists(AppDataPath))
        {
            Directory.CreateDirectory(AppDataPath);
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
    }
}