using System;
using System.Collections.Generic;
using System.Text;

namespace Coremero
{
    public interface IServer
    {
        string Name { get; }
        IEnumerable<IChannel> Channels { get; }
    }
}