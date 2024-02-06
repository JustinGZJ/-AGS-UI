using CommunityToolkit.Mvvm.ComponentModel;

namespace 比亚迪AGS_WPF.ViewModels;

public partial class TestItem:ObservableObject
{
    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private double? _lower;
    [ObservableProperty]
    private double? _upper;
    [ObservableProperty]
    private string? _value;
    [ObservableProperty]
    private string? _result;
    [ObservableProperty]
    private string? _unit;
}

