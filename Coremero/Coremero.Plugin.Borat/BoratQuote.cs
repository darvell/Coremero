using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Utilities;

namespace Coremero.Plugin.Borat
{
    public class BoratQuote : IPlugin
    {
        [Command("borat", Help = "I like!")]
        public IMessage Quote()
        {
            return Message.Create("I like!",
                new FileAttachment(Path.Combine(PathExtensions.ResourceDir, "IASIPLaughing.jpg")));
        }
    }
}