﻿using System.Linq;
using System.Threading.Tasks;
using Discord;
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

        public async Task<IMessage> SendAsync(IMessage message)
        {
            var dmChannel = await _user.GetOrCreateDMChannelAsync();
            if (message.Attachments?.Count > 0)
            {
                DiscordMessage result = null;
                foreach (IAttachment attachment in message.Attachments)
                {
                    try
                    {
                        result = new DiscordMessage(await dmChannel.SendFileAsync(attachment.Contents, attachment.Name,
                            message.Attachments?.Count == 1 ? message.Text : null));
                        attachment.Contents?.Dispose();
                    }
                    finally
                    {
                        attachment.Contents?.Dispose();
                    }
                }
                return result;
            }
            return new DiscordMessage(await dmChannel.SendMessageAsync(message.Text));
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
                if (_user is IGuildUser user)
                {
                    return user.Nickname ?? user.Username;
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
                if (_user is IGuildUser guildUser)
                {
                    // TODO: Move to configuration
                    // HARDCODE SA-MINECRAFT FOR NOW

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