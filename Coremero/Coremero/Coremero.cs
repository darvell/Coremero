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

            _container = new Container();
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
                from file in new DirectoryInfo(PlatformServices.Default.Application.ApplicationBasePath).GetFiles()
                where file.Extension.ToLower() == ".dll" && file.Name.StartsWith("Coremero.Client.")
                select loader.LoadFromAssemblyPath(file.FullName);
            _container.RegisterCollection<IClient>(clientAssemblies);

            // Scan for plugins
            if (Directory.Exists(PathExtensions.PluginDir))
            {
                var pluginAssemblies =
                    from file in new DirectoryInfo(PathExtensions.PluginDir).GetFiles()
                    where file.Extension.ToLower() == ".dll" && file.Name.StartsWith("Coremero.Plugin.")
                    select loader.LoadFromAssemblyPath(file.FullName);

                _container.RegisterCollection<IPlugin>(pluginAssemblies);
            }

            _container.Verify();
            
            // TODO: Allow host application to control this.
            Debug.WriteLine("Connecting all clients.");
            foreach (IClient client in _container.GetAllInstances<IClient>())
            {
                client.Connect();
            }

            Debug.WriteLine("Loading all plugins.");
            CommandRegistry cmdRegistry = _container.GetInstance<CommandRegistry>();

            try
            {
                foreach (IPlugin plugin in _container.GetAllInstances<IPlugin>())
                {
                    cmdRegistry.Register(plugin);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
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
