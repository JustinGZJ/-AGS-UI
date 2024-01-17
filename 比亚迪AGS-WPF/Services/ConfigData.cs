using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace 比亚迪AGS_WPF.Services;



public class ConfigData<T>
{
    private readonly string _fileName;
    public List<T> Data { get;  set; }

    public ConfigData(string fileName)
    {
        _fileName = fileName;
        Data = new List<T>();
        Data = new List<T>();
    }
    public void Add(T item)
    {
        Data.Add(item);
    }

    public void Save()
    {
        string json = JsonConvert.SerializeObject(Data);
        File.WriteAllText(_fileName, json);
    }

    public void Load()
    {
        //   throw new FileNotFoundException("文件名为空");
        if(!File.Exists(_fileName))
            return;
        string json = File.ReadAllText(_fileName);
        Data = JsonConvert.DeserializeObject<List<T>>(json);
    }
}
