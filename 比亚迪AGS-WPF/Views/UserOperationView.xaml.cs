using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using 比亚迪AGS_WPF.Services;

namespace 比亚迪AGS_WPF.Views;

public partial class UserOperationView : UserControl
{
    public UserOperationView()
    {
        InitializeComponent();
        this.DataContext = App.Current.Services.GetService<UserOperationViewModel>();
    }
}