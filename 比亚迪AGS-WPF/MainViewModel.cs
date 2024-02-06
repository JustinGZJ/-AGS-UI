using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Newtonsoft.Json;
using Python.Runtime;
using 比亚迪AGS_WPF.Config;
using 比亚迪AGS_WPF.Services;
using 比亚迪AGS_WPF.ViewModels;
using 比亚迪AGS_WPF.Views;
using MessageBox = HandyControl.Controls.MessageBox;

namespace 比亚迪AGS_WPF;
public static class PyObjectExtensions
{
    public static T? Cask<T>(this PyObject pyObject)
    {
        return JsonConvert.DeserializeObject<T>(pyObject.ToString());
    }
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

    public MainViewModel(RootConfig config, IScriptExecutor scriptExecutor)
    {
        _config = config;
        _scriptExecutor = scriptExecutor;
        SetupTimer();
        InitializeProperties();
        ContentView = new TestLogView();
    }


    private void InitializeProperties()
    {
        Title = _config.Title;
        Version = _config.Version;
        PhoneNumber = _config.PhoneNumber;
        Company = _config.Company;
        // 解构赋值
        ;
    }

    private void SetupTimer()
    {
        DispatcherTimer timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(500),
        };
        timer.Tick += (_, _) => { OnPropertyChanged(nameof(CurrentTIme)); };
        timer.Start();
    }

    #region fields
    [ObservableProperty]
    private string? _title;
    [ObservableProperty]
    private string? _version;
    [ObservableProperty]
    private string _productBarcode = "产品条码";
    [ObservableProperty]
    private string _userName = "作业员";
    [ObservableProperty]
    private string _mode = "本地模式";
    [ObservableProperty]
    private string _productId = "产品编码";
    [ObservableProperty]
    private string _productCode = "产品代码";
    [ObservableProperty]
    private string _fixtureBinding = "01";
    [ObservableProperty]
    private string _productName = "产品1";
    [ObservableProperty]
    private string _runningStatus = "待机";
    [ObservableProperty]
    private string _operationPrompt = "操作提示";
    [ObservableProperty]
    private int _totalCount;
    [ObservableProperty]
    private int _maintenance;
    [ObservableProperty]
    private int _workOderQty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(YieldRate))]
    private int _completeQty;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(YieldRate))]
    private int _okQty;
    [ObservableProperty]
    private string? _phoneNumber;
    [ObservableProperty]
    private string _productStatus = "OK";
    [ObservableProperty]
    private int _productTime;
    [ObservableProperty]
    private object? _contentView;
    [ObservableProperty]
    private bool _heartBeat;
    [ObservableProperty]
    private int _cycleTime;
    [ObservableProperty]
    private int _lastCycleTime;
    
    [ObservableProperty]
    private ObservableCollection<StatusItem> _statusItems = new()
    {
        new StatusItem { Name = "PLC", Status = false },
        new StatusItem { Name = "MES", Status = false }
    };
    
    [ObservableProperty]
    private string? _company;
    
    private readonly RootConfig _config;
    private readonly IScriptExecutor _scriptExecutor;
    
    public string YieldRate => CompleteQty > 0 ? (OkQty * 1.0 / CompleteQty).ToString("P2") : 0.ToString("P2");

    /// <summary>
    /// 当前时间
    /// </summary>

    public string CurrentTIme => DateTime.Now.ToString("G");


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
        
    [RelayCommand]
    private void Open(string obj)
    {
        switch (obj)
        {
            case "开始运行":
                ContentView = new TestLogView();
                // 提示是否开始测试
                var result = MessageBox.Show("是否开始测试？", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    // 开始测试
                    WeakReferenceMessenger.Default.Send(new TestLog
                    {
                        Type = "系统",
                        Log = "开始测试",
                        Level = "Info"
                    });
                    PyObject data = _scriptExecutor.Execute("Scripts.Inst", "main");

                    var testItems = data.Cask<List<TestItem>>();
                    
                    WeakReferenceMessenger.Default.Send(new TestItemsChangeMessage(testItems));

                    WeakReferenceMessenger.Default.Send(new TestLog
                    {
                        Type = "系统",
                        Log = data.ToString(),
                        Level = "Info"
                    });
                }

                break;
            case "停止":
                ContentView = new TestLogView();
                break;
            case "监视页面":
                ContentView = new ScriptsView();
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
                ContentView = new PlotView();
                break;
        }
    }


    #endregion
}

public partial class StatusItem : ObservableObject
{
    [ObservableProperty] private string? _name;
    [ObservableProperty] private bool? _status;
}