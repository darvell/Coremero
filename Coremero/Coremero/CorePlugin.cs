using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coremero.Client;
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

        [Command("echo", Help = ".echo <text> - Return <text>.")]
        public string Echo(IInvocationContext context, IMessage message)
        {
            return string.Join(" ", message.Text.GetCommandArguments());
        }

        [Command("woke", Help = ".woke <message> - Return message in uppercase, split by spaces separated by 👏.")]
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

        [Command("list", Help = "List all commands.")]
        public string CommandList(IInvocationContext context, IMessage message)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var cmd in _commandRegistry.CommandAttributes.OrderBy(x => x.Name))
            {
                sb.AppendLine($"{"." + cmd.Name,-10} {cmd.Help}");
            }

            if (context.OriginClient.Features.HasFlag(ClientFeature.Markdown))
            {
                return $"```css\n{sb.ToString()}\n```";
            }
            return sb.ToString();
        }

        public void Dispose()
        {
            // ignore
        }
    }
}
