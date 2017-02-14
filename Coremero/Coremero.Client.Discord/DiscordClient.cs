using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coremero;
using Coremero.Services;
using Coremero.Storage;
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
        private DateTime _lastIgnoreTime = DateTime.MinValue;
        private const string DEBUG_IGNORE_PING = "DEBUG_RUNNING_IGNORE";
        private const long DEBUG_CNC_CHANNEL_ID = 280827972720525313;
        private const long DEBUG_GUILD = 109063664560009216;
        private readonly string DISCORD_BOT_KEY;

        public DiscordClient(IMessageBus messageBus, ICredentialStorage credentialStorage)
        {
            _messageBus = messageBus;
            DISCORD_BOT_KEY = credentialStorage.GetKey("discord", null);
            _discordClient = new DiscordSocketClient();
        }

        public async Task Connect()
        {
            if (IsConnected)
            {
                throw new InvalidOperationException();
            }

            await _discordClient.LoginAsync(TokenType.Bot, DISCORD_BOT_KEY);
            await _discordClient.ConnectAsync();
            await _discordClient.WaitForGuildsAsync();
            IsConnected = true;

#if DEBUG
            var cncChannel =
                _discordClient.GetGuild(DEBUG_GUILD)?.Channels.FirstOrDefault(x => x.Id == DEBUG_CNC_CHANNEL_ID);
            if (cncChannel != null)
            {
                var token = new CancellationToken();
                Task.Run(async () =>
                {
                    IMessageChannel cncMessageChannel = cncChannel as IMessageChannel;
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();
                        await cncMessageChannel.SendMessageAsync(DEBUG_IGNORE_PING);
                        Thread.Sleep(TimeSpan.FromSeconds(25));
                    }
                }, token);
            }
#endif

            // TODO: Abstract in to config.
            _discordClient.MessageReceived += DiscordClientOnMessageReceived;
        }

        private Task DiscordClientOnMessageReceived(SocketMessage socketMessage)
        {
            return Task.Run(() =>
            {
#if RELEASE
                if (_lastIgnoreTime != DateTime.MinValue && (DateTime.Now - _lastIgnoreTime).Seconds < 30)
                {
                    return;
                }
#endif
                if (socketMessage.Author.Id == _discordClient.CurrentUser.Id)
                {
#if RELEASE
// TODO: Configurable CNC channel ID.
                    if (socketMessage.Channel.Id == DEBUG_CNC_CHANNEL_ID)
                    {
                        if (socketMessage.Content == DEBUG_IGNORE_PING)
                        {
                            _lastIgnoreTime = DateTime.Now;
                        }
                    }
#endif
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

        public IEnumerable<IServer> Servers
        {
            get { return _discordClient.Guilds.Select(x => new DiscordServer(x)); }
        }
    }
}