using System;
using System.Collections.Generic;
using System.Text;
using Discord;

namespace Coremero.Client.Discord
{
    public class DiscordObjectFactory<TSource, TTarget> where TSource : ISnowflakeEntity
    {
        private Dictionary<ulong, TTarget> _cache = new Dictionary<ulong, TTarget>();

        public TTarget Get(TSource source)
        {
            TTarget result;
            _cache.TryGetValue(source.Id, out result);
            if (result == null)
            {
                // Resort to reflection. :(
                _cache[source.Id] = (TTarget) Activator.CreateInstance(typeof(TTarget), new[] {source});
            }
            return result;
        }
    }

    public static class DiscordFactory
    {
            public static readonly DiscordObjectFactory<IGuild, IServer> ServerFactory = new DiscordObjectFactory<IGuild, IServer>();
            public static readonly DiscordObjectFactory<IMessageChannel, IChannel> ChannelFactory = new DiscordObjectFactory<IMessageChannel, IChannel>();
            public static readonly DiscordObjectFactory<global::Discord.IUser, IUser> UserFactory = new DiscordObjectFactory<global::Discord.IUser, IUser>();
    }
}
