using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Coremero.Client;
using Coremero.Registry;
using Coremero.Services;
using Coremero.Storage;
using Coremero.Utilities;
using Microsoft.Extensions.PlatformAbstractions;
using NLog;
using NLog.Config;
using NLog.Targets;
using SimpleInjector;
using SimpleInjector.Advanced;

namespace Coremero
{
    public static class Coremero
    {
        private static bool _hasInit;
        private static Container _container;

        public static void Initialize()
        {
            if (_hasInit)
            {
                throw new InvalidOperationException();
            }

            // Log init.
            var loggingConfig = new LoggingConfiguration();

            var consoleTarget = new ColoredConsoleTarget { Name = "console", Layout = @"[${date:format=HH\:mm\:ss}] ${message}" };

            var fileTarget = new FileTarget()
            {
                Name = "file",
                FileName = "${basedir}/coremero.log",
                Layout = @"[${yyyy-MM-dd date:format=HH\:mm\:ss}] ${message}",
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
            _container = new Container();
            _container.ExpressionBuilt +=
                (sender, args) => { Log.Trace($"Type {args.RegisteredServiceType} registered."); };

            _container.Options.LifestyleSelectionBehavior = new SingletonLifestyleSelectionBehavior(); // Lazy hack to force all plugins to be singleton.

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

            // TODO: Allow host application to control this.
            Log.Info("Connecting all clients.");
            foreach (IClient client in _container.GetAllInstances<IClient>())
            {
                var clientTask = client.Connect();
                clientTask.Wait();
                if (clientTask.Exception != null)
                {
                    Log.Exception(clientTask.Exception.GetBaseException(), $"Failed to connect to {client.Name}");
                }
                else
                {
                    Log.Info($"Connected {client.Name}.");
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

            _hasInit = true;
        }

        public static IMessageBus GetMessageBus()
        {
            return _container.GetInstance<IMessageBus>();
        }

        public class SingletonLifestyleSelectionBehavior : ILifestyleSelectionBehavior
        {
            public Lifestyle SelectLifestyle(Type serviceType, Type implementationType)
            {
                return Lifestyle.Singleton;
            }
        }

    }
}
