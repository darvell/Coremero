using System;
using System.Collections.Generic;
using System.Text;
using Coremero.Registry;
using Coremero.Services;
using SimpleInjector;

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

            // Register registries
            _container.RegisterSingleton<ClientRegistry>();
            _container.RegisterSingleton<CommandRegistry>();

            // Register services
            _container.RegisterSingleton<IMessageBus, MessageBus>();
            _container.RegisterSingleton<ICommandHandler, CommandHandler>();
            



            _container.Verify();            

            _hasInit = true;
        }

    }
}
