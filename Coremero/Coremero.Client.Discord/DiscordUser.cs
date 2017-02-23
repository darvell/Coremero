using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rpc;
using IAttachment = Coremero.Attachments.IAttachment;
using IMessage = Coremero.Messages.IMessage;

#pragma warning disable 4014

namespace Coremero.Client.Discord
{
    public class DiscordUser : IUser, IEntity
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
                    try
                    {
                        await dmChannel.SendFileAsync(attachment.Contents, attachment.Name,
                            message.Attachments?.Count == 1 ? message.Text : null);
                        attachment.Contents?.Dispose();
                    }
                    finally
                    {
                        attachment.Contents?.Dispose();
                    }
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
                    return ((IGuildUser) _user).Nickname ?? _user.Username;
                }
                return _user.Username;
            }
        }

        public string Username
        {
            get { return _user.Username; }
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
                        if (guildUser.GuildPermissions.Administrator ||
                            guildUser.RoleIds.Any(x => x.Equals(109078556360863744)))
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

        public ulong ID
        {
            get { return _user.Id; }
        }
    }
}