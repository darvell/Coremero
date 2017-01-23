using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
        public async Task TaskForExampleWillExist()
        {
            CommandMap commandMap = new CommandMap();
            IPlugin testPlugin = new CommandMapTestPlugin();
            commandMap.RegisterPluginCommands(testPlugin);
            object result = await commandMap.ExecuteCommand("example", null);
            var s = result as string;
            if (s == null)
            {
                throw new Exception("Wrong.");
            }
        }
    }
}
