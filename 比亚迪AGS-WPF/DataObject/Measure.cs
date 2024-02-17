using System.Collections.Generic;
using 比亚迪AGS_WPF.ViewModels;

namespace 比亚迪AGS_WPF.DataObject;

public class Measure
{
    public string? Name { get; set; }
    public string Result { get; set; }
    public List<TestItem> TestItems { get; set; }
}