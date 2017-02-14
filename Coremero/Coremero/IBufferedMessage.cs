using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Coremero
{
    public interface IBufferedMessage : IMessage
    {
        IUser User { get; }
    }
}
