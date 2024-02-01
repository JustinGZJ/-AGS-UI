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
using Workstation.ServiceModel.Ua;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration.Json;
using Serilog;
using SimpleTCP;
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

         //   PropertyGridEditorHelper.RegisterEditor(typeof(List<>), typeof(MyCustomListEditor));
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App) Application.Current;
        
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
                .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day,retainedFileCountLimit:20)
                .CreateLogger();
            
            Services.GetService<TcpServerService>();


            // Create and show the main view.
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            // services
            Config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("AppSettings.json", true)
                .AddJsonFile("Users.json",true)
                .Build();


           
            services.AddSingleton<IConfiguration>(Config);

            services.AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            });
            
            services.AddSingleton<TcpServerService>();
            // viewmodels
            services.AddSingleton<MainViewModel>();
            services.AddTransient<ConfigViewModel>(); 
            services.AddTransient<EnquireViewModel>();
            services.AddSingleton<TestLogViewModel>();
            services.AddTransient<UserViewModel>();
            services.AddTransient<ScannerView>();
            services.AddSingleton(ConfigHelper.LoadConfig<RootConfig>( AppPath.AppSettingsPath));

            return services.BuildServiceProvider();
        }
    }
}