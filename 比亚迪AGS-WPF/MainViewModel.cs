using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using SimpleTCP;
using Workstation.ServiceModel.Ua;

namespace 比亚迪AGS_WPF;

public class TestItem
{
    public string Name { get; set; }
    public string Parameter { get; set; }
    public string Value { get; set; }
    public string Result { get; set; }
}


[Subscription(endpointUrl: "MainPLC", publishingInterval: 500, keepAliveCount: 20)]
public partial class MainViewModel : SubscriptionBase
{
    private readonly SimpleTcpServer _simpleTcpServer;

    public MainViewModel(SimpleTcpServer simpleTcpServer)
    {
        _simpleTcpServer = simpleTcpServer;
        Title = App.Current.Config.GetValue<string>("title");
        Version = App.Current.Config.GetValue<string>("Version");
        PhoneNumber = App.Current.Config.GetValue<string>("PhoneNumber");
        DispatcherTimer timer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(500),
        };
        timer.Tick += ((sender, args) =>
        {
            NotifyPropertyChanged(nameof(CurrentTIme));
            NotifyPropertyChanged(nameof(TcpStatus));
           // TcpStatus
        });
        timer.Start();

    }


    #region fields

    private ServerStatusDataType? _serverServerStatus;
    private string? _title;
    private string? _version;
    private string _productBarcode = "产品条码";
    private string _userName = "作业员";
    private string _mode = "本地模式";
    private string _productId = "产品编码";
    private string _productCode = "产品代码";
    private string _fixtureBinding = "01";
    private string _productName = "产品1";
    private string _runningStatus = "设备正在进行那个项目，哪那里有异常信息以及MES状态反调等在本栏显示，便问题的处理";
    private string _operationPrompt = "操作提示";
    private int _totalCount = 0;
    private int _maintenance;
    private int _workOderQty;
    private int _completeQty;
    private int _okQty;
    private string? _phoneNumber;

    #endregion

    #region properties

    /// <summary>
    /// Gets the value of ServerServerStatus.
    /// </summary>
    [MonitoredItem(nodeId: "i=2256")]
    public ServerStatusDataType? ServerServerStatus
    {
        get => _serverServerStatus;
        private set
        {
            SetProperty(ref this._serverServerStatus, value);
            NotifyPropertyChanged(nameof(ServerStatus));
        }
    }

   


    /// <summary>
    /// OPC Status
    /// </summary>
    public Brush ServerStatus
    {
        get
        {
            if (ServerServerStatus == null)
            {
                return Brushes.Gray;
            }

            return ServerServerStatus.State switch
            {
                ServerState.Failed => Brushes.Red,
                ServerState.Shutdown => Brushes.Red,
                ServerState.Suspended => Brushes.Red,
                ServerState.Running => Brushes.Chartreuse,
                ServerState.CommunicationFault => Brushes.Fuchsia,
                _ => Brushes.Gray
            };
        }
    }

    /// <summary>
    /// 机器人状态
    /// </summary>
    public Brush RobotStatus => Brushes.Chartreuse;


    /// <summary>
    /// 报警状态
    /// </summary>
    public Brush AlarmStatus => Brushes.Chartreuse;


    /// <summary>
    /// TCP状态
    /// </summary>
    public Brush TcpStatus => _simpleTcpServer.ConnectedClientsCount>0? Brushes.Chartreuse:Brushes.Brown;


    /// <summary>
    /// 标题
    /// </summary>
    public string? Title
    {
        get => _title;
        private set => SetProperty(ref this._title, value);
    }

    /// <summary>
    /// 版本
    /// </summary>

    public string? Version
    {
        get => _version;
        private set => SetProperty(ref this._version, value);
    }

    /// <summary>
    /// 产品条码
    /// </summary>

    public string ProductBarcode
    {
        get => _productBarcode;
        set => SetProperty(ref this._productBarcode, value);
    }

    /// <summary>
    /// 当前用户名
    /// </summary>
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref this._userName, value);
    }

    /// <summary>
    /// 模式
    /// </summary>
    public string Mode
    {
        get => _mode;
        set => SetProperty(ref this._mode, value);
    }

    /// <summary>
    /// 产品编码
    /// </summary>

    public string ProductId
    {
        get => _productId;
        set => SetProperty(ref this._productId, value);
    }

    /// <summary>
    /// 产品名称
    /// </summary>

    public string ProductName
    {
        get => _productName;
        set => SetProperty(ref this._productName, value);
    }


    /// <summary>
    /// 产品代码
    /// </summary>

    public string ProductCode
    {
        get => _productCode;
        set => SetProperty(ref this._productCode, value);
    }

    /// <summary>
    /// 工装绑定
    /// </summary>

    public string FixtureBinding
    {
        get => _fixtureBinding;
        set => SetProperty(ref this._fixtureBinding, value);
    }
    
    /// <summary>
    /// 工装绑定
    /// </summary>

    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref this._phoneNumber, value);
    }

    /// <summary>
    /// 运行状态
    /// </summary>
    ///
    public string RunningStatus
    {
        get => _runningStatus;
        set => SetProperty(ref this._runningStatus, value);
    }

    /// <summary>
    /// 操作提示
    /// </summary>
    public string OperationPrompt
    {
        get => _operationPrompt;
        set => SetProperty(ref this._operationPrompt, value);
    }

    /// <summary>
    /// 总次数
    /// </summary>
    public int TotalCount
    {
        get => _totalCount;
        set => SetProperty(ref this._totalCount, value);
    }

    /// <summary>
    /// 保养计数
    /// </summary>
    public int Maintenance
    {
        get => _maintenance;
        set => SetProperty(ref this._maintenance, value);
    }

    /// <summary>
    /// 工单数量
    /// </summary>
    public int WorkOderQty
    {
        get => _workOderQty;
        set => SetProperty(ref this._workOderQty, value);
    }

    /// <summary>
    /// 完成数
    /// </summary>
    public int CompleteQty
    {
        get => _completeQty;
        set
        {
            SetProperty(ref this._completeQty, value); 
            NotifyPropertyChanged(nameof(YieldRate));
        }
    }

    /// <summary>
    /// 合格数
    /// </summary>
    
    public int OkQty
    {
        get => _okQty;
        set
        {
            SetProperty(ref this._okQty, value); 
            NotifyPropertyChanged(nameof(YieldRate));
        }
    }

    /// <summary>
    /// 合格率
    /// </summary>
    public string YieldRate => _totalCount > 0 ? (_okQty * 1.0 / _totalCount).ToString("P2") : 0.ToString("P2");
  
    /// <summary>
   /// 当前时间
   /// </summary>

   public string CurrentTIme=>DateTime.Now.ToString("G");
    /// <summary>
    /// 测试项目
    /// </summary>
    public ObservableCollection<TestItem> TestItems { get; } = new()
    {
        new TestItem { Name = "Test 1", Parameter = "Parameter 1", Value = "10", Result = int.Parse("10") >= 0 ? "OK" : "NG" },
        new TestItem { Name = "Test 2", Parameter = "Parameter 2", Value = "-5", Result = int.Parse("-5") >= 0 ? "OK" : "NG" },
        new TestItem { Name = "Test 3", Parameter = "Parameter 3", Value = "20", Result = int.Parse("20") >= 0 ? "OK" : "NG" },
        new TestItem { Name = "Test 4", Parameter = "Parameter 4", Value = "-15", Result = int.Parse("-15") >= 0 ? "OK" : "NG" }
    };

    public ObservableCollection<dynamic> TestLogs { get; set; } = new ObservableCollection<dynamic>(Enumerable
        .Range(1, 10).Select(i =>
        {
            dynamic expandoObject = new ExpandoObject();
            expandoObject.Name = $"Test {i}";
            expandoObject.Parameter = $"Parameter {i}";
            expandoObject.Value = i * 10;
            expandoObject.Result = i % 2 == 0 ? "OK" : "NG";
            return expandoObject;
        }));

    #endregion


    #region commands

    [RelayCommand]
    private void Exit()
    {
        //this.Exit();
        Application.Current.Shutdown();
    }

    #endregion
}