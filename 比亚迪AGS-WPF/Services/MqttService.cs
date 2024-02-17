using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using 比亚迪AGS_WPF.ViewModels;
using Microsoft.Extensions.Logging;
using 比亚迪AGS_WPF.Config;

namespace 比亚迪AGS_WPF.Services;

public class MqttPayload
{
    
    public string? Function { get; set; }
    public string? Path { get; set; }
    public string[]? Params { get; set; }
}

public class MqttClientService : IMqttClientService
{
    private readonly IMqttClientWrapper _mqttClientWrapper;
    private readonly ILogger _logger;
    private readonly RootConfig _config;

    public MqttClientService(IMqttClientWrapper mqttClientWrapper,ILogger<MqttClientService> logger,RootConfig config)
    {
        _mqttClientWrapper = mqttClientWrapper;
        _logger = logger;
        _config = config;
    }

    public async Task ConnectAsync()
    {
        await _mqttClientWrapper.ConnectAsync(); 
        
        _mqttClientWrapper.SubscribeAsync($"{_config.MqttTopic}", (MqttPayload message) =>
        {
            _logger.LogDebug( JsonConvert.SerializeObject(message) );
            WeakReferenceMessenger.Default.Send(new ScriptRequestMessage()
            {
                Topic = "Measure",
                Function = message.Function!,
                Path =message.Path!,
                Params = message.Params
            });
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