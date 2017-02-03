using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coremero.Commands;
using Coremero.Utilities;

namespace Coremero
{
    public class CorePlugin : IPlugin
    {
        [Command("echo")]
        public string Echo(IInvocationContext context, IMessage message)
        {
            return string.Join(" ", message.Text.GetCommandArguments());
        }

        public void Dispose()
        {
            // ignore
        }
    }
}
