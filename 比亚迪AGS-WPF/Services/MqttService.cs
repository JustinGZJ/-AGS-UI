
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using 比亚迪AGS_WPF.ViewModels;
using Microsoft.Extensions.Logging;

namespace 比亚迪AGS_WPF.Services;

public class Payload
{
    public string? Cmd { get; set; }
    public string? Action { get; set; }
}

public class MqttClientService : IMqttClientService
{
    private readonly IMqttClientWrapper _mqttClientWrapper;
    private readonly ILogger _logger;

    public MqttClientService(IMqttClientWrapper mqttClientWrapper,ILogger<MqttClientService> logger)
    {
        _mqttClientWrapper = mqttClientWrapper;
        _logger = logger;
    }

    public async Task ConnectAsync()
    {
        await _mqttClientWrapper.ConnectAsync(); 
        _mqttClientWrapper.SubscribeAsync("TestItems", (List<TestItem> message) => 
            { _logger.LogDebug( JsonConvert.SerializeObject(message) ); });
        
        _mqttClientWrapper.SubscribeAsync("inst/test", (Payload message) =>
        {
            _logger.LogDebug( JsonConvert.SerializeObject(message) );
            var scriptExecutor=App.Current.Services.GetService<IScriptExecutor>();
            scriptExecutor?.Execute(message.Cmd!, message.Action!);
        });
    }

    public async Task DisconnectAsync()
    {
        await _mqttClientWrapper.DisconnectAsync();
    }

    public bool IsConnected => _mqttClientWrapper.IsConnected;
}

public interface IMqttClientService
{
    public Task ConnectAsync();
    public Task DisconnectAsync();

    public bool IsConnected { get; }
}