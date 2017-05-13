using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coremero.Attachments;
using Coremero.Client.Discord;
using Coremero.Messages;
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
            IReactableMessage message = e.Message as IReactableMessage;

            if (message == null)
                return;

            if (message.Text.Split(' ').Any(x => x.Equals("anime", StringComparison.OrdinalIgnoreCase)))
            {
                foreach (var reactId in _animeReactionIds)
                {
                    try
                    {
                        await message.React(reactId);
                    }
                    catch
                    {
                        // Whatever
                        break;
                    }
                }
            }
        }
    }
}