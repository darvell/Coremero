using System;
using System.Collections.Generic;
using System.Text;
using Coremero.Commands;
using Coremero.Utilities;
using System.Linq;

namespace Coremero.Plugin.Playground
{
    public class Bigmojify : IPlugin
    {
        private readonly Dictionary<char, string> _charRegionMap = new Dictionary<char, string>()
        {
            {'a', "🇦"},
            {'b', "🇧"},
            {'c', "🇨"},
            {'d', "🇩"},
            {'e', "🇪"},
            {'f', "🇫"},
            {'g', "🇬"},
            {'h', "🇭"},
            {'i', "🇮"},
            {'j', "🇯"},
            {'k', "🇰"},
            {'l', "🇱"},
            {'m', "🇲"},
            {'n', "🇳"},
            {'o', "🇴"},
            {'p', "🇵"},
            {'q', "🇶"},
            {'r', "🇷"},
            {'s', "🇸"},
            {'t', "🇹"},
            {'u', "🇺"},
            {'v', "🇻"},
            {'w', "🇼"},
            {'x', "🇽"},
            {'y', "🇾"},
            {'z', "🇿"}
        };

        [Command("bigmojify")]
        public string TextToRegionalIndicators(string input)
        {
            return string.Concat(input.TrimCommand().Select(c =>
            {
                char lowerChar = Char.ToLower(c);
                if (_charRegionMap.ContainsKey(lowerChar))
                {
                    return _charRegionMap[lowerChar] + " ";
                }
                return c.ToString();
            }));
        }
    }
}
