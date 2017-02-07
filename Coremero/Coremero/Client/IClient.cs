using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coremero.Client
{
    public interface IClient
    {
        /// <summary>
        /// The name of the client, generally the supported protocol. (e.g. IRC, Discord, et al.)
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A description of the client.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// What features the client supports.
        /// </summary>
        ClientFeature Features { get; }

        /// <summary>
        /// Returns true if the client is connected to the appropriate backend, else false.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Connects a client asynchronously.
        /// </summary>
        /// <returns>Task for the connection process.</returns>
        Task Connect();

        /// <summary>
        /// Disconnects a client asynchronously, throws an exception if not connected.
        /// </summary>
        /// <returns>Task for the disconnection process.</returns>
        Task Disconnect();

        /// <summary>
        /// Event that fires when a client disconnects for an unknown reason.
        /// </summary>
        event EventHandler<Exception> Error;

        /// <summary>
        /// An enumerable list of what servers the client is currently connected to.
        /// </summary>
        IEnumerable<IServer> Servers { get; }
    }
}
