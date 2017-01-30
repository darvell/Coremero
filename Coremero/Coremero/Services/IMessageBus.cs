using System;
using System.Collections.Generic;
using System.Text;

namespace Coremero.Services
{
    public interface IMessageBus
    {
        event EventHandler Sent;
        event EventHandler Received;

        void RaiseIncoming();
        void RaiseOutgoing();
    }
}
