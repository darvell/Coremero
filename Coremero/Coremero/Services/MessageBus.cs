using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Coremero.Services
{
    public class MessageBus : IMessageBus
    {
        public event EventHandler<MessageSentEventArgs> Sent;
        public event EventHandler<MessageReceivedEventArgs> Received;

        private readonly Stopwatch _incomingStopwatch = new Stopwatch();
        private readonly Stopwatch _outgoingStopwatch = new Stopwatch();

        public void RaiseIncoming(IInvocationContext context, IMessage message)
        {
            Log.Trace($"[{context.OriginClient.Name} @ {context.Channel.Name}] {context.User.Name}: {message.Text}");
            Received?.Invoke(this, new MessageReceivedEventArgs(context, message));
        }

        public void RaiseOutgoing(ISendable target, IMessage message)
        {
            Log.Trace($"[OUT (A {message.Attachments?.Count}] {message.Text}");
            Sent?.Invoke(this, new MessageSentEventArgs(target, message));
            _outgoingStopwatch.Reset();
        }
    }
}
