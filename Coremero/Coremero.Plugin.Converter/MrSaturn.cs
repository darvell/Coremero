using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coremero.Commands;
using Coremero.Utilities;

namespace Coremero.Plugin.Converter
{
    public class MrSaturn : IPlugin
    {
        private Dictionary<char, char> _saturnReplacements = new Dictionary<char, char>
        {
            {'a', 'ᗩ'},
            {'b', 'ᗷ'},
            {'c', 'ᘓ'},
            {'d', 'ᗪ'},
            {'e', 'ᕮ'},
            {'f', 'ᖴ'},
            {'g', 'ᕤ'},
            {'h', 'ᗁ'},
            {'i', 'ᓮ'},
            {'j', 'ᒎ'},
            {'k', 'ᔌ'},
            {'l', 'ᒪ'},
            {'m', 'ᙏ'},
            {'n', 'ᑎ'},
            {'o', 'ᘎ'},
            {'p', 'ᖘ'},
            {'q', 'ᕴ'},
            {'r', 'ᖇ'},
            {'s', 'ᔕ'},
            {'t', 'ᒮ'},
            {'u', 'ᘮ'},
            {'v', 'ᐯ'},
            {'w', 'ᙎ'},
            {'x', '᙭'},
            {'y', 'ᖿ'},
            {'z', 'ᔓ'},
            {'\'', 'ᐞ'}
        };

        [Command("saturn", Help = ".saturn <text> - Cᘎᑎᐯᕮᖇᒮ <ᒮᕮ᙭ᒮ> ᒮᘎ Sᗩᒮᘮᖇᑎᓮᗩᑎ.")]
        public string SaturnConvert(IInvocationContext context, IMessage message)
        {
            return String.Concat(message.Text?.TrimCommand().Select(x =>
            {
                if (_saturnReplacements.ContainsKey(x))
                {
                    return _saturnReplacements[x];
                }
                return x;
            }) ?? String.Empty);
        }

    }
}
