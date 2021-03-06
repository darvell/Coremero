﻿using Coremero.Client;
using Coremero.Commands;
using Coremero.Context;
using Coremero.Messages;
using Coremero.Utilities;

namespace Coremero.Plugin.Classic
{
    public class Thinker : IPlugin
    {
        [Command("think", Arguments = "Thought", Help = "( .   __ . ) . o O ( [Thought] )")]
        public string Think(IInvocationContext context, IMessage message)
        {
            string output = $"( .   __ . ) . o O ( {message.Text.TrimCommand()} )";

            if (context.OriginClient.Features.HasFlag(ClientFeature.Markdown))
            {
                output = $"```{output}```";
            }
            return output;
        }
    }
}