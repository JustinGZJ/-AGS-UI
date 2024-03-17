using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using 比亚迪AGS_WPF.Config;
using 比亚迪AGS_WPF.ViewModels;

namespace 比亚迪AGS_WPF.Services;

public partial class UserOperationViewModel:ObservableRecipient
{
    private readonly RootConfig _config;

    public UserOperationViewModel(RootConfig config)
    {
        _config = config;
        IsActive = true;
    }
    [RelayCommand]
    private void Calibration(string mode)
    {
        // 提示用户校准
      var result = HandyControl.Controls.MessageBox.Show("请确认校准件已经放到治具中...", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information);
      if (result == MessageBoxResult.OK)
      {
          Messenger.Send(new ScriptRequestMessage
          {
              Topic = "Calibration",
              Function = "calibration",
              Path = _config.CurrentScript,
              Params = new[] {mode}
          });
      }
    }
}