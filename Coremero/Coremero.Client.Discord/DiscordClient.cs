using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coremero;
using Coremero.Context;
using Coremero.Services;
using Coremero.Storage;
using Discord;
using Discord.WebSocket;
using IMessage = Coremero.Messages.IMessage;

namespace Coremero.Client.Discord
{
    public class DiscordClient : IClient, IClientUserStatus
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

        public string Username
        {
            get { return _discordClient?.CurrentUser?.Username; }
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
        private const long DEBUG_CNC_CHANNEL_ID = 336313212280766475;
        private const long DEBUG_GUILD = 336312951743053824;
        private readonly string DISCORD_BOT_KEY;

        public DiscordClient(IMessageBus messageBus, ICredentialStorage credentialStorage)
        {
            _messageBus = messageBus;
            DISCORD_BOT_KEY = credentialStorage.GetKey("discord", null);
            _discordClient = new DiscordSocketClient(new DiscordSocketConfig()
            {
                MessageCacheSize = 50
            });
        }

        public async Task Connect()
        {
            if (IsConnected)
            {
                throw new InvalidOperationException();
            }

            await _discordClient.LoginAsync(TokenType.Bot, DISCORD_BOT_KEY);
            await _discordClient.StartAsync();
            IsConnected = true;

#if DEBUG
            var cncChannel = _discordClient.GetGuild(DEBUG_GUILD)?.Channels.FirstOrDefault(x => x.Id == DEBUG_CNC_CHANNEL_ID);
            if (cncChannel != null)
            {
                var token = new CancellationToken();
#pragma warning disable 4014
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
#pragma warning restore 4014

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
                if (_lastIgnoreTime != DateTime.MinValue && (DateTime.Now - _lastIgnoreTime).TotalSeconds < 30)
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
                await _discordClient.StopAsync();
                _discordClient.MessageReceived -= DiscordClientOnMessageReceived;
            }
        }

        public event EventHandler<Exception> Error;

        public IEnumerable<IServer> Servers
        {
            get { return _discordClient.Guilds.Select(x => DiscordFactory.ServerFactory.Get(x)); }
        }

        public string UserStatus
        {
            get
            {
                return _discordClient.CurrentUser?.Game?.Name;
            }
            set
            {
#pragma warning disable 4014
                _discordClient?.SetGameAsync(value);
#pragma warning restore 4014
            }
        }
    }
}