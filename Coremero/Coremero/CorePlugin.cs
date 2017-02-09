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

        [Command("gc")]
        public string RunGC(IInvocationContext context, IMessage message)
        {
            if (context.User?.Permissions != UserPermission.BotOwner)
            {
                return null;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Pre: {GC.GetTotalMemory(false) / 1024}KB");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            builder.AppendLine($"Post: {GC.GetTotalMemory(false) / 1024}KB");
            return builder.ToString();
        }

        [Command("exception")]
        public string ThrowException(IInvocationContext context, IMessage message)
        {
            throw new Exception("I broke for you.");
            return "How?";
        }

        public void Dispose()
        {
            // ignore
        }
    }
}
