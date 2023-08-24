using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Prism.Commands;
using Workstation.ServiceModel.Ua;
using 比亚迪AGS_WPF.Services;
using 比亚迪AGS_WPF.ViewModels;
using 比亚迪AGS_WPF.Views;

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

[Subscription(endpointUrl: "MainPLC", publishingInterval: 500, keepAliveCount: 20)]
public partial class MainViewModel : SubscriptionBase
{
    public DelegateCommand<string> _ConfigDialog;

    public DelegateCommand<string> ConfigDialog =>
        _ConfigDialog ??= new DelegateCommand<string>(Config_Dialog); //用来打开添加数据库各种模块DeleteProject

    public DelegateCommand<string> _EnquireDialog;

    public DelegateCommand<string> EnquireDialog =>
        _EnquireDialog ??= new DelegateCommand<string>(Enquire_Dialog); //用来打开添加数据库各种模块DeleteProject

    public DelegateCommand<string> _OpenCommand;

    public DelegateCommand<string> OpenCommand =>
        _OpenCommand ??= new DelegateCommand<string>(UiChange); //用来打开添加数据库各种模块DeleteProject

    public MainViewModel()
    {
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
            HeartBeat = !HeartBeat;
            // TcpStatus
        });
        UiChange("TestLogView");
        timer.Start();
        WeakReferenceMessenger.Default.Register<TcpStatusMessage>(this,
            (r, m) => { TcpStatus = m.Value ? Brushes.Chartreuse : Brushes.Red; });

        WeakReferenceMessenger.Default.Register<DataUploadMessage>(this, ((recipient, message) =>
        {
            var testItems = message.Value.Split('!').Where(x => x.Contains(",")).Select(x =>
            {
                var m = x.Split(',');
                return new TestItem()
                {
                    Name = m[0],
                    Parameter = m[1],
                    Value = m[2],
                    Result = message.Result
                };
            });
            // 保存文件
            SaveFile(message, testItems);

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
            var log = new TestLog()
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


    private static void SaveFile(DataUploadMessage message, IEnumerable<TestItem> testItems)
    {
        // 保存文件
        var fileName = AppDomain.CurrentDomain.BaseDirectory + "Data\\" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
        // 检查目录是否存在,+ DateTime.Now.ToString("yyyy-MM") +"\\"
        if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + "Data\\"))
        {
            Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "Data\\");
        }

        // 把数据放如dictionary
        var dictionary = new Dictionary<string, string>();
        dictionary.Add("时间", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        dictionary.Add("站位", message.Station);
        dictionary.Add("识别码",message.Code);
        dictionary.Add("结果", message.Result);
        foreach (var item in testItems)
        {
            dictionary.TryAdd(item.Name, item.Value.Trim());
        }

        // 把dictionary内的数据追加到文件，如果文件不存在则追加表头
        //检查文件是否存在
        var fileExists = File.Exists(fileName);
        var header = string.Join(",", dictionary.Keys);
        //添加表头到文件
        if (!fileExists)
        {
            File.WriteAllText(fileName, header + Environment.NewLine);
        }

        //将数据追加到文件
        File.AppendAllText(fileName, string.Join(",", dictionary.Values) + Environment.NewLine);
    }

    private void Config_Dialog(string obj)
    {
        //DialogParameters keys = new DialogParameters();
        //keys.Add("Title", SelectedItems);
        //dialogService.ShowDialog(obj);
    }

    private void Enquire_Dialog(string obj)
    {
        //   EnquireView popup = new EnquireView();
        //   popup.ShowDialog();        
    }

    private void UiChange(string obj)
    {
        switch (obj)
        {
            case "TestLogView":
                Body = new TestLogView();
                break; //ConfigView
            case "UserView":
                Body = new UserView();
                break;
            case "ConfigView":
                Body = new ConfigView();
                break;
            case "EnquireView":
                Body = new EnquireView();
                break;
            case "ScannerView":
                Body = new ScannerView();
                break;
        }
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
    private int _totalCount = 0;
    private int _maintenance;
    private int _workOderQty;

    private int _completeQty;
    private int _okQty;
    private string? _phoneNumber;
    private Brush _tcpStatus = Brushes.Gray;
    private bool _robotStatus;
    private bool _alarmStatus;
    private string _productStatus="OK";
    private int _productTime;
    private Object body;
    private bool _hartBeat;
    private string _pcScan;
    private bool _pcScanDone;

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
    /// PC扫码
    /// </summary>

    [MonitoredItem(nodeId: "ns=4;s=MES_交互.PC扫码")]
    public string PCScan
    {
        get => _pcScan;
        set => SetProperty(ref _pcScan, value);
    }

    /// <summary>
    /// PC扫码完成
    /// </summary>

    [MonitoredItem(nodeId: "ns=4;s=MES_交互.PC扫码完成")]
    public bool PCScanDone
    {
        get => _pcScanDone;
        set => SetProperty(ref _pcScanDone, value);
    }


    /// <summary>
    /// 机器人状态
    /// </summary>

    [MonitoredItem(nodeId: "ns=4;s=MES_交互.机器人")]
    public bool RobotStatus
    {
        get => _robotStatus;
        set => SetProperty(ref _robotStatus, value);
    }


    /// <summary>
    /// 报警状态
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.报警")]
    public bool AlarmStatus
    {
        get => _alarmStatus;
        set => SetProperty(ref _alarmStatus, value);
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

    [RelayCommand]
    public void ScanDone()
    {
        this.PCScanDone = !this.PCScanDone;
    }


    /// <summary>
    /// TCP状态
    /// </summary>
    public Brush TcpStatus
    {
        get { return _tcpStatus; }
        set { SetProperty(ref _tcpStatus, value); }
    }


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
    /// 生产时间
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.生产时间")]
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
        set => SetProperty(ref this._productBarcode, value);
    }

    /// <summary>
    /// 当前用户名
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.作业员")]
    public string UserName
    {
        get => _userName;
        set => SetProperty(ref this._userName, value);
    }

    /// <summary>
    /// 模式
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.模式")]
    public string Mode
    {
        get => _mode;
        set => SetProperty(ref this._mode, value);
    }

    /// <summary>
    /// 产品编码
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.产品编码")]
    public string ProductId
    {
        get => _productId;
        set => SetProperty(ref this._productId, value);
    }

    /// <summary>
    /// 产品名称
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.产品名称")]
    public string ProductName
    {
        get => _productName;
        set => SetProperty(ref this._productName, value);
    }


    /// <summary>
    /// 产品代码
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.产品代码")]
    public string ProductCode
    {
        get => _productCode;
        set => SetProperty(ref this._productCode, value);
    }

    /// <summary>
    /// 工装绑定
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.工装绑定")]
    public string FixtureBinding
    {
        get => _fixtureBinding;
        set => SetProperty(ref this._fixtureBinding, value);
    }

    /// <summary>
    /// 联系电话
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
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.运行状态")]
    public string RunningStatus
    {
        get => _runningStatus;
        set => SetProperty(ref this._runningStatus, value);
    }

    /// <summary>
    /// 操作提示
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.操作提示")]
    public string OperationPrompt
    {
        get => _operationPrompt;
        set => SetProperty(ref this._operationPrompt, value);
    }

    /// <summary>
    /// 总次数
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.总次数")]
    public int TotalCount
    {
        get => _totalCount;
        set => SetProperty(ref this._totalCount, value);
    }

    /// <summary>
    /// 保养计数
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.保养计数")]
    public int Maintenance
    {
        get => _maintenance;
        set => SetProperty(ref this._maintenance, value);
    }

    /// <summary>
    /// 工单数量
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.工单数量")]
    public int WorkOderQty
    {
        get => _workOderQty;
        set => SetProperty(ref this._workOderQty, value);
    }


    /// <summary>
    /// 心跳， OPC库
    /// </summary>
    [MonitoredItem(nodeId: "ns=4;s=MES_交互.心跳")]
    public bool HeartBeat
    {
        get => _hartBeat;
        set => SetProperty(ref this._hartBeat, value);
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
            SetProperty(ref this._completeQty, value);
            NotifyPropertyChanged(nameof(YieldRate));
        }
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
            SetProperty(ref this._okQty, value);
            NotifyPropertyChanged(nameof(YieldRate));
        }
    }

    public Object Body
    {
        get => body;
        set { SetProperty(ref this.body, value); }
    }

    /// <summary>
    /// 合格率
    /// </summary>
    public string YieldRate => _totalCount > 0 ? (_okQty * 1.0 / _totalCount).ToString("P2") : 0.ToString("P2");

    /// <summary>
    /// 当前时间
    /// </summary>

    public string CurrentTIme => DateTime.Now.ToString("G");

    /// <summary>
    /// 测试项目
    /// </summary>
    public ObservableCollection<TestItem> TestItems { get; } = new()
    {
    };

    public ObservableCollection<TestLog> TestLogs { get; set; } = new();

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