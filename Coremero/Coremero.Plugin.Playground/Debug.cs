using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Coremero.Client;

namespace Coremero.Plugin.Playground
{
    public class Debug : IPlugin
    {
        private readonly IClientUserStatus _client;
        private long _minutesAlive = 0;
        public Debug(IEnumerable<IClient> clients)
        {
            _client = clients.FirstOrDefault(x => x is IClientUserStatus) as IClientUserStatus;

            var timer = new Timer((state) =>
            {
                _minutesAlive += 1;
                if (_client != null)
                {
                    _client.UserStatus = $"U: {_minutesAlive}m | A: {GC.GetTotalMemory(false) / 1024.0}MB ";
                }
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
        }

    }
}
