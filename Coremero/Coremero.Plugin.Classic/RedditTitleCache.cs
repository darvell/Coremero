using System;
using System.Collections.Generic;

namespace Coremero.Plugin.Classic
{
    // TODO: Just write a string caching mechanism already. Plenty of things use this.
    public class RedditTitleCache
    {
        private readonly List<string> _titles = new List<string>();

        public RedditTitleCache(string subreddit, TimeSpan cacheExpiry)
        {
        }
    }
}