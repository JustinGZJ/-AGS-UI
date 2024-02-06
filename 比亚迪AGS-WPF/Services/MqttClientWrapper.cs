using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Messaging.Messages;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using 比亚迪AGS_WPF.ViewModels;

namespace 比亚迪AGS_WPF.Services;

public class TestItemsChangeMessage:ValueChangedMessage<List<TestItem>>
{
    public TestItemsChangeMessage(List<TestItem> value) : base(value)
    {
    }
}

public interface IMqttClientWrapper
{
    Task ConnectAsync();
    Task DisconnectAsync();
    bool IsConnected { get; }
    Task PublishAsync<T>(string topic, T message);
    void SubscribeAsync<T>(string topic, Action<T> messageHandler);
    
    Task UnsubscribeAsync(string topic);
}

/// <summary>
/// This class is a wrapper for the MQTT client.
/// </summary>
public class MqttClientWrapper : IMqttClientWrapper
{
    private readonly IMqttClient _client;
    private readonly MqttClientOptions _options;

    /// <summary>
    /// Indicates whether the client is connected to the MQTT server.
    /// </summary>
    public bool IsConnected => _client.IsConnected;

    private readonly Dictionary<string, Action<MqttApplicationMessageReceivedEventArgs>> _messageHandlers = new();

    /// <summary>
    /// Constructor for the MqttClientWrapper class.
    /// </summary>
    /// <param name="brokerAddress">The address of the MQTT broker.</param>
    /// <param name="brokerPort">The port of the MQTT broker. Default is 1883.</param>
    public MqttClientWrapper(string brokerAddress, int brokerPort = 1883)
    {
        var factory = new MqttFactory();
        _client = factory.CreateMqttClient();

        _options = new MqttClientOptionsBuilder()
            .WithTcpServer(brokerAddress, brokerPort)
            .Build();

        _client.ApplicationMessageReceivedAsync += HandleReceivedApplicationMessage;

        _client.DisconnectedAsync += async e =>
        {
            Console.WriteLine("Disconnected from MQTT server, trying to reconnect...");
            await Task.Delay(TimeSpan.FromSeconds(5));
            try
            {
                await ConnectAsync();
            }
            catch
            {
                Console.WriteLine("Reconnect failed, will try again in 5 seconds...");
            }
        };
    }

    /// <summary>
    /// Connects the client to the MQTT server.
    /// </summary>
    public async Task ConnectAsync()
    {
        var result = await _client.ConnectAsync(_options);
        Console.WriteLine(result.ResultCode == MqttClientConnectResultCode.Success
            ? "Connected to MQTT server"
            : "Failed to connect to MQTT server");
        foreach (var handler in _messageHandlers)
        {
            await _client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(handler.Key)
                .Build());
            Console.WriteLine("Subscribed to topic: " + handler.Key);
        }
    }

    /// <summary>
    /// Disconnects the client from the MQTT server.
    /// </summary>
    public async Task DisconnectAsync()
    {
        await _client.DisconnectAsync();
        _messageHandlers.Clear();
    }

    /// <summary>
    /// Publishes a message to a specific topic.
    /// </summary>
    /// <param name="topic">The topic to publish the message to.</param>
    /// <param name="message">The message to publish.</param>
    public async Task PublishAsync<T>(string topic, T message)
    {
        var payload = JsonConvert.SerializeObject(message);
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(Encoding.UTF8.GetBytes(payload))
            .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
            .WithRetainFlag()
            .Build();

        await _client.PublishAsync(mqttMessage);
    }

    /// <summary>
    /// Subscribes the client to a specific topic.
    /// </summary>
    /// <param name="topic">The topic to subscribe to.</param>
    /// <param name="messageHandler">The handler for the messages received on this topic.</param>
    public void SubscribeAsync<T>(string topic, Action<T> messageHandler)
    {
        _client.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(topic)
            .Build());

        _messageHandlers[topic] = (message) =>
        {
            var msg = message.ApplicationMessage;
            var messageString = Encoding.UTF8.GetString(msg.PayloadSegment.ToArray());
            Console.WriteLine(messageString);
            var receivedMessage = JsonConvert.DeserializeObject<T>(messageString);
            if (receivedMessage != null) messageHandler(receivedMessage);
        };
    }

    /// <summary>
    /// Unsubscribes the client from a specific topic.
    /// </summary>
    /// <param name="topic">The topic to unsubscribe from.</param>
    public async Task UnsubscribeAsync(string topic)
    {
        await _client.UnsubscribeAsync(topic);
        _messageHandlers.Remove(topic);
    }

    /// <summary>
    /// Handles the received application messages.
    /// </summary>
    /// <param name="e">The event arguments containing the received message.</param>
    private Task HandleReceivedApplicationMessage(MqttApplicationMessageReceivedEventArgs e)
    {
        if (_messageHandlers.TryGetValue(e.ApplicationMessage.Topic, out var handler))
        {
            handler(e);
        }

        return Task.CompletedTask;
    }
}