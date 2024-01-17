using Microsoft.Extensions.DependencyInjection;

namespace 比亚迪AGS_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow 
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext =App.Current.Services.GetService<MainViewModel>();
        }
    }
}