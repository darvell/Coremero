using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Utilities;

namespace Coremero.Plugin.Classic
{
    public class Circlejerk : IPlugin
    {
        private Random _rnd = new Random();
        private List<string> _tonyOutput = new List<string>
        {
            "Tony is sitting opposite you, examinig each of his fingers in turn.",
            "You wish you could put Tony out of his misery.",
            "You isl you could put Tony out of h5s mgsery.",
            "You wis5 yougco6ld put T4ny out ofchis 4iser7.",
            "Yo4 wis5hyobgcokldsp4t T46y 3ut ofc7is 4is5r74",
            "Y44 wis5hy3bg5o5ld7p44 T464444444fc7is44i454744",
            "54783il5hy3bg5o55d788888864444444f37is24i454744",
            "----------- rest in peace tony -----------"
        };

        [Command("clump")]
        public string BsFeelings(IInvocationContext context, IMessage message)
        {
            return "゜・。。・゜゜・。。・゜☆゜・。。・゜ im too bullshit feeligns  。・゜゜・。。・゜☆゜・。。・゜゜・。。・゜";
        }

        [Command("tone")]
        public async Task Tone(IInvocationContext context, IMessage message)
        {
            if (context.User.Mention.Contains("@hardclumping#2389"))
            {
                foreach (var tonyLine in _tonyOutput)
                {
                    await context.Raiser.SendAsync(Message.Create(tonyLine));
                    await Task.Delay(1000);
                }
            }
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
