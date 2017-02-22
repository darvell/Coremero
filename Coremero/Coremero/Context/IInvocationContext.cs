using Coremero.Client;

namespace Coremero.Context
{
    public interface IInvocationContext
    {
        /// <summary>
        /// The client (message source) that raised the invocation.
        /// </summary>
        IClient OriginClient { get; }

        /// <summary>
        /// The ISendable object that raised the invocation.
        /// </summary>
        ISendable Raiser { get; }

        /// <summary>
        /// The user that raised invocation.
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// The channel in which the invocation was raised.
        /// </summary>
        IChannel Channel { get; }
    }
}