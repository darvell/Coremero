using System.Collections.Generic;

namespace Coremero
{
    public interface IChannel : ISendable
    {
        string Name { get; }
        string Topic { get; }
        IEnumerable<IUser> Users { get; }
    }
}