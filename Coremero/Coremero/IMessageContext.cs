using Coremero.Client;

namespace Coremero
{
    public interface IMessageContext
    {
        IClient OriginClient { get; }

    }
}
