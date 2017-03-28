using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coremero.Client;
using Coremero.Plugin;
using Coremero.Registry;
using Coremero.Services;
using Coremero.Storage;
using Coremero.Utilities;
using Microsoft.Extensions.PlatformAbstractions;
using NLog;
using NLog.Config;
using NLog.Targets;
using SimpleInjector;

namespace Coremero.Console
{
    class Program
    {
        private static Container _container;

        static void Main(string[] args)
        {
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            var exitEvent = new ManualResetEvent(false);

            System.Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                exitEvent.Set();
            };

            MainAsync(args).GetAwaiter().GetResult();
            exitEvent.WaitOne();
            Log.Info("Exit called.");
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Log.Warn($"Unobserved task exception:\n{e.Exception.GetBaseException()}");
        }

        public static async Task MainAsync(string[] args)
        {
            // Log init.
            var loggingConfig = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget
            {
                Name = "console",
                Layout = @"[${date:format=HH\:mm\:ss}] ${message}"
            };

            var fileTarget = new FileTarget()
            {
                Name = "file",
                FileName = "${basedir}/coremero.log",
                Layout = @"[${date:format=yyyy-MM-dd HH\:mm\:ss}] ${message}",
            };

            loggingConfig.AddTarget(fileTarget);
            loggingConfig.AddTarget(consoleTarget);

#if DEBUG
            loggingConfig.AddRule(LogLevel.Trace, LogLevel.Fatal, consoleTarget);
#else
            loggingConfig.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);
#endif
            loggingConfig.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);
            LogManager.Configuration = loggingConfig;

            Log.Info("Coremero initializing.");

            // IoC setup
            _container = new Container
            {
                Options =
                {
                    LifestyleSelectionBehavior = new SingletonLifestyleSelectionBehavior()
                }
            };

            _container.ExpressionBuilt += (sender, arg) =>
            {
                Log.Trace($"Type {arg.RegisteredServiceType} registered.");
            };

            // Register registries
            _container.RegisterSingleton<ClientRegistry>();
            _container.RegisterSingleton<CommandRegistry>();
            _container.RegisterSingleton<ICredentialStorage, JsonCredentialStorage>();

            // Register services
            _container.RegisterSingleton<IMessageBus, MessageBus>();
            _container.RegisterSingleton<ICommandHandler, CommandHandler>();

            AssemblyLoader loader = new AssemblyLoader();

            // Scan for clients
            var clientAssemblies =
                new DirectoryInfo(PlatformServices.Default.Application.ApplicationBasePath).GetFiles()
                    .Where(file => file.Extension.ToLower() == ".dll" && file.Name.StartsWith("Coremero.Client."))
                    .Select(file => loader.LoadFromPath(file.FullName));
            _container.RegisterCollection<IClient>(clientAssemblies);

            // Scan for plugins
            if (Directory.Exists(PathExtensions.PluginDir))
            {
                var pluginAssemblies =
                    new DirectoryInfo(PathExtensions.PluginDir).GetFiles()
                        .Where(file => file.Extension.ToLower() == ".dll" && file.Name.StartsWith("Coremero.Plugin."))
                        .Select(file => loader.LoadFromPath(file.FullName));

                if (pluginAssemblies?.Any() == true)
                    _container.RegisterCollection<IPlugin>(pluginAssemblies);
            }

            _container.Verify();

            Log.Info("Connecting all clients.");
            foreach (IClient client in _container.GetAllInstances<IClient>())
            {
                try
                {
                    await client.Connect();
                    Log.Info($"Connected {client.Name}.");
                }
                catch (Exception e)
                {
                    Log.Exception(e.GetBaseException(), $"Failed to connect to {client.Name}");
                }
            }

            Log.Info("Loading all plugins.");
            CommandRegistry cmdRegistry = _container.GetInstance<CommandRegistry>();

            try
            {
                foreach (IPlugin plugin in _container.GetAllInstances<IPlugin>())
                {
                    try
                    {
                        cmdRegistry.Register(plugin);
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e, $"Failed to register ${plugin.GetType()} in to the command registry.");
                    }
                }
            }
            catch
            {
                // No plugins registered.
                Log.Warn("No plugins were registered.");
            }

            cmdRegistry.Register(_container.GetInstance<CorePlugin>());
        }
    }
}