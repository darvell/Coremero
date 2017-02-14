using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coremero.Commands;

namespace Coremero.Test
{
    public class MockPlugin : IPlugin
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

        [Command("echo")]
        public IMessage Echo(IInvocationContext context, IMessage message)
        {
            return message;
        }

        public void Dispose()
        {
            // ignore
        }
    }
}