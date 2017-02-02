using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Coremero.Client;
using Coremero.Registry;
using Coremero.Services;
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

            // Register services
            _container.RegisterSingleton<IMessageBus, MessageBus>();
            _container.RegisterSingleton<ICommandHandler, CommandHandler>();

            // Scan for clients
            var clientAssemblies =
                from file in new DirectoryInfo(PlatformServices.Default.Application.ApplicationBasePath).GetFiles()
                where file.Extension.ToLower() == ".dll" && file.Name.StartsWith("Coremero.Client.")
                select AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);
            _container.RegisterCollection<IClient>(clientAssemblies);

            // Scan for plugins
            string pluginDirectory = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Plugins");
            var pluginAssemblies = 
                from file in new DirectoryInfo(pluginDirectory).GetFiles()
                where file.Extension.ToLower() == ".dll" && file.Name.StartsWith("Coremero.Plugin.")
                select AssemblyLoadContext.Default.LoadFromAssemblyPath(file.FullName);

            _container.RegisterCollection<IPlugin>(pluginAssemblies);

            _container.Verify();            

            _hasInit = true;
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
