using System;
using System.Threading.Tasks;
using Coremero.Commands;
using Xunit;

namespace Coremero.Test
{
    public class CommandMapTestPlugin : IPlugin
    {

        [Command("example")]
        public string Example(IInvocationContext context, IMessage message)
        {
            return "hi";
        }

        [Command("exampleasync")]
        public async Task<string> ExampleAsync(IInvocationContext context, IMessage message)
        {
            await Task.Delay(1000);
            return "hi im async";
        }

        public void Dispose()
        {
            // ignore
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
            object result = commandMap.ExecuteCommand("exampleasync", null, null);
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
            object result = commandMap.ExecuteCommand("example", null, null);
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
            object result = await commandMap.ExecuteCommandAsync("example", null, null);
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
                result = commandMap.ExecuteCommand("example", null, null);
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
                 result = await commandMap.ExecuteCommandAsync("example", null, null);
            }
            var s = result as string;
            if (s == null)
            {
                throw new Exception("Wrong.");
            }
        }

        [Fact]
        public void CommandDoesntExistAndReturnsNull()
        {
            CommandMap commandMap = new CommandMap();
            if (commandMap.ExecuteCommand("notreal", null, null) != null)
            {
                throw new Exception("Somehow ran a non-existant command.");
            }
        }

        [Fact]
        public void CommandDoesntExistAndOthersAreRegistered()
        {
            CommandMap commandMap = new CommandMap();
            IPlugin testPlugin = new CommandMapTestPlugin();
            commandMap.RegisterPluginCommands(testPlugin);
            object result = commandMap.ExecuteCommand("notreal", null, null);
            if (result != null)
            {
                throw new Exception("Somehow ran a non-existant command.");
            }
        }


    }
}
