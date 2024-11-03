using Microsoft.Extensions.DependencyInjection;
using Osmuim.GUI.Services;
using System;
using System.Windows;

namespace Osmuim.GUI
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            // Create the MainWindow using DI
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register services as singletons
            services.AddSingleton<ChromeDataService>();
            // Add additional services here as needed
            services.AddSingleton<MainWindow>();  // Register MainWindow with DI
        }
    }
}
