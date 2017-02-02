using System;
using System.Collections.Generic;
using System.Text;

namespace Coremero.Client.Discord
{
    public class DiscordInvocationContext : IInvocationContext
    {
        public IClient OriginClient { get; private set; }
        public ISendable Raiser { get; }
        public IUser User { get; }
        public IChannel Channel { get; }

        public DiscordInvocationContext(IClient client)
        {
            OriginClient = client;
        }

    }
}
