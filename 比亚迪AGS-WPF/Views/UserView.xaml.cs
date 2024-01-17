using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using 比亚迪AGS_WPF.ViewModels;

namespace 比亚迪AGS_WPF.Views;

public partial class UserView : UserControl
{
    public UserView()
    {
        InitializeComponent();
        this.DataContext = App.Current.Services.GetService<UserViewModel>();
    }
}