using System;
using System.Threading.Tasks;
using Coremero;

namespace Coremero.Client.Discord
{
    public class DiscordClient : IClient
    {
        public string Name
        {
            get { return "Discord"; }
        }

        public string Description
        {
            get { return "Discord client based off Discord.NET 1.x"; }
        }

        public ClientFeature Features
        {
            get
            {
                return ClientFeature.All; // TODO: Not all
            }
        }

        public bool IsConnected { get; }
        public Task Connect()
        {

        }

        public Task Disconnect()
        {
        }

        public event EventHandler<Exception> Error;
    }
}
