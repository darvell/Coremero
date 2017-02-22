using System;
using Coremero.Context;
using Coremero.Messages;

namespace Coremero.Services
{
    public interface IMessageBus
    {
        event EventHandler<MessageSentEventArgs> Sent;
        event EventHandler<MessageReceivedEventArgs> Received;

        void RaiseIncoming(IInvocationContext context, IMessage message);
        void RaiseOutgoing(ISendable target, IMessage message);
    }
}