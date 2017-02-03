using System;
using System.Collections.Generic;
using System.Text;

namespace Coremero.Client.Discord
{
    public class DiscordInvocationContext : IInvocationContext
    {
        public IClient OriginClient { get; private set; }

        public ISendable Raiser
        {
            get
            {
                if (_message.Channel.Name == _message.Author.Username)
                {
                    return User;
                }
                return Channel;
            }
        }

        public IUser User { get; }
        public IChannel Channel { get; }

        private global::Discord.IMessage _message; // Discord holds a lot of invocation context in here, great.
         
        public DiscordInvocationContext(IClient client, global::Discord.IMessage message)
        {
            OriginClient = client;
        }

    }
}
