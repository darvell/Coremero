using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Coremero.Client;

namespace Coremero.Plugin.Playground
{
    public class Debug : IPlugin, IDisposable
    {
        private readonly IClientUserStatus _client;
        private readonly Timer _timer;

        public Debug(IEnumerable<IClient> clients)
        {
            _client = clients.FirstOrDefault(x => x is IClientUserStatus) as IClientUserStatus;
            _timer = new Timer((state) =>
            {
                if (_client != null)
                {
                    _client.UserStatus = $"GC: {(GC.GetTotalMemory(false) / 1024.0 / 1024.0):##.##}Mb";
                }
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}