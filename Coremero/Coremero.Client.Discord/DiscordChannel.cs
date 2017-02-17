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
    public class DiscordChannel : IBufferedChannel, IChannelTypingIndicator
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
                        break;
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e, "Discord file send fail");
                        return;
                    }
                }
                message.Attachments.ForEach(x => x.Contents?.Dispose());
            }
            else
            {
                await _channel.SendMessageAsync(message.Text);
            }
            if(IsTyping)
            {
                SetTyping(false);
            }
        }

        public void Send(IMessage message)
        {
            #pragma warning disable 4014
            SendAsync(message);
            #pragma warning restore 4014
        }

        public string Name
        {
            get { return _channel.Name; }
        }

        public string Topic
        {
            get { return String.Empty; // Channel interface has no topic? Do we need to cast?
            }
        }

        public IEnumerable<IUser> Users
        {
            get
            {
                List<IUser> result = new List<IUser>();
                var enumerator = _channel.GetUsersAsync().GetEnumerator();
                while (enumerator.MoveNext().Result == true)
                {
                    foreach (var user in enumerator.Current)
                    {
                        result.Add(DiscordFactory.UserFactory.Get(user));
                    }
                }
                return result;
            }
        }

        private IDisposable _typingState = null;
        public bool IsTyping
        {
            get { return _typingState != null; }
        }

        public void SetTyping(bool isTyping)
        {
            if (isTyping)
            {
                _typingState = _channel.EnterTypingState();
            }
            else
            {
                if (_typingState != null)
                {
                    _typingState.Dispose();
                    _typingState = null;
                }
            }
        }

        public List<IBufferedMessage> GetLatestMessages(int limit = 100)
        {
            return GetLatestMessagesAsync(limit).Result;
        }

        public async Task<List<IBufferedMessage>> GetLatestMessagesAsync(int limit = 100)
        {
            List<IBufferedMessage> result = new List<IBufferedMessage>();
            var enumerator = _channel.GetMessagesAsync(limit);
            await enumerator.ForEachAsync(messages =>
            {
                foreach (var message in messages)
                {
                    result.Add(new DiscordMessage(message));
                }
            });
            return result;
        }

    }
}
