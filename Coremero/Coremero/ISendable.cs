using System.Threading.Tasks;

namespace Coremero
{
    public interface ISendable
    {
        /// <summary>
        /// Sends a message to the target asynchronously and can be awaited on until the server confirms that the message has been handled successfully.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <returns>Awaitable task.</returns>
        Task SendAsync(IMessage message);

        /// <summary>
        /// Sends a message to the target in a fire-and-forget process.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        void Send(IMessage message);
    }
}