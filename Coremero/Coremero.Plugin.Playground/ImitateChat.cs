using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Utilities;
using MarkovSharpNetCore.TokenisationStrategies;

namespace Coremero.Plugin.Playground
{
    public class ImitateChat : IPlugin
    {
        [Command("imichat")]
        public async Task<string> ImiChat(IInvocationContext context)
        {
            IBufferedChannel bufferedChannel = context.Channel as IBufferedChannel;
            if (bufferedChannel != null)
            {
                StringMarkov markov = new StringMarkov(1) { EnsureUniqueWalk = true };
                List<IBufferedMessage> messages = await bufferedChannel.GetLatestMessagesAsync();
                markov.Learn(messages.Where(x => x.User.Name != context.OriginClient.Username && !string.IsNullOrEmpty(x.Text?.Trim()) && !x.Text.IsCommand()).Select(x => x.Text));
                return markov.Walk(10).OrderByDescending(x => x.Length).Take(5).GetRandom();
            }
            throw new Exception("Not buffered channel.");
        }
    }
}
