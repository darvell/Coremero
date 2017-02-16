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
                _cache[source.Id] = (TTarget) Activator.CreateInstance(typeof(TTarget), source);
            }
            return result;
        }
    }

    public static class DiscordFactory
    {
            public static readonly DiscordObjectFactory<IGuild, DiscordServer> ServerFactory = new DiscordObjectFactory<IGuild, DiscordServer>();
            public static readonly DiscordObjectFactory<IMessageChannel, DiscordChannel> ChannelFactory = new DiscordObjectFactory<IMessageChannel, DiscordChannel>();
            public static readonly DiscordObjectFactory<global::Discord.IUser, DiscordUser> UserFactory = new DiscordObjectFactory<global::Discord.IUser, DiscordUser>();
    }
}
