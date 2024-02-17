using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
using Python.Runtime;
using 比亚迪AGS_WPF.DataObject;

namespace 比亚迪AGS_WPF.ViewModels;

public class ScriptRequestMessage : RequestMessage<ScriptResponse>
{
    public string Topic { get; set; }
    public string Function { get; set; }
    public string? Path { get; set; }
    public string[]? Params { get; set; }
}

public class ScriptResponse
{
    public string Topic { get; set; }
    public PyObject Result { get; set; }
}

public class ScriptHubViewModel : ObservableRecipient
{
    public ScriptHubViewModel(IServiceProvider services)
    {
        async void ScriptRequestHandler(ScriptHubViewModel s, ScriptRequestMessage e)
        {
            var execute = services.GetService<IScriptExecutor>();
            if (execute == null) return;
            Messenger.Send(new TestLog()
            {
                Level = "Info",
                Log = $"Executing script: {e.Path} {e.Function} {string.Join(" ", e.Params ?? Array.Empty<string>())}"
            });
            try
            {
                var r = await Task.Run(() => execute.Execute(e.Path, e.Function, e.Params));
                Messenger.Send(new ScriptResponse() { Topic = e.Topic, Result = r });
                Messenger.Send(new TestLog()
                {
                    Level = "Info",
                    Log = $"Script executed successfully: {e.Path} {e.Function}"
                });
            }
            catch (Exception ex)
            {
                Messenger.Send(new TestLog()
                {
                    Level = "Error",
                    Log = $"An error occurred while executing the script: {ex.Message}"
                });
                // Handle the exception here, for example, log it or show an error message to the user
                // Console.WriteLine($"An error occurred while executing the script: {ex.Message}");
            }
        }

        Messenger.Register<ScriptHubViewModel, ScriptRequestMessage>(this,
            ScriptRequestHandler);
    }
}