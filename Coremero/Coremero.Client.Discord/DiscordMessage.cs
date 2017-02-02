using System;
using System.Collections.Generic;
using System.Text;
using Discord.Rpc;

namespace Coremero.Client.Discord
{
    public class DiscordMessage : IMessage
    {
        private global::Discord.IMessage _message;

        public DiscordMessage(global::Discord.IMessage discordMessage)
        {
            _message = discordMessage;
        }

        public string Text
        {
            get { return _message.Content; }
        }

        public List<IAttachment> Attachments
        {
            get
            {
                // TODO: Convert attachments.
                return null;
            }
        }
    }
}
