using System;
using System.Threading.Tasks;
using Coremero;
using Coremero.Services;
using Discord;
using Discord.WebSocket;

namespace Coremero.Client.Discord
{
    public class DiscordClient : IClient
    {
        #region IClient Properties
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

        public bool IsConnected { get; private set; }

        #endregion


        private IMessageBus _messageBus;
        private DiscordSocketClient _discordClient;

        public DiscordClient(IMessageBus messageBus)
        {
            _messageBus = messageBus;
            _discordClient = new DiscordSocketClient();
        }

        public async Task Connect()
        {
            if (IsConnected)
            {
                throw new InvalidOperationException();
            }

            await _discordClient.LoginAsync(TokenType.Bot, "NO");
            await _discordClient.ConnectAsync();
            IsConnected = true;

            _discordClient.MessageReceived += DiscordClientOnMessageReceived;

        }

        private Task DiscordClientOnMessageReceived(SocketMessage socketMessage)
        {
            return Task.Run(() =>
            {
                if (socketMessage.Author.Id == _discordClient.CurrentUser.Id)
                {
                    return;
                }

                IMessage message = new DiscordMessage(socketMessage);
                IInvocationContext context = new DiscordInvocationContext(this, socketMessage);
                _messageBus.RaiseIncoming(context, message);
            });
        }

        public async Task Disconnect()
        {
            if (IsConnected)
            {
                await _discordClient.DisconnectAsync();
                _discordClient.MessageReceived -= DiscordClientOnMessageReceived;
            }
        }

        public event EventHandler<Exception> Error;
    }
}
