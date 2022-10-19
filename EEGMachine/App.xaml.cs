using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EEGMachine
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App : Application
    {
        private IConfiguration configuration;
        private ServiceProvider serviceProvider;

        private void OnStartup(object sender, StartupEventArgs e)
        {
            BuildConfiguration();

            ConfigureServices();

            ConfigureLogger();
        }

        private void ConfigureServices()
        {
            ServiceCollection services = new ServiceCollection();

            services.AddLogging();

            serviceProvider = services.BuildServiceProvider();
        }

        private void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();
        }

        private void ConfigureLogger()
        {
            // Initialize logger (serilog).
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            ILoggerFactory loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            loggerFactory.AddSerilog();

            // TODO: log version information.
            Log.Information("Starting up.");
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "Unhandled exception caught. Application will exit.");
        }
    }
}
