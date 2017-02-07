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
            string clean = string.Join(" ", message.Text.Split(' ').Skip(1));
            List<string> cmds = clean.Split('|').ToList();
            Message basicMessage = Message.Create(message.Text, message.Attachments?.ToArray());
            basicMessage.Text = cmds.First();
            foreach (var cmd in cmds)
            {
                IMessage result = await _commandRegistry.ExecuteCommandAsync(cmd.Split(' ').First().Trim(), context, basicMessage);
                basicMessage.Text = result.Text;
                if (basicMessage.Attachments != null)
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
