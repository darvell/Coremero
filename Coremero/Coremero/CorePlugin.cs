using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coremero.Commands;
using Coremero.Registry;
using Coremero.Utilities;

namespace Coremero
{
    public class CorePlugin : IPlugin
    {
        private CommandRegistry _commandRegistry;
        public CorePlugin(CommandRegistry commandRegistry)
        {
            _commandRegistry = commandRegistry;
        }

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

        [Command("gc", Help = "Dev only command. Forces a GC.")]
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

        [Command("exception", Help = "Dev only command, throws an exception.")]
        public string ThrowException(IInvocationContext context, IMessage message)
        {
            throw new Exception("I broke for you.");
            return "How?";
        }

        [Command("list")]
        public string CommandList(IInvocationContext context, IMessage message)
        {
            StringBuilder sb = new StringBuilder();
            _commandRegistry.CommandAttributes.ForEach(x =>
            {
                sb.AppendLine($".{x.Name} - {x.Help}");
            });
            return $"```\n{sb.ToString()}\n```";
        }

        public void Dispose()
        {
            // ignore
        }
    }
}
