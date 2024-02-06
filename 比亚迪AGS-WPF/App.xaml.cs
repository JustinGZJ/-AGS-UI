using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.Json;
using Serilog;
using 比亚迪AGS_WPF.Services;
using 比亚迪AGS_WPF.ViewModels;
using Newtonsoft.Json;
using 比亚迪AGS_WPF.Config;
using 比亚迪AGS_WPF.Views;

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
            // Create and show the main view.
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            var config = ConfigHelper.LoadConfig<RootConfig>(AppPath.AppSettingsPath);
            

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
                       
            services.AddTransient<IScriptExecutor>(serviceProvider => new PythonExecutor(config.PythonPath));
            // services.AddSingleton(new MqttClientWrapper(config.MqttBrokerAddress, config.MqttBrokerPort));
            services.AddSingleton<IMqttClientWrapper>(serviceProvider => new MqttClientWrapper(config.MqttBrokerAddress, config.MqttBrokerPort));
            services.AddSingleton<IMqttClientService, MqttClientService>();
            // services.AddSingleton(new PythonExecutor(config.PythonPath));
            return services.BuildServiceProvider();
        }
    }
}