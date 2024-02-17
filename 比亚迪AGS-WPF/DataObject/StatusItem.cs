using CommunityToolkit.Mvvm.ComponentModel;

namespace 比亚迪AGS_WPF;

public partial class StatusItem : ObservableObject
{
    [ObservableProperty] private string? _name;
    [ObservableProperty] private bool? _status;
}