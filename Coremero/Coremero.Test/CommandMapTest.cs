using System;
using System.Collections.Generic;
using System.Text;
using Coremero.Commands;
using Xunit;

namespace Coremero.Test
{
    public class CommandMapTestPlugin : IPlugin
    {

        [Command("example")]
        public string Example(IMessageContext context)
        {
            return "hi";
        }

    }

    public class CommandMapTest
    {
        [Fact]
        public void TaskForExampleWillExist()
        {
            CommandMap commandMap = new CommandMap();
            IPlugin testPlugin = new CommandMapTestPlugin();
            commandMap.RegisterPluginCommands(testPlugin);
            if (commandMap.GetTaskForCommand("example") == null)
            {
                throw new Exception("Task for command was not found.");
            }
        }
    }
}
