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
#if DEBUG
            Debug.WriteLine($"[{context.Channel.Name}] {context.User.Name}: {message.Text}");
#endif
            _incomingStopwatch.Start();
            Received?.Invoke(this, new MessageReceivedEventArgs(context, message));
            _incomingStopwatch.Stop();
            if (_incomingStopwatch.ElapsedMilliseconds > 100)
            {
                Debug.WriteLine($"Incoming message bus took {_incomingStopwatch.ElapsedMilliseconds}ms!");
            }
            _incomingStopwatch.Reset();
        }

        public void RaiseOutgoing(ISendable target, IMessage message)
        {
            _outgoingStopwatch.Start();
            Sent?.Invoke(this, new MessageSentEventArgs(target, message));
            _outgoingStopwatch.Stop();
            if (_outgoingStopwatch.ElapsedMilliseconds > 100)
            {
                Debug.WriteLine($"Outgoing message bus took {_incomingStopwatch.ElapsedMilliseconds}ms!");
            }
            _outgoingStopwatch.Reset();
        }
    }
}
