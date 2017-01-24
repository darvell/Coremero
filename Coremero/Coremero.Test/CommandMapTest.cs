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

        [Command("exampleasync")]
        public async Task<string> ExampleAsync(IMessageContext context)
        {
            await Task.Delay(1000);
            return "hi im async";
        }

    }

    public class CommandMapTest
    {
        [Fact]
        public void CanExecuteAsyncCommandAsyncAndGetResult()
        {
            CommandMap commandMap = new CommandMap();
            IPlugin testPlugin = new CommandMapTestPlugin();
            commandMap.RegisterPluginCommands(testPlugin);
            object result = commandMap.ExecuteCommand("exampleasync", null);
            var s = result as string;
            if (s == null)
            {
                throw new Exception("Wrong.");
            }

        }

        [Fact]
        public void CanRunCommandAndGetResult()
        {
            CommandMap commandMap = new CommandMap();
            IPlugin testPlugin = new CommandMapTestPlugin();
            commandMap.RegisterPluginCommands(testPlugin);
            object result = commandMap.ExecuteCommand("example", null);
            var s = result as string;
            if (s == null)
            {
                throw new Exception("Wrong.");
            }
        }

        [Fact]
        public async Task CanRunCommandAsyncAndGetResult()
        {
            CommandMap commandMap = new CommandMap();
            IPlugin testPlugin = new CommandMapTestPlugin();
            commandMap.RegisterPluginCommands(testPlugin);
            object result = await commandMap.ExecuteCommandAsync("example", null);
            var s = result as string;
            if (s == null)
            {
                throw new Exception("Wrong.");
            }
        }

        [Fact]
        public void CanRunCommandAThousandTimesAndGetResult()
        {
            CommandMap commandMap = new CommandMap();
            IPlugin testPlugin = new CommandMapTestPlugin();
            commandMap.RegisterPluginCommands(testPlugin);
            object result = null;
            for (int i = 0; i < 1000; i++)
            {
                result = commandMap.ExecuteCommand("example", null);
            }
            var s = result as string;
            if (s == null)
            {
                throw new Exception("Wrong.");
            }
        }

        [Fact]
        public async Task CanRunCommandAsyncAThousandTimesAndGetResult()
        {
            CommandMap commandMap = new CommandMap();
            IPlugin testPlugin = new CommandMapTestPlugin();
            commandMap.RegisterPluginCommands(testPlugin);
            object result = null;
            for (int i = 0; i < 1000; i++)
            {
                 result = await commandMap.ExecuteCommandAsync("example", null);
            }
            var s = result as string;
            if (s == null)
            {
                throw new Exception("Wrong.");
            }
        }


    }
}
