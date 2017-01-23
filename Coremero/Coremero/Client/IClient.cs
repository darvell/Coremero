using System;
using System.Threading.Tasks;

namespace Coremero.Client
{
    public interface IClient
    {
        string Name { get; }

        string Description { get; }

        ClientFeature Features { get; }

        bool IsConnected { get; }

        Task Connect();

        Task Disconnect();

        /// <summary>
        /// Event that fires when a client disconnects for an unknown reason.
        /// </summary>
        event EventHandler<Exception> Error;


        //List<IServer> Servers { get; }

        //event EventHandler<MessageEventArgs> MessageReceived;

        //event EventHandler<MessageEventArgs> MessageSent;
    }
}
