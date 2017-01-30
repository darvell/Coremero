using System;
using System.Collections.Generic;
using System.Text;

namespace Coremero.Services
{
    public interface IEventAggregator
    {
        event EventHandler TopicChanged;
        event EventHandler CommandRequested;
        event EventHandler CommandFired;
    }
}
