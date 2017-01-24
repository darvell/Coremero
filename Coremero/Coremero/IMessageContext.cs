using System;
using System.Collections.Generic;
using System.Text;
using Coremero.Client;

namespace Coremero
{
    public interface IMessageContext
    {
        IClient OriginClient { get; }

    }
}
