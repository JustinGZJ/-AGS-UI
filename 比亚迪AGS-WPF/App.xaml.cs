using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Workstation.ServiceModel.Ua;

namespace 比亚迪AGS_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Services = ConfigureServices();

            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the current <see cref="App"/> instance in use
        /// </summary>
        public new static App Current => (App)Application.Current;
        private UaApplication application;

        /// <summary>
        /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
        /// </summary>
        public IServiceProvider Services { get; }
        
        
        
        protected override void OnStartup(StartupEventArgs e)
        {
            // Build and run an OPC UA application instance.
            this.application = new UaApplicationBuilder()
                .SetApplicationUri($"urn:{Dns.GetHostName()}:Workstation.StatusHmi")
                .SetDirectoryStore($"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Workstation.StatusHmi\\pki")
                // .SetIdentity(this.ShowSignInDialog)
                .Build();

            this.application.Run();

            // Create and show the main view.
        }

        /// <summary>
        /// Configures the services for the application.
        /// </summary>
        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            return services.BuildServiceProvider();
        }
    }
}