using System;
using System.Collections.Generic;
using System.Text;
using Coremero.Context;
using Discord.WebSocket;

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

        private IUser _user;

        public IUser User
        {
            get
            {
                if (_user == null)
                {
                    _user = DiscordFactory.UserFactory.Get(_message.Author);
                }
                return _user;
            }
        }

        private IChannel _channel;

        public IChannel Channel
        {
            get
            {
                if (_channel == null)
                {
                    _channel = DiscordFactory.ChannelFactory.Get((ISocketMessageChannel) _message.Channel);
                }
                return _channel;
            }
        }

        private global::Discord.IMessage _message; // Discord holds a lot of invocation context in here, great.

        public DiscordInvocationContext(IClient client, global::Discord.IMessage message)
        {
            OriginClient = client;
            _message = message;
        }
    }
}