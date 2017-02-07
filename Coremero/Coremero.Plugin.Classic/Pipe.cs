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

        [Command("pipe")]
        public async Task<IMessage> PipeCommand(IInvocationContext context, IMessage message)
        {
            List<string> cmds = string.Join(" ", message.Text.GetCommandArguments()).Split('|').ToList();
            Message basicMessage = Message.Create(message.Text, message.Attachments?.ToArray());
            basicMessage.Text = cmds.First();
            foreach (var cmd in cmds)
            {
                string cmdCall = cmd.Trim().Split(' ').First().Trim();
                IMessage result = await _commandRegistry.ExecuteCommandAsync(cmdCall, context, basicMessage);
                if (!string.IsNullOrEmpty(result.Text))
                {
                    basicMessage.Text = result.Text;
                }
                if (result.Attachments != null)
                {
                    basicMessage.Attachments = result.Attachments;
                }
            }

            return basicMessage;
        }

        public void Dispose()
        {
        }
    }
}
