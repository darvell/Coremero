using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coremero.Messages;
using Discord;
using IAttachment = Coremero.Attachments.IAttachment;
using IMessage = Coremero.Messages.IMessage;

namespace Coremero.Client.Discord
{
    public class DiscordChannel : IBufferedChannel, IChannelTypingIndicator
    {
        private IMessageChannel _channel;

        public IMessageChannel RootObject => _channel;

        public DiscordChannel(IMessageChannel channel)
        {
            _channel = channel;
        }

        public async Task<IMessage> SendAsync(IMessage message)
        {
            IMessage result = null;
            if (message.Attachments?.Count > 0)
            {
                foreach (IAttachment attachment in message.Attachments)
                {
                    try
                    {
                        result = new DiscordMessage(await _channel.SendFileAsync(attachment.Contents, attachment.Name,
                            message.Attachments?.Count == 1 ? message.Text : null));
                        break;
                    }
                    catch (Exception e)
                    {
                        Log.Exception(e, "Discord file send fail");
                        return null;
                    }
                }
                message.Attachments.ForEach(x => x.Contents?.Dispose());
                IsTyping = false;
            }
            else
            {
                result = new DiscordMessage(await _channel.SendMessageAsync(message.Text));
                IsTyping = false;
            }

            return result;
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
            get
            {
                return string.Empty; // Channel interface has no topic? Do we need to cast?
            }
        }

        public IEnumerable<IUser> Users
        {
            get
            {
                List<IUser> result = new List<IUser>();
                var enumerator = _channel.GetUsersAsync().GetEnumerator();
                while (enumerator.MoveNext().Result)
                {
                    foreach (var user in enumerator.Current)
                    {
                        if (user.IsBot)
                        {
                            continue;
                        }

                        result.Add(DiscordFactory.UserFactory.Get(user));
                    }
                }
                return result;
            }
        }

        private readonly IDisposable _typingState = null;
        public bool IsTyping { get; private set; }

        public async void SetTyping(bool isTyping)
        {
            if (isTyping)
            {
                await _channel.TriggerTypingAsync();
                IsTyping = true;
            }
            else
            {
                IsTyping = false;
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

        public async Task<List<IBufferedMessage>> GetMessagesAsync(DateTimeOffset time, SearchDirection direction = SearchDirection.Around, int limit = 100)
        {
            List<IBufferedMessage> result = new List<IBufferedMessage>();
            Direction discordDirection;
            switch (direction)
            {
                case SearchDirection.Before:
                    discordDirection = Direction.Before;
                    break;

                case SearchDirection.After:
                    discordDirection = Direction.After;
                    break;

                default:
                    discordDirection = Direction.Around;
                    break;
            }

            var enumerator = _channel.GetMessagesAsync(time.ToSnowflake(), discordDirection, limit);
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