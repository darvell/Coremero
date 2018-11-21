using System;
using System.Text;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Context;
using Coremero.Messages;

namespace Coremero.Plugin.Classic
{
    public class YearsAgo : IPlugin
    {
        [Command("1", Help = "One year ago...")]
        [Command("2", Help = "Two years ago...")]
        [Command("3", Help = "Three years ago...")]
        [Command("4", Help = "Four years ago...")]
        public async Task<string> YearsAgoAsync(IInvocationContext context, IMessage cmdMessage)
        {
            if (context.Channel is IBufferedChannel channel)
            {
                int yearsAgo = int.Parse(cmdMessage.Text[1].ToString());
                var messages = await channel.GetMessagesAsync(DateTimeOffset.Now - TimeSpan.FromDays(356 * yearsAgo), SearchDirection.After,
                    4);
                StringBuilder builder = new StringBuilder();

                builder.AppendLine("```");

                foreach (var message in messages)
                {
                    builder.AppendLine($"{message.User.Name}: {message.Text.Replace("```", "` ` `")}");
                }

                builder.AppendLine("```");
                return builder.ToString();
            }

            throw new InvalidOperationException("This isn't from a buffered channel. Go away.");
        }
    }
}