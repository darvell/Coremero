using Coremero.Commands;
using Coremero.Context;
using Coremero.Messages;
using Coremero.Utilities;

namespace Coremero.Plugin.Converter
{
    public class Script : IPlugin
    {
        [Command("script", Help = ".script <text> - 𝕮𝒐𝒏𝒗𝒆𝒓𝒕𝒔 𝒕𝒆𝒙𝒕 𝒕𝒐 𝒉𝒂𝒏𝒅𝒘𝒓𝒊𝒕𝒕𝒆𝒏 𝒔𝒄𝒓𝒊𝒑𝒕."
        )]
        public string ScriptText(IInvocationContext context, IMessage message)
        {
            return message.Text.TrimCommand().ToUnicodeHandwrittenScript();
        }
    }
}