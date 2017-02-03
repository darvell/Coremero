using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;

namespace Coremero.Client.Discord
{
    public class DiscordChannel : IChannel
    {
        private IMessageChannel _channel;

        public DiscordChannel(IMessageChannel channel)
        {
            _channel = channel;
        }

        public async Task SendAsync(IMessage message)
        {
            if (message.Attachments?.Count > 0)
            {
                foreach (IAttachment attachment in message.Attachments)
                {
                    await _channel.SendFileAsync(attachment.Contents, attachment.Name, message.Attachments?.Count == 1 ? message.Text : null);
                }
            }
            else
            {
                await _channel.SendMessageAsync(message.Text);
            }
        }

        public void Send(IMessage message)
        {
            SendAsync(message);
        }

        public string Name
        {
            get { return _channel.Name; }
        }

        public string Topic
        {
            get
            {
                return String.Empty; // Channel interface has no topic? Do we need to cast?
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
