using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Coremero.Client.Mock
{
    public class MockClient : IClient
    {

        #region Private Fields

        private bool _isConnected = false;

        #endregion

        #region Public Interface Properties

        public string Name
        {
            get
            {
                return "Mock Client";
            }
        }

        public string Description
        {
            get
            {
                return "A client used for mocking, testing, et al.";
            }
        }

        public ClientFeature Features
        {
            get
            {
                return ClientFeature.All;
            }
        }

        public bool IsConnected => _isConnected;

        #endregion

        public Task Connect()
        {
            _isConnected = true;
            return Task.FromResult(true);
        }

        public Task Disconnect()
        {
            if (!_isConnected)
            {
                return Task.FromException(new InvalidOperationException("Client is not connected."));
            }

            _isConnected = false;
            return Task.FromResult(true);
        }

        public event EventHandler<Exception> Error;

        public void MockDisconnectError()
        {
            if (_isConnected)
            {
                _isConnected = false;
                Error?.Invoke(this, new SocketException());
            }
        }
    }
}
