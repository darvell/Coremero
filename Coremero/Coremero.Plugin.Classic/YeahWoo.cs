using Coremero.Messages;
using Coremero.Services;

namespace Coremero.Plugin.Classic
{
    public class YeahWoo : IPlugin
    {
        public YeahWoo(IMessageBus messageBus)
        {
            messageBus.Received += MessageBus_Received;
        }

        private async void MessageBus_Received(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message.Text.ToLower().Equals("yeah"))
            {
                await e.Context.Raiser.SendAsync(Message.Create("Woo!"));
            }
            else if (e.Message.Text.ToLower().Equals("woo"))
            {
                await e.Context.Raiser.SendAsync(Message.Create("Yeah!"));
            }
        }
    }
}