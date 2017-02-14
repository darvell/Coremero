using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Registry;
using Coremero.Utilities;

namespace Coremero.Plugin.Classic
{
    public class Pipe : IPlugin
    {
        private CommandRegistry _commandRegistry;

        public Pipe(CommandRegistry cmdRegistry)
        {
            _commandRegistry = cmdRegistry;
        }

        [Command("pipe", Arguments = "Command | Command", Help = "Chain multiple commands together.")]
        public async Task<IMessage> PipeCommand(IInvocationContext context, IMessage message)
        {
            List<string> cmds = string.Join(" ", message.Text.GetCommandArguments()).Split('|').ToList();
            Message basicMessage = Message.Create(message.Text, message.Attachments?.ToArray());
            basicMessage.Text = string.Join(" ", cmds.First().Split(' ').Skip(1).ToList()).Trim();
            bool appendNext = false;
            foreach (var cmd in cmds)
            {
                List<string> call = cmd.Trim().Split(' ').ToList();
                if (!_commandRegistry.Exists(call.First()))
                {
                    // Special commands.
                    if (call[0] == "cleartext")
                    {
                        basicMessage.Text = string.Empty;
                    }
                    else if (call[0] == "clearattachment")
                    {
                        basicMessage.Attachments?.ForEach(x => x.Contents?.Dispose());
                        basicMessage.Attachments = null;
                    }
                    else if (call[0] == "append")
                    {
                        appendNext = true;
                    }
                    continue;
                }

                IMessage result = await _commandRegistry.ExecuteCommandAsync(call.First(), context,
                    call.Count == 1
                        ? basicMessage
                        : Message.Create(string.Join(" ", call.Skip(1)), basicMessage.Attachments?.ToArray()));

                if (!string.IsNullOrEmpty(result.Text))
                {
                    basicMessage.Text = result.Text;
                }
                if (result.Attachments != null)
                {
                    if (appendNext)
                    {
                        basicMessage.Attachments?.AddRange(result.Attachments);
                        appendNext = false;
                    }
                    else
                    {
                        if (basicMessage.Attachments?.Count > 0)
                        {
                            // Prevent leak since we're going out of scope
                            basicMessage.Attachments.ForEach(x => x.Contents?.Dispose());
                        }
                        basicMessage.Attachments = result.Attachments;
                    }

                }
            }

            return basicMessage;
        }
    }
}