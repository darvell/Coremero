using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
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
                    attachment.Contents?.Dispose();
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
            get
            {
                if (_user is IGuildUser)
                {
                    return ((IGuildUser)_user).Nickname ?? _user.Username;
                }
                return _user.Username;
            }
        }

        public string Mention
        {
            get { return _user.Mention; }
        }

        public UserPermission Permissions
        {
            get
            {
                if (_user is IGuildUser)
                {
                    // TODO: Move to configuration
                    // HARDCODE SA-MINECRAFT FOR NOW
                    var guildUser = (IGuildUser) _user;

                    if (guildUser.Guild.Id == 109063664560009216)
                    {
                        if (guildUser.GuildPermissions.Administrator)
                        {
                            return UserPermission.BotOwner;
                        }
                    }

                    return guildUser.GuildPermissions.Administrator
                        ? UserPermission.Admin
                        : UserPermission.Normal;
                }
                return UserPermission.Normal;
            }
        }
    }
}
