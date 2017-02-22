using System.Collections.Generic;

namespace Coremero
{
    public interface IServer
    {
        string Name { get; }
        IEnumerable<IChannel> Channels { get; }
    }
}