using System;
using System.Collections.Generic;
using System.Text;
using Coremero.Commands;
using Coremero.Utilities;

namespace Coremero.Plugin.Converter
{
    public class Script : IPlugin
    {
        [Command("script")]
        public string ScriptText(IInvocationContext context, IMessage message)
        {
            return message.Text.TrimCommand().ToUnicodeHandwrittenScript();
        }
    }
}
