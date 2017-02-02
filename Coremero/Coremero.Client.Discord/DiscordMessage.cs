using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Rest;
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

        public async Task React(string emoji)
        {
            var restMessage = (RestUserMessage) await _message.Channel.GetMessageAsync(_message.Id);
            await restMessage.AddReactionAsync(emoji);
        }

        public async Task<List<string>> GetReactions()
        {
            var restMessage = (RestUserMessage) await _message.Channel.GetMessageAsync(_message.Id);
            List<string> result = new List<string>();
            foreach (KeyValuePair<Emoji, int> reaction in restMessage.Reactions)
            {
                for (int i = 0; i < reaction.Value; i++)
                {
                    result.Add(reaction.Key.ToString());
                }
            }
            return result;
        }
    }
}
