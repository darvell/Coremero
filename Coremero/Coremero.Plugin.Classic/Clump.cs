using Coremero.Commands;

namespace Coremero.Plugin.Classic
{
    public class Clump : IPlugin
    {
        [Command("clump")]
        public string BsFeelings(IInvocationContext context, IMessage message)
        {
            return "゜・。。・゜゜・。。・゜☆゜・。。・゜ im too bullshit feeligns  。・゜゜・。。・゜☆゜・。。・゜゜・。。・゜";
        }
    }
}
