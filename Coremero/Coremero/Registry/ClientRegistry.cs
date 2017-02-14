using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Coremero.Client;

namespace Coremero.Registry
{
    public class ClientRegistry
    {
        private readonly List<IClient> Clients = new List<IClient>();

        public void Register(IClient client)
        {
            if (Clients.Contains(client))
            {
                throw new NotSupportedException("Client already registered.");
            }

            Clients.Add(client);
        }

        public T Get<T>() where T : IClient
        {
            T result = (T) Clients.FirstOrDefault(x => x is T);

            if (result == null)
            {
                // TODO: TypeNotFoundException
                throw new TypeAccessException();
            }

            return result;
        }
    }
}