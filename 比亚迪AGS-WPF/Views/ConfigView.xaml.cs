using Microsoft.Extensions.DependencyInjection;
using 比亚迪AGS_WPF.ViewModels;

namespace 比亚迪AGS_WPF.Views
{
    /// <summary>
    /// ConfigView.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigView
    {
        public ConfigView()
        {
            InitializeComponent();
            this.DataContext = App.Current.Services.GetService<ConfigViewModel>();
        }
    }

}
