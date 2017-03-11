using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Discord;

namespace Coremero.Client.Discord
{
    public class DiscordObjectFactory<TSource, TTarget> where TSource : ISnowflakeEntity
    {
        private ConcurrentDictionary<ulong, TTarget> _cache = new ConcurrentDictionary<ulong, TTarget>();

        public TTarget Get(TSource source)
        {
            return (TTarget) Activator.CreateInstance(typeof(TTarget), source);

            /* DISABLED FOR NOW BECAUSE THINGS GO OUT OF SYNC AND THAT MAKES ME SAD */

            /*
            TTarget result;
            _cache.TryGetValue(source.Id, out result);
            if (result == null)
            {
                // Resort to reflection. :(
                _cache.TryAdd(source.Id, (TTarget) Activator.CreateInstance(typeof(TTarget), source));
                return Get(source);
            }
            return result;
            */
        }
    }

    // TODO: Move to DI.
    public static class DiscordFactory
    {
        public static readonly DiscordObjectFactory<IGuild, DiscordServer> ServerFactory = new DiscordObjectFactory<IGuild, DiscordServer>();
        public static readonly DiscordObjectFactory<IMessageChannel, DiscordChannel> ChannelFactory = new DiscordObjectFactory<IMessageChannel, DiscordChannel>();
        public static readonly DiscordObjectFactory<global::Discord.IUser, DiscordUser> UserFactory = new DiscordObjectFactory<global::Discord.IUser, DiscordUser>();
    }
}
