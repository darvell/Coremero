using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Messages;
using Xunit;

namespace Coremero.Test
{

    public class CommandMapTest
    {
        public CommandMap Map { get; set; }

        public CommandMapTest()
        {
            Map = new CommandMap();
            Map.RegisterPluginCommands(new MockPlugin());
        }

        [Fact]
        public async void CanExecuteAsync()
        {
            await Map.ExecuteCommandAsync("example", null, null);
        }

        [Fact]
        public async void CanEcho()
        {
            IMessage message = Message.Create("Hello!");
            IMessage result = await Map.ExecuteCommandAsync("echo", null, message);
            if (result != message)
            {
                Debug.Fail("Result != Message");
            }
        }

        [Fact]
        public async void CanAwaitInvalidCommand()
        {
            IMessage result = await Map.ExecuteCommandAsync("notreal", null, null);
            if (result != null)
            {
                Debug.Fail("Result was not null.");
            }
        }

    }
}
