using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coremero.Messages;
using Discord;
using Discord.WebSocket;

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
                    try
                    {
                        await _channel.SendFileAsync(attachment.Contents, attachment.Name,
                            message.Attachments?.Count == 1 ? message.Text : null);
                        attachment.Contents?.Dispose();
                    }
                    catch (Exception e)
                    {
                        attachment.Contents?.Dispose();
                        Debug.WriteLine(e);
                        return;
                    }
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
                List<IUser> result = new List<IUser>();
                var enumerator = _channel.GetUsersAsync().GetEnumerator();
                while(enumerator.MoveNext().Result == true)
                {
                    foreach (var user in enumerator.Current)
                    {
                        result.Add(new DiscordUser(user));
                    }
                }
                return result;
            }
        }
    }
}
