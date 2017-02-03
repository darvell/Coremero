using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coremero.Client.Discord
{
    public class DiscordChannel : IChannel
    {
        private global::Discord.IChannel _channel;

        public DiscordChannel(global::Discord.IChannel channel)
        {
            _channel = channel;
        }

        public Task SendAsync(IMessage message)
        {
            throw new NotImplementedException();
        }

        public void Send(IMessage message)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return _channel.Name; }
        }

        public string Topic
        {
            get
            {
                return String.Empty; // IChannel has no topic? Do we need to cast?
            }
        }

        public IEnumerable<IUser> Users
        {
            get
            {
                /* TODO: Work out how async enumerables can work in a sync pattern.
                var users = _channel.GetUsersAsync().ForEachAsync(x =>
                {
                })
                */
                return null;
            }
        }
    }
}
