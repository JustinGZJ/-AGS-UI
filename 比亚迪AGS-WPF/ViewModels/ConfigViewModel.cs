using CommunityToolkit.Mvvm.Input;
using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using 比亚迪AGS_WPF.Config;
using 比亚迪AGS_WPF.Services;

namespace 比亚迪AGS_WPF.ViewModels;

public partial class ConfigViewModel : ObservableObject
{
    public RootConfig Config { get; }
    readonly string _fileName = Path.Combine(AppPath.ConfigPath, "AppSettings.json");

    public ConfigViewModel()
    {
        Config = ConfigHelper.LoadConfig<RootConfig>(_fileName);
    }

    [RelayCommand]
    public void Save()
    {
        ConfigHelper.SaveConfig(Config, _fileName);
        HandyControl.Controls.MessageBox.Success("保存成功", "提示");
    }
}