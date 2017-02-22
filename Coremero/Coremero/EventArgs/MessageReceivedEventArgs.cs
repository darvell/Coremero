using Coremero.Context;
using Coremero.Messages;

namespace Coremero
{
    public class MessageReceivedEventArgs : System.EventArgs
    {
        public IInvocationContext Context { get; private set; }
        public IMessage Message { get; private set; }

        public MessageReceivedEventArgs(IInvocationContext context, IMessage message)
        {
            Context = context;
            Message = message;
        }
    }
}