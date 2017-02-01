using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Registry;
using Xunit;

namespace Coremero.Test
{

    public class CommandMapTest
    {
        public CommandRegistry Registry { get; set; }

        public CommandMapTest()
        {
            Registry = new CommandRegistry();
            Registry.Register(new MockPlugin());
        }

        [Fact]
        public async void CanExecuteAsync()
        {
            await Registry.ExecuteCommandAsync("example", null, null);
        }

        [Fact]
        public async void CanEcho()
        {
            IMessage message = Message.Create("Hello!");
            IMessage result = await Registry.ExecuteCommandAsync("echo", null, message);
            if (result != message)
            {
                Debug.Fail("Result != Message");
            }
        }

        [Fact]
        public async void CanAwaitInvalidCommand()
        {
            IMessage result = await Registry.ExecuteCommandAsync("notreal", null, null);
            if (result != null)
            {
                Debug.Fail("Result was not null.");
            }
        }

    }
}
