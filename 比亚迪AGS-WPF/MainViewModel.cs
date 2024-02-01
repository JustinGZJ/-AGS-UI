using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Prism.Commands;
using Workstation.ServiceModel.Ua;
using 比亚迪AGS_WPF.Config;
using 比亚迪AGS_WPF.Services;
using 比亚迪AGS_WPF.Views;
using MessageBox = HandyControl.Controls.MessageBox;

namespace 比亚迪AGS_WPF;

public class TestItem
{
    public string Name { get; set; }
    public string Parameter { get; set; }
    public string Value { get; set; }
    public string Result { get; set; }
}

public class TestLog
{
    public string Time { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    public string Type { get; set; }
    public string Log { get; set; }
    public string Level { get; set; }
}

// [Subscription(endpointUrl: "MainPLC", publishingInterval: 500, keepAliveCount: 20)]
public partial class MainViewModel : ObservableRecipient
{
    
    
    private DelegateCommand<string>? _openCommand;

    public DelegateCommand<string> OpenCommand =>
        _openCommand ??= new DelegateCommand<string>(UiChange); //用来打开添加数据库各种模块DeleteProject

    public MainViewModel(RootConfig config)
    {
        _config = config;
        SetupTimer();
        InitializeProperties();
        RegisterMessageHandlers();
        UiChange("TestLogView");
    }


    private void InitializeProperties()
    {
        Title = _config.Title;
        Version = _config.Version;
        PhoneNumber = _config.PhoneNumber;
        Company = _config.Company;
    }

    private void SetupTimer()
    {
        DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500),
        };
        timer.Tick += ((sender, args) => { OnPropertyChanged(nameof(CurrentTIme)); });
        timer.Start();
    }

    private void RegisterMessageHandlers()
    {
        WeakReferenceMessenger.Default.Register<DataUploadMessage>(this, ((recipient, message) =>
        {
            var testItems = message.Value.Split('!').Where(x => x.Contains(",")).Select(x =>
            {
                var m = x.Split(',');
                return new TestItem
                {
                    Name = m[0],
                    Parameter = m[1],
                    Value = m[2],
                    Result = message.Result
                };
            });
            // 保存文件
            TestLogHelper.SaveFile(message, testItems);

            // 刷新界面
            Application.Current.Dispatcher.Invoke(() =>
            {
                TestItems.Clear();
                foreach (var item in testItems)
                {
                    TestItems.Add(item);
                }
            });
        }));

        WeakReferenceMessenger.Default.Register<TestLog>(this, ((recipient, message) =>
        {
            var log = new TestLog
            {
                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                Type = message.Type,
                Log = message.Log,
                Level = message.Level
            };
            Application.Current.Dispatcher.Invoke(() =>
            {
                TestLogs.Add(log);
                if (TestLogs.Count > 200)
                    TestLogs.RemoveAt(0);
            });
        }));
    }
    private void UiChange(string obj)
    {
        switch (obj)
        {
            case "开始运行":
                ContentView = new TestLogView
                {
                    DataContext = this
                };
                break;
            case "停止":
                ContentView = new TestLogView
                {
                    DataContext = this
                };
                break;
            case "监视页面":
                ContentView = new TestLogView
                {
                    DataContext = this
                };
                break;
            case "配置信息":
                ContentView = new ConfigView();
                break;
            case "数据查询":
                ContentView = new DataView();
                break;
            case "用户变更":
                ContentView = new UserView();
                break;
            case "用户操作":
                ContentView = new ScannerView();
                break;
        }
        
        ContentView = obj switch
        {
            "TestLogView" => new TestLogView
            {
                DataContext = this
            },
            "UserView" => new UserView(),
            "ConfigView" => new ConfigView(),
            "EnquireView" => new DataView(),
            "ScannerView" => new ScannerView(),
            _ => ContentView
        };
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
    private string _runningStatus = "运行";
    private string _operationPrompt = "操作提示";
    private int _totalCount;
    private int _maintenance;
    private int _workOderQty;

    private int _completeQty;
    private int _okQty;
    private string? _phoneNumber;
    private bool _robotStatus;
    private bool _alarmStatus;
    private string _productStatus = "OK";
    private int _productTime;
    private object? _contentView;
    private bool _hartBeat;
    private string? _pcScan;
    private bool _pcScanDone;
    private int _cycleTime;
    private int _lastCycleTime;

    #endregion

    #region properties


    private ObservableCollection<StatusItem> _statusItems = new()
    {
        new StatusItem { Name = "PLC", Status = Brushes.Gray },
        new StatusItem { Name = "MES", Status = Brushes.Red },
        new StatusItem { Name = "OPC", Status = Brushes.Gray },
        new StatusItem { Name = "TCP", Status = Brushes.Gray }
    };

    public ObservableCollection<StatusItem> StatusItems
    {
        get => _statusItems;
        set => SetProperty(ref _statusItems, value);
    }
    
    



    


    /// <summary>
    /// 产品状态
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.产品状态")]
    public string ProductStatus
    {
        get => _productStatus;
        set => SetProperty(ref _productStatus, value);
    }
    
    


    /// <summary>
    /// 标题
    /// </summary>
    public string? Title
    {
        get => _title;
        private set => SetProperty(ref _title, value);
    }

    /// <summary>
    /// 版本
    /// </summary>

    public string? Version
    {
        get => _version;
        private set => SetProperty(ref _version, value);
    }

    /// <summary>
    /// 生产时间
    /// </summary>
    public int ProductTime
    {
        get => _productTime;
        set => SetProperty(ref _productTime, value);
    }

    /// <summary>
    /// 产品条码
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.产品条码")]
    public string ProductBarcode
    {
        get => _productBarcode;
        set => SetProperty(ref _productBarcode, value);
    }

    /// <summary>
    /// 当前用户名
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.作业员")]
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref _userName, value);
    }

    /// <summary>
    /// 模式
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.模式")]
    public string Mode
    {
        get => _mode;
        set => SetProperty(ref _mode, value);
    }

    /// <summary>
    /// 产品编码
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.产品编码")]
    public string ProductId
    {
        get => _productId;
        set => SetProperty(ref _productId, value);
    }

    /// <summary>
    /// 产品名称
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.产品名称")]
    public string ProductName
    {
        get => _productName;
        set => SetProperty(ref _productName, value);
    }


    /// <summary>
    /// 产品代码
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.产品代码")]
    public string ProductCode
    {
        get => _productCode;
        set => SetProperty(ref _productCode, value);
    }

    /// <summary>
    /// 工装绑定
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.工装绑定")]
    public string FixtureBinding
    {
        get => _fixtureBinding;
        set => SetProperty(ref _fixtureBinding, value);
    }

    /// <summary>
    /// 公司
    /// </summary>
    private string? _company;

    private readonly RootConfig _config;

    public string? Company
    {
        get => _company;
        set => SetProperty(ref _company, value);
    }

    /// <summary>
    /// 联系电话
    /// </summary>

    public string? PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }

    /// <summary>
    /// 运行状态
    /// </summary>
    ///
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.运行状态")]
    public string RunningStatus
    {
        get => _runningStatus;
        set => SetProperty(ref _runningStatus, value);
    }

    /// <summary>
    /// 操作提示
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.操作提示")]
    public string OperationPrompt
    {
        get => _operationPrompt;
        set => SetProperty(ref _operationPrompt, value);
    }

    /// <summary>
    /// 总次数
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.总次数")]
    public int TotalCount
    {
        get => _totalCount;
        set => SetProperty(ref _totalCount, value);
    }

    /// <summary>
    /// 保养计数
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.保养计数")]
    public int Maintenance
    {
        get => _maintenance;
        set => SetProperty(ref _maintenance, value);
    }

    /// <summary>
    /// 工单数量
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.工单数量")]
    public int WorkOderQty
    {
        get => _workOderQty;
        set => SetProperty(ref _workOderQty, value);
    }


    /// <summary>
    /// 心跳， OPC库
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.心跳")]
    public bool HeartBeat
    {
        get => _hartBeat;
        set => SetProperty(ref _hartBeat, value);
    }

    /// <summary>
    /// 完成数
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.完成数")]

    public int CompleteQty
    {
        get => _completeQty;
        set
        {
            SetProperty(ref _completeQty, value);
            OnPropertyChanged(nameof(YieldRate));
        }
    }

    [MonitoredItem(nodeId: "ns=4;s=MES_交互.周期时间")]
    public int CycleTime
    {
        get => _cycleTime;
        set { SetProperty(ref _cycleTime, value); }
    }

    [MonitoredItem(nodeId: "ns=4;s=MES_交互.上一次周期")]
    public int LastCycleTime
    {
        get => _lastCycleTime;
        set => SetProperty(ref _lastCycleTime, value);
    }


    /// <summary>
    /// 合格数
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.合格数")]
    public int OkQty
    {
        get => _okQty;
        set
        {
            SetProperty(ref _okQty, value);
            OnPropertyChanged(nameof(YieldRate));
        }
    }

    public object? ContentView
    {
        get => _contentView;
        private set => SetProperty(ref _contentView, value);
    }

    /// <summary>
    /// 合格率
    /// </summary>
    public string YieldRate => _completeQty > 0 ? (_okQty * 1.0 / _completeQty).ToString("P2") : 0.ToString("P2");

    /// <summary>
    /// 当前时间
    /// </summary>

    public string CurrentTIme => DateTime.Now.ToString("G");

    /// <summary>
    /// 测试项目
    /// </summary>
    public ObservableCollection<TestItem> TestItems { get; } = new();

    public ObservableCollection<TestLog> TestLogs { get; set; } = new();

    #endregion


    #region commands

    [RelayCommand]
    private void Exit()
    {
        //添加提醒
        var result = MessageBox.Show("是否退出程序？", "提示", MessageBoxButton.YesNo);
        if (result == MessageBoxResult.Yes)
            Application.Current.Shutdown();
    }

    #endregion
}

public partial class StatusItem : ObservableObject
{
    [ObservableProperty] private string? _name;
    [ObservableProperty] private Brush? _status;
}