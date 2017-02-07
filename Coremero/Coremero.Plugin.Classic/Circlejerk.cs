using System;
using Coremero.Commands;
using Coremero.Utilities;

namespace Coremero.Plugin.Classic
{
    public class Circlejerk : IPlugin
    {
        private Random _rnd = new Random();

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

        [Command("pepito")]
        public string Pepito(IInvocationContext context, IMessage message)
        {
            int pillAmount = _rnd.Next(20,421);
            return $"<@!256179116200558594> hey guys i ate ${ (pillAmount == 100 ? "💯" : pillAmount.ToString()) } pills"
        }
    }
}
