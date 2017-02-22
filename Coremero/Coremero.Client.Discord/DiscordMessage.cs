using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coremero.Attachments;
using Coremero.Messages;
using Discord;
using Discord.Rest;
using Discord.Rpc;
using Discord.WebSocket;
using IAttachment = Coremero.Attachments.IAttachment;

namespace Coremero.Client.Discord
{
    public class DiscordMessage : IDeletableMessage, IReactableMessage, IBufferedMessage
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

        private List<IAttachment> _attachments;

        public List<IAttachment> Attachments
        {
            get
            {
                if (_attachments == null)
                {
                    _attachments =
                        _message.Attachments.Select(x => new UrlAttachment(x.Url)).Cast<IAttachment>().ToList();
                }
                return _attachments;
            }
        }

        public DateTime Timestamp
        {
            get { return _message.Timestamp.DateTime; }
        }


        public async Task React(string emoji)
        {
            if (emoji.StartsWith("<")) // I guess Discord.Net doesn't use the same methods internally?
            {
                await ((SocketUserMessage) _message).AddReactionAsync(Emoji.Parse(emoji));
            }
            else
            {
                await ((SocketUserMessage) _message).AddReactionAsync(emoji);
            }
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

        public async Task DeleteAsync()
        {
            await _message.DeleteAsync();
        }

        public IUser User
        {
            get { return DiscordFactory.UserFactory.Get(_message.Author); }
        }
    }
}