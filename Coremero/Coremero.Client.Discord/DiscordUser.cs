using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.Rpc;

namespace Coremero.Client.Discord
{
    public class DiscordUser : IUser
    {
        private global::Discord.IUser _user;

        public DiscordUser(global::Discord.IUser user)
        {
            _user = user;
        }

        public async Task SendAsync(IMessage message)
        {
            var dmChannel = await _user.GetDMChannelAsync();
            if (message.Attachments?.Count > 0)
            {
                foreach (IAttachment attachment in message.Attachments)
                {
                    await dmChannel.SendFileAsync(attachment.Contents, attachment.Name, message.Attachments?.Count == 1 ? message.Text : null);
                }
            }
            else
            {
                await dmChannel.SendMessageAsync(message.Text);
            }
        }

        public void Send(IMessage message)
        {
            // Fire-and-forget
            SendAsync(message);
        }

        public string Name
        {
            get { return _user.Username; }
        }

        public string Mention
        {
            get { return _user.Mention; }
        }
    }
}
