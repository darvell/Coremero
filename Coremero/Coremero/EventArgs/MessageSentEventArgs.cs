using Coremero.Messages;

namespace Coremero
{
    public class MessageSentEventArgs
    {
        public IMessage Message { get; private set; }
        public ISendable Target { get; private set; }

        public MessageSentEventArgs(ISendable target, IMessage message)
        {
            Target = target;
            Message = message;
        }
    }
}