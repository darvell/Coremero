using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Coremero.Attachments;
using Coremero.Client;
using Coremero.Client.Discord;
using Coremero.Messages;
using Coremero.Utilities;

namespace Coremero.Plugin.Playground
{
    public class Pancake : IPlugin
    {
        private IChannel _catChannel;

        public Pancake(IEnumerable<IClient> clients)
        {
            DiscordClient discordClient = clients.FirstOrDefault(x => x.Name.Contains("Discord")) as DiscordClient;
            new Timer((state) =>
            {
                if (_catChannel == null)
                {
                    _catChannel = discordClient?.Servers.Select(x => x.Channels.FirstOrDefault(y => y.Name.Contains("behind-the-dennys"))).FirstOrDefault(x => x != null);
                }
                if (DateTime.Now.Minute == 0 && DateTime.Now.Hour == 15)
                {
                    _catChannel?.SendAsync(Message.Create("I will post this every day until you like it.",
                        new FileAttachment(Path.Combine(PathExtensions.ResourceDir, "pancakecat.jpg"))));
                }
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }
    }
}