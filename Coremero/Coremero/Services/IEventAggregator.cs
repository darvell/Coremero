using System;

namespace Coremero.Services
{
    public interface IEventAggregator
    {
        event EventHandler TopicChanged;

        event EventHandler CommandRequested;

        event EventHandler CommandFired;
    }
}