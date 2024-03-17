using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OxyPlot;
using 比亚迪AGS_WPF.Config;
using 比亚迪AGS_WPF.DataObject;
using 比亚迪AGS_WPF.Utils;
using 比亚迪AGS_WPF.ViewModels;
using 比亚迪AGS_WPF.Views;
using MessageBox = HandyControl.Controls.MessageBox;

namespace 比亚迪AGS_WPF;

// [Subscription(endpointUrl: "MainPLC", publishingInterval: 500, keepAliveCount: 20)]
public partial class MainViewModel : ObservableRecipient,IRecipient<RootConfig>,IRecipient<ScriptResponse>,IRecipient<TestLog>
{
    public MainViewModel(RootConfig config)
    {
        _config = config;
        SetupTimer();
        InitializeProperties();

        // 初始化视图实例
        _views["TestLogView"] = new TestLogView();
        _views["ScriptsView"] = new ScriptsView();
        _views["ConfigView"] = new ConfigView();
        _views["DataView"] = new DataView();
        _views["UserView"] = new UserView();
        _views["PlotView"] =new UserOperationView();

        ContentView = _views["TestLogView"];
        IsActive=true;
        
    }


    private void InitializeProperties()
    {
        Title = _config.Title;
        Version = _config.Version;
        PhoneNumber = _config.PhoneNumber;
        Company = _config.Company;
        UserName = _config.CurrentUser;
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
    private readonly Dictionary<string, UserControl> _views = new();
    #region fields

    [ObservableProperty] private string? _title;
    [ObservableProperty] private string? _version;
    [ObservableProperty] private string _productBarcode = "产品条码";
    [ObservableProperty] private string _userName = "作业员";
    [ObservableProperty] private string _mode = "本地模式";
    [ObservableProperty] private string _productId = "产品编码";
    [ObservableProperty] private string _productCode = "产品代码";
    [ObservableProperty] private string _fixtureBinding = "01";
    [ObservableProperty] private string _productName = "产品1";
    [ObservableProperty] private string _runningStatus = "待机";
    [ObservableProperty] private string _operationPrompt = "操作提示";
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private int _maintenance;
    [ObservableProperty] private int _workOderQty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(YieldRate))]
    private int _completeQty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(YieldRate))]
    private int _okQty;

    [ObservableProperty] private string? _phoneNumber;
    [ObservableProperty] private string _productStatus = "OK";
    [ObservableProperty] private int _productTime;
    [ObservableProperty] private object? _contentView;
    [ObservableProperty] private bool _heartBeat;
    [ObservableProperty] private int _cycleTime;
    [ObservableProperty] private int _lastCycleTime;

    [ObservableProperty] private ObservableCollection<StatusItem> _statusItems = new()
    {
        new StatusItem { Name = "PLC", Status = false },
        new StatusItem { Name = "MES", Status = false }
    };

    [ObservableProperty] private string? _company;

    private readonly RootConfig _config;

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
        {
            // 退出程序
            Application.Current.Shutdown();
            
        }
    }

    [RelayCommand]
    private void Open(string obj)
    {
        switch (obj)
        {
            case "开始运行":
                ContentView = _views["TestLogView"];
                // 提示是否开始测试
                var result = MessageBox.Show("是否开始测试？", "提示", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Messenger.Send(new ScriptRequestMessage
                    {
                        Topic = "Measure",
                        Function = "main",
                        Path = _config.CurrentScript,
                    });
                }
                break;
            case "停止":
                ContentView = new StationCollectionView();
                break;
            case "脚本编辑":
                ContentView = _views["ScriptsView"];
                break;
            case "配置信息":
                ContentView = _views["ConfigView"];
                break;
            case "数据查询":
                ContentView =new DataView();
                break;
            case "用户变更":
                ContentView = _views["UserView"];
                break;
            case "用户操作":
                ContentView = _views["PlotView"];
                break;
        }
    }

    #endregion
    

    public void Receive(RootConfig message)
    {
        _config.Title = message.Title;
        _config.Version = message.Version;
        _config.PhoneNumber = message.PhoneNumber;
        _config.Company = message.Company;
        InitializeProperties();
    }

    public void Receive(ScriptResponse message)
    {
        if (message.Topic == "Measure")
        {
            // 使用ui线程更新
            Application.Current.Dispatcher.Invoke(() =>
            {
                var measure = message.Result.Cask<Measure>();
                if (measure == null) return;
                ProductStatus=measure.Result;
                OkQty += measure.Result == "OK" ? 1 : 0;
                CompleteQty += 1;
            });
        };
    }

    public void Receive(TestLog message)
    {
        RunningStatus = message.Log;
    }
}