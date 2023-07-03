using CommunityToolkit.Mvvm.Input;
using Workstation.ServiceModel.Ua;

namespace 比亚迪AGS_WPF.ViewModels;

[Subscription(endpointUrl: "MainPLC", publishingInterval: 500, keepAliveCount: 20)]
public partial class ScannerViewModel : SubscriptionBase
{
    private string _pcScan;
    private bool _pcScanDone;

    /// <summary>
    /// MES_交互.PC扫码
    /// </summary>

    [MonitoredItem(nodeId: "ns=4;s=MES_交互.PC扫码")]
    public string PcScan
    {
        get => _pcScan;
        set
        {
            SetProperty(ref _pcScan, value);
            PcScanDone = true;
        }
    }

    [MonitoredItem(nodeId: "ns=4;s=MES_交互.PC扫码完成")]
    public bool PcScanDone
    {
        get => _pcScanDone;
        set => SetProperty(ref _pcScanDone, value);
    }
    [RelayCommand]
    private void ScanDone()
    {
        PcScanDone=true;
    }

}