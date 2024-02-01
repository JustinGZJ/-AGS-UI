using System;
using System.Collections.Generic;
using Python.Runtime;
using 比亚迪AGS_WPF.Config;

namespace 比亚迪AGS_WPF;


public class PythonExecutor
{
    public PythonExecutor(string pythonPath)
    {
        Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", pythonPath);
        PythonEngine.Initialize();
    }
    public void RunPython(string pythonCode)
    {
        using (Py.GIL())
        {
            PythonEngine.RunSimpleString(pythonCode);
        }
    }
    
    public void RunPython(string pythonCode, IDictionary<string, object> parameters)
    {
        using (Py.GIL())
        {
            using PyModule scope = Py.CreateScope();
            foreach (var parameter in parameters)
                scope.Set(parameter.Key, parameter.Value.ToPython());
            scope.Exec(pythonCode);
        }
    }

    public dynamic RunPythonFile(string filePath,Func<dynamic,dynamic> action)
    {
        using (Py.GIL())
        {
           dynamic py= Py.Import(filePath);
           return action(py);
        }
    }
}
