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

        [Command("woke")]
        public string Woke(IInvocationContext context, IMessage message)
        {
            return $"👏 {string.Join(" 👏 ", message.Text.ToUpper().GetCommandArguments())} 👏";
        }

        // TODO: PERM CHECK ASAP!
        [Command("gc")]
        public string RunGC(IInvocationContext context, IMessage message)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Pre: {GC.GetTotalMemory(false) / 1024.0}KB");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            builder.AppendLine($"Post: {GC.GetTotalMemory(false) / 1024.0}KB");
            return builder.ToString();
        }

        public void Dispose()
        {
            // ignore
        }
    }
}
