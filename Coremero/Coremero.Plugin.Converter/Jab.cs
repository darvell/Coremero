using System.Text;
using Coremero.Client;
using Coremero.Commands;
using Coremero.Utilities;

namespace Coremero.Plugin.Converter
{
    public class Jab : IPlugin
    {
        [Command("jab")]
        public string FullWidth(IInvocationContext context, IMessage message)
        {
            return message.Text.TrimCommand().ToUnicodeFullWidth();
        }

        [Command("bigjab")]
        public string FullWidthMultiline(IInvocationContext context, IMessage message)
        {

            string jab = FullWidth(context, message);

            // Generate borders.
            var fancy = "ஜ۩۞۩ஜ";

            var padLen = (jab.Length * 2 - fancy.Length) / 2 - 1;
            var spaces = "";

            if (padLen < 0)
            {
                jab = jab.PadLeft(fancy.Length / 2 + 1).PadRight(fancy.Length);
            }
            else
            {
                spaces = "".PadLeft(padLen, '\u25AC');
            }

            fancy = $"{spaces}{fancy}{spaces}";
            jab = $"{fancy}\n{jab}\n{fancy}";


            if (context.OriginClient.Features.HasFlag(ClientFeature.Markdown))
            {
                return $"```\n{jab}\n```";
            }

            return jab;
        }
    }
}
