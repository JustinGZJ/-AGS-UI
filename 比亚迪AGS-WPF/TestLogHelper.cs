using System;
using System.Collections.Generic;
using System.IO;
using 比亚迪AGS_WPF.Services;

namespace 比亚迪AGS_WPF;

public static class TestLogHelper
{
    public static void SaveFile(DataUploadMessage message, IEnumerable<TestItem> testItems)
    {
        var directoryPath = AppPath.DataPath;
        var fileName = GenerateFileName(directoryPath);
        var data = PrepareData(message, testItems);

        WriteDataToFile(fileName, data);
    }
    

    private static string GenerateFileName(string directoryPath)
    {
        return Path.Combine( directoryPath ,DateTime.Now.ToString("yyyy-MM-dd") + ".csv");
    }

    private static Dictionary<string, string> PrepareData(DataUploadMessage message, IEnumerable<TestItem> testItems)
    {
        var dictionary = new Dictionary<string, string>
        {
            { "时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
            { "站位", message.Station },
            { "识别码", message.Code },
            { "结果", message.Result }
        };

        foreach (var item in testItems)
        {
            dictionary.TryAdd(item.Name, item.Value.Trim());
        }

        return dictionary;
    }

    private static void WriteDataToFile(string fileName, Dictionary<string, string> data)
    {
        var header = string.Join(",", data.Keys);
        var content = string.Join(",", data.Values);

        if (!File.Exists(fileName))
        {
            File.WriteAllText(fileName, header + Environment.NewLine);
        }

        File.AppendAllText(fileName, content + Environment.NewLine);
    }
}