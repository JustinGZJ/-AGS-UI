using System;

namespace 比亚迪AGS_WPF.DataObject;

public class TestLog
{
    public string Time { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public string Type { get; set; }
    public string Log { get; set; }
    public string Level { get; set; }
}