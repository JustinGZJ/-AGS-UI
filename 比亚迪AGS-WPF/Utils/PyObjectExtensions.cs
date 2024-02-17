using Newtonsoft.Json;
using Python.Runtime;

namespace 比亚迪AGS_WPF.Utils;

public static class PyObjectExtensions
{
    public static T? Cask<T>(this PyObject pyObject)
    {
        return JsonConvert.DeserializeObject<T>(pyObject.ToString());
    }
}