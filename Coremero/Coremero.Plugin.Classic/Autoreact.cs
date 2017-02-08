using System;
using System.Collections.Generic;
using System.Text;
using Coremero.Client.Discord;
using Coremero.Services;

namespace Coremero.Plugin.Classic
{
    public class Autoreact : IPlugin
    {
        private List<string> _animeReactionIds = new List<string>()
        {
            "<:_i:275473549286834176>",
            "<:_think:275473560854724608>",
            "<:_you:275473570413412352>",
            "<:_mean:275473580421021696>",
            "<:_anime:275473588843184128>"
        };

        public Autoreact(IMessageBus messageBus)
        {
            messageBus.Received += MessageBusOnReceived;
        }

        private async void MessageBusOnReceived(object sender, MessageReceivedEventArgs e)
        {
            DiscordMessage message = e.Message as DiscordMessage;

            if (message == null)
                return;

            if (message.Text.Contains("anime"))
            {
                foreach (var reactId in _animeReactionIds)
                {
                    await message.React(reactId);
                }
            }
        }
    }
}
