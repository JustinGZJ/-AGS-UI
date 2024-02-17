using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using 比亚迪AGS_WPF.Config;
using 比亚迪AGS_WPF.Services;

namespace 比亚迪AGS_WPF.ViewModels;

public partial class ConfigViewModel : ObservableRecipient
{
    public RootConfig? Config { get; }
    readonly string _fileName = AppPath.AppSettingsPath;

    public ConfigViewModel(RootConfig config)
    {
        Config =config;
    }

    [RelayCommand]
    private void Save()
    {
        ConfigHelper.SaveConfig(Config, _fileName);
        if (Config != null) Messenger.Send(Config);

        HandyControl.Controls.MessageBox.Success("保存成功", "提示");
        // 通知其他页面配置已经更新
        
    }
    
    
}