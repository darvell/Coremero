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

namespace Coremero.Plugin.Borat
{
    public class BoratReact : IPlugin
    {
        private readonly List<string> _boratPhrases = new List<string>()
        {
            "i like",
            "my wife",
            "what type of dog is this",
            "very nice",
            "great success",
            "I can hit a gypsy with a rock from 15 meters away if chained",
            "Your dog is a loser"
        };

        private readonly Random _rnd = new Random();

        public BoratReact(IMessageBus messageBus)
        {
            messageBus.Received += MessageBus_Received;
        }

        private async void MessageBus_Received(object sender, MessageReceivedEventArgs e)
        {
            IReactableMessage reactableMessage = e.Message as IReactableMessage;
            if (reactableMessage != null && _boratPhrases.Any(x => reactableMessage.Text.CaseInsensitiveContains(x)))
            {
                try
                {
                    await reactableMessage.React("<:borat:244253799030587402>");
                    if (_rnd.Next(0, 100) < 5)
                    {
                        await e.Context.Raiser.SendAsync(
                            Message.Create($"{e.Context.User.Mention} Dude is that Borat?"));
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }

        [Command("boratreact", HasSideEffects = true)]
        public async Task<string> BoratGame(IInvocationContext context)
        {
            IReactableMessage message = (IReactableMessage)await context.Channel.SendAsync(Message.Create("hello give me a thumbs up if you think i'm very nice"));

            if (message == null)
            {
                return "I can't read reactions anyway here, I don't like!";
            }

            await Task.Delay(5000);
            StringBuilder builder = new StringBuilder();
            foreach (Reaction reaction in await message.GetReactions())
            {
                if (reaction.Emoji.Contains("👍"))
                {
                    foreach (IUser user in reaction.Reactors)
                    {
                        builder.Append($"{user.Name}, I like! ");
                    }
                }
            }
            if (builder.Length == 0)
            {
                return "I don't like you either!";
            }

            return builder.ToString();
        }
    }
}
