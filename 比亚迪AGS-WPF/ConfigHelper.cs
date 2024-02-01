using System.IO;
using Newtonsoft.Json;

namespace 比亚迪AGS_WPF;

public static class ConfigHelper
{
    public static T LoadConfig<T>(string fileName) where T : new()
    {
        return File.Exists(fileName)
            ? JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName))
            : new T();
    }

    public static void SaveConfig<T>(T config, string fileName)
    {
        string json = JsonConvert.SerializeObject(config, Formatting.Indented);
        File.WriteAllText(fileName, json);
    }
}