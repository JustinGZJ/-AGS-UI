using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Python.Runtime;

namespace 比亚迪AGS_WPF;

public interface IScriptExecutor
{
    void Execute(string pythonCode);
    void Execute(string pythonCode, IDictionary<string, object> parameters);
    dynamic Execute(string? moduleName, string functionName, params object[]? args);
}

public class PythonExecutor : IScriptExecutor
{
    public PythonExecutor(string pythonPath)
    {
        if (PythonEngine.IsInitialized) return;
        Runtime.PythonDLL = pythonPath;

        PythonEngine.Initialize();
        PythonEngine.BeginAllowThreads();
    }

    public void Execute(string pythonCode)
    {
        using (Py.GIL())
        {
            PythonEngine.RunSimpleString(pythonCode);
        }
    }

    public void Execute(string pythonCode, IDictionary<string, object> parameters)
    {
        using (Py.GIL())
        {
            using PyModule scope = Py.CreateScope();
            foreach (var parameter in parameters)
                scope.Set(parameter.Key, parameter.Value.ToPython());
            scope.Exec(pythonCode);
        }
    }

    public object[] Execute(string pythonCode, IDictionary<string, object> parameters, string[] returnedVariableNames)
    {
        object[] result = new object[returnedVariableNames.Length];
        using (Py.GIL())
        {
            using PyModule scope = Py.CreateScope();
            foreach (var parameter in parameters)
                scope.Set(parameter.Key, parameter.Value.ToPython());
            scope.Exec(pythonCode);
            for (int i = 0; i < returnedVariableNames.Length; i++)
                result[i] = scope.Get<object>(returnedVariableNames[i]);
        }

        return result;
    }


    public dynamic Execute(string? moduleName, string functionName, params object[]? args)
    {
        using (Py.GIL())
        {
            // PythonEngine.
            using var scope = Py.CreateScope();
            var code = File.ReadAllText(moduleName);
            var pyObject = scope.Exec(code);
            var result = args == null
                ? pyObject.InvokeMethod(functionName)
                : pyObject.InvokeMethod(functionName, args.Select(x => x.ToPython()).ToArray());
            return result;
            
        }
    
        
    }
}