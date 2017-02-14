using System;
using System.Collections.Generic;
using System.Linq;
using Coremero.Client.Discord;
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
            "great success"
        };

        private readonly Random _rnd = new Random();

        public BoratReact(IMessageBus messageBus)
        {
            messageBus.Received += MessageBus_Received;
        }

        private async void MessageBus_Received(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message is DiscordMessage && _boratPhrases.Any(x => e.Message.Text.CaseInsensitiveContains(x)))
            {
                await ((DiscordMessage) e.Message).React("<:borat:244253799030587402>");
                if (_rnd.Next(0, 100) < 5)
                {
                    await e.Context.Raiser.SendAsync(Message.Create($"{e.Context.User.Mention} Dude is that Borat?"));
                }
            }
        }
    }
}