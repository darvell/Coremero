using System;
using System.Collections.Generic;
using System.Text;

namespace Coremero
{
    public interface IServer
    {
        IEnumerable<IChannel> Channels { get; }
    }
}
