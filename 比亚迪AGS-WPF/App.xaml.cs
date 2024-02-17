using System;
using System.Text;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using 比亚迪AGS_WPF.Services;
using 比亚迪AGS_WPF.ViewModels;
using 比亚迪AGS_WPF.Config;

namespace 比亚迪AGS_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Services = ConfigureServices();
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                Log.CloseAndFlush();
            };
            this.InitializeComponent();
            
            
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // This method will be called when an unhandled exception occurs
            // You can add your own logic here, for example logging the exception
            Exception exception = (Exception)e.ExceptionObject;
            MessageBox.Show("Unhandled exception: " + exception.Message, "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
            Log.Error("Unhandled exception: " + exception.Message, exception);
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;

        public IConfigurationRoot Config { get; private set; }

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }


        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 20)
                .CreateLogger();

            var clientWrapper =Services.GetRequiredService<IMqttClientService>();
            clientWrapper.ConnectAsync();
            
            var scriptHub = Services.GetRequiredService<ScriptHubViewModel>();
            // Create and show the main view.
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            var config = ConfigHelper.LoadConfig<RootConfig>("./Config/appsettings.json");
            

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            });

            // viewmodels
            services.AddSingleton(config);
            services.AddSingleton<MainViewModel>();
            services.AddTransient<ConfigViewModel>();
            services.AddSingleton<TestLogViewModel>();
            services.AddTransient<UserViewModel>();
            services.AddTransient<ScannerViewModel>();
            
            services.AddTransient<PlotViewModel>();
            services.AddTransient<XyPlotViewModel>();
            services.AddTransient<ScriptsViewModel>();
            services.AddSingleton<ScriptHubViewModel>();
                       
            services.AddTransient<IScriptExecutor>(serviceProvider => new PythonExecutor(config.PythonPath));
            services.AddSingleton<IMqttClientWrapper>(serviceProvider => new MqttClientWrapper(config.MqttBrokerAddress, config.MqttBrokerPort));
            services.AddSingleton<IMqttClientService, MqttClientService>();
            
            return services.BuildServiceProvider();
        }
    }
}