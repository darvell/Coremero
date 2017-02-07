using Coremero.Commands;
using Coremero.Utilities;

namespace Coremero.Plugin.Classic
{
    public class Circlejerk : IPlugin
    {
        [Command("clump")]
        public string BsFeelings(IInvocationContext context, IMessage message)
        {
            return "゜・。。・゜゜・。。・゜☆゜・。。・゜ im too bullshit feeligns  。・゜゜・。。・゜☆゜・。。・゜゜・。。・゜";
        }

        [Command("depths")]
        public string Depths(IInvocationContext context, IMessage message)
        {
            return message.Text.TrimCommand().Replace('o', 'ø');
        }
    }
}
