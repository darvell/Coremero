using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Context;
using Coremero.Messages;
using Coremero.Services;
using Coremero.Utilities;
using MarkovSharpNetCore.TokenisationStrategies;

namespace Coremero.Plugin.Playground
{
    public class ImitateChat : IPlugin
    {
        readonly Dictionary<string, StringMarkov> _models = new Dictionary<string, StringMarkov>();

        public ImitateChat(IMessageBus messageBus)
        {
            messageBus.Received += MessageBus_Received;
        }

        private void MessageBus_Received(object sender, MessageReceivedEventArgs e)
        {
            IChannel channel = e.Context?.Channel;
            if (channel != null)
            {
                if (e.Context.User?.Name != e.Context.OriginClient.Username &&
                    !string.IsNullOrEmpty(e.Message.Text?.Trim()) && !e.Message.Text.IsCommand())
                {
                    if (_models.ContainsKey(e.Context.Channel.Name))
                    {
                        _models[channel.Name].Learn(e.Message.Text);
                    }
                }
            }
        }

        [Command("imichat")]
        public async Task<string> ImiChat(IInvocationContext context)
        {
            IBufferedChannel bufferedChannel = context.Channel as IBufferedChannel;
            if (bufferedChannel != null)
            {
                if (!_models.ContainsKey(bufferedChannel.Name))
                {
                    StringMarkov markov = new StringMarkov() { EnsureUniqueWalk = true };
                    DateTimeOffset offset = DateTimeOffset.UtcNow;
                    while (markov.Model.Keys.Count < 5000)
                    {
                        List<IBufferedMessage> messages = await bufferedChannel.GetMessagesAsync(offset, SearchDirection.Before);
                        var newOffset = new DateTimeOffset(messages.Last().Timestamp);
                        if (newOffset == offset)
                        {
                            break;
                        }
                        markov.Learn(messages.Where(x => x.User.Name != context.OriginClient.Username && !string.IsNullOrEmpty(x.Text?.Trim()) && !x.Text.IsCommand()).Select(x => x.Text));
                        offset = newOffset;
                    }
                    _models[bufferedChannel.Name] = markov;
                }

                return _models[context.Channel.Name].Walk(10).OrderByDescending(x => x.Length).Take(5).GetRandom();
            }
            throw new Exception("Not buffered channel.");
        }


    }
}
