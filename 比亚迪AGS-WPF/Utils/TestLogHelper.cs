using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using 比亚迪AGS_WPF.Services;

namespace 比亚迪AGS_WPF;

public static class TestLogHelper
{
    public static void SaveFile(Dictionary<string, string> message, IEnumerable<ViewModels.TestItem> testItems)
    {
        var directoryPath = AppPath.DataPath;
        var fileName = GenerateFileName(directoryPath);
        var data = PrepareData(message, testItems);

        WriteDataToFile(fileName, data);
    }


    private static string GenerateFileName(string directoryPath)
    {
        return Path.Combine(directoryPath, DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
    }

    private static Dictionary<string, string> PrepareData(Dictionary<string, string> dictionary,
        IEnumerable<ViewModels.TestItem> testItems)
    {
        foreach (var item in testItems)
        {
            dictionary.TryAdd(item.Name, item.Value.Trim());
        }

        return dictionary;
    }

    private static void WriteDataToFile(string fileName, Dictionary<string, string> data)
    {
        var header = string.Join(",", data.Keys.Select(k => $"\"{k}\""));
        var content = string.Join(",", data.Values.Select(v => $"\"{v}\""));

        if (!File.Exists(fileName))
        {
            File.WriteAllText(fileName, header + Environment.NewLine);
        }

        File.AppendAllText(fileName, content + Environment.NewLine);
    }
}