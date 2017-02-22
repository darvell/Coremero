using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coremero.Plugin.Classic
{
    // TODO: Just write a string caching mechanism already. Plenty of things use this.
    public class RedditTitleCache
    {
        private List<string> _titles = new List<string>();

        public RedditTitleCache(string subreddit, TimeSpan cacheExpiry)
        {
        }
    }
}