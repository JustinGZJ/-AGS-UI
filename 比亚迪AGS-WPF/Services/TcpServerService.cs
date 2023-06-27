using System.Linq;
using System.Net;
using System.Text;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SimpleTCP;

namespace 比亚迪AGS_WPF.Services;

public class TcpStatusMessage : ValueChangedMessage<bool>
{
    public TcpStatusMessage(bool value) : base(value)
    {
    }
}

public class CodeVerifyMessage
{
    public string Station { get; set; }
    public string Code { get; set; }
    public Message message { get; set; }
}

public class UserLoginMessage
{
    public string Station { get; set; }
    public string CardNo { get; set; }

    public string Mode { get; set; }
    public Message message { get; set; }
}

public class DataUploadMessage
{
    public string Station { get; set; }
    public string Code { get; set; }
    public string Result { get; set; } = "PASS";
    public string Value { get; set; }
    public Message message { get; set; }
}

public class OrderBindingMessage
{
    public string Station { get; set; }
    public string Order { get; set; }
    public Message message { get; set; }
}

public class AssemblyMessage
{
    public string Station { get; set; }
    public string Code { get; set; }
    public string[] PartCodes { get; set; }
    public Message message { get; set; }
}

public class UserValidate
{
    public string Staff_ID { get; set; }
    public string Code { get; set; }
    public string[] PartCodes { get; set; }
    public Message message { get; set; }
}

public class TcpServerService
{
    private readonly ILogger<TcpServerService> _logger;
    private SimpleTcpServer _server;

    public TcpServerService(ILogger<TcpServerService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _server = new SimpleTcpServer
        {
            Delimiter = 0x13,
            StringEncoder = Encoding.GetEncoding("GB2312")
        };
        _server.DataReceived += Server_DataReceived!;
        _server.ClientConnected += Server_ClientConnected!;
        _server.ClientDisconnected += Server_ClientDisconnected!;
        var port = configuration.GetValue<int>("ServerPort");
        _server.Start(port);
    }

    private void Server_ClientDisconnected(object sender, System.Net.Sockets.TcpClient e)
    {
        WeakReferenceMessenger.Default.Send(new TcpStatusMessage(_server.ConnectedClientsCount > 0));
        _logger.LogInformation("Client disconnected: {ClientRemoteEndPoint}", e.Client.RemoteEndPoint);
        //  Console.WriteLine($"Client disconnected: {e.Client.RemoteEndPoint}");
    }

    private void Server_ClientConnected(object sender, System.Net.Sockets.TcpClient e)
    {
        WeakReferenceMessenger.Default.Send(new TcpStatusMessage(_server.ConnectedClientsCount > 0));
        _logger.LogInformation("Client connected: {ClientRemoteEndPoint}", e.Client.RemoteEndPoint);
        // Console.WriteLine($"Client connected: {e.Client.RemoteEndPoint}");
    }

    //WeakReferenceMessenger 类使用单例模式,可以WeakReferenceMessenger.Default 静态属性来获取默认的全局实例
    private void Server_DataReceived(object sender, SimpleTCP.Message e)
    {
        var msg = e.MessageString;
        _logger.LogInformation($"Received: {msg}");
        var splits = msg.Split(';');
        switch (splits[1])
        {
            case "条码验证":
                WeakReferenceMessenger.Default.Send(new CodeVerifyMessage
                {
                    Station = splits[0],
                    Code = splits[2],
                    message = e
                });
                break;
            case "数据上传":
            case "上传数据":
                if (splits.Length >4)
                {
                    WeakReferenceMessenger.Default.Send(new DataUploadMessage
                    {
                        Station = splits[0],
                        Code = splits[2],
                        Result = splits[3],
                        Value = splits[4],
                        message = e
                    });
                }
                else
                {
                    e.Reply("N;格式错误");
                }
                break;
            case "离散装配":
                WeakReferenceMessenger.Default.Send(new AssemblyMessage
                {
                    Station = splits[0],
                    Code = splits[2],
                    PartCodes = splits[3..],
                    message = e
                });
                break;
            case "用户登录":
                WeakReferenceMessenger.Default.Send(new UserLoginMessage
                {
                    Station = splits[0],
                    Mode = splits[2],
                    CardNo = splits[3],
                    message = e
                });
                break;
            case "工单绑定":  // 站位;工单绑定;工单
                WeakReferenceMessenger.Default.Send(new OrderBindingMessage
                {
                    Station = splits[0],
                    Order = splits[2],
                    message = e
                });
                break;
        }

        //  e.ReplyLine("OK");
    }
}