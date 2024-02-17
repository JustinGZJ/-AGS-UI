using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using 比亚迪AGS_WPF.ViewModels;

namespace 比亚迪AGS_WPF.Views
{
    /// <summary>
    /// TestLogView.xaml 的交互逻辑
    /// </summary>
    public partial class TestLogView : UserControl
    {
        public TestLogView()
        {
            InitializeComponent();
            DataContext = App.Current.Services.GetService<TestLogViewModel>();
        }
    }
}
