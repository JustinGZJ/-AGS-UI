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
using 比亚迪AGS_WPF.BydMes;
using 比亚迪AGS_WPF.Services;
using 比亚迪AGS_WPF.ViewModels;
using Newtonsoft.Json;

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


            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App) Application.Current;

        public UaApplication OpcApplication { get; private set; }
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
                .WriteTo.File("logs\\log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Build and run an OPC UA application instance.
            OpcApplication = new UaApplicationBuilder()
                .SetApplicationUri($"urn:{Dns.GetHostName()}:Workstation.StatusHmi")
                .AddMappedEndpoints(Config)
                .Build();
            OpcApplication.Run();

            Services.GetService<TcpServerService>();
            Services.GetService<MesService>();


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
            services.AddSingleton<IConfiguration>(Config);
            services.AddOptions<BydMesConfig>().Bind(Config.GetSection("BydMesConfig"));
            services.AddSingleton<TcpServerService>();
            services.AddTransient<BydMesCom>();
            services.AddSingleton<MesService>();
            // viewmodels
            services.AddTransient<MainViewModel>();
            services.AddTransient<ConfigViewModel>(); 
            services.AddTransient<EnquireViewModel>();
            //     services.AddSingleton<TcpServer>();

            return services.BuildServiceProvider();
        }
    }
}