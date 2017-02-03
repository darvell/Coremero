using System;
using System.Collections.Generic;
using System.Text;
using Coremero.Commands;
using Coremero.Messages;

namespace Coremero.Plugin.Borat
{
    public class BoratQuote : IPlugin
    {
        [Command("borat")]
        public IMessage Quote(IInvocationContext context, IMessage message)
        {
            return Message.Create("I like!", new FileAttachment("IASIPLaughing.jpg"));
        }


        public void Dispose()
        {
        }
    }
}
