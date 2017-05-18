using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coremero.Client;
using Coremero.Commands;
using Coremero.Context;
using Coremero.Messages;
using Coremero.Plugin;
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

        [Command("echo", Help = "Returns the exact same input.")]
        public string Echo(IInvocationContext context, IMessage message)
        {
            return string.Join(" ", message.Text.GetCommandArguments());
        }

        [Command("upper", Help = "Returns the exact same input but in uppercase.")]
        public string Upper(IMessage message)
        {
            return message.Text.TrimCommand().ToUpper();
        }


        [Command("woke", Help = "Return message in uppercase, split by spaces separated by 👏.")]
        public string Woke(IMessage message)
        {
            return $"👏 {string.Join(" 👏 ", message.Text.ToUpper().GetCommandArguments())} 👏";
        }

        [Command("gc", MinimumPermissionLevel = UserPermission.BotOwner, Help = "Forces GC.")]
        public string RunGC(IInvocationContext context)
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

        [Command("tasklist", MinimumPermissionLevel = UserPermission.BotOwner,
            Help = "Reports task list if not threadpooling.")]
        public string Tasks(IInvocationContext context)
        {
            if (context.User?.Permissions != UserPermission.BotOwner)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();
            int taskCount = 0;
            foreach (var task in TaskScheduler.Current.GetScheduledTasksForDebugger())
            {
                builder.AppendLine($"{task.Id} - {task.Status}");
                taskCount++;
            }
            builder.AppendLine($"Tasks total: {taskCount}");

            if (context.OriginClient.Features.HasFlag(ClientFeature.Markdown))
            {
                return $"```\n{builder.ToString()}\n```";
            }
            return builder.ToString();
        }

        [Command("exception", MinimumPermissionLevel = UserPermission.BotOwner, Help = "Throw an exception.")]
        public string ThrowException(IInvocationContext context, IMessage message)
        {
            throw new Exception("I broke for you.");
        }

        [Command("list", Help = "List all commands.")]
        public void CommandList(IInvocationContext context, IMessage message)
        {
            ISendable target = context.Raiser;
            if (context.User.Permissions != UserPermission.BotOwner)
            {
                target = context.User;
            }
            StringBuilder sb = new StringBuilder();
            foreach (
                var cmd in
                _commandRegistry.CommandAttributes.OrderBy(x => x.Name)
                    .Where(x => x.MinimumPermissionLevel <= context.User.Permissions))
            {
                string args = String.Empty;
                if (!String.IsNullOrEmpty(cmd.Arguments))
                {
                    args = string.Join(" ", cmd.Arguments.Split('|').Select(x => $"[{x.Trim()}]"));
                }
                sb.AppendLine($"{"." + cmd.Name + " " + args}");
            }

            target.Send(context.OriginClient.Features.HasFlag(ClientFeature.Markdown)
                ? Message.Create($"```css\n{sb.ToString()}\n```")
                : Message.Create(sb.ToString()));
        }

        [Command("help", Arguments = "Command Name", Help = "Get info on a command.")]
        public string Help(string command)
        {
            return _commandRegistry.GetHelp(command);
        }

        [Command("hello", Help = "Just says hello.")]
        public string Hello()
        {
            return "hello";
        }

        [Command("bye", Help = "Just says bye.")]
        public string Bye()
        {
            return "Bye";
        }

        [Command("setstatus", Arguments = "Status", MinimumPermissionLevel = UserPermission.BotOwner)]
        public void SetClientStatus(IInvocationContext context, string status)
        {
            if (context.OriginClient is IClientUserStatus)
            {
                ((IClientUserStatus) context.OriginClient).UserStatus = status;
            }
        }

        public void Dispose()
        {
            // ignore
        }
    }
}