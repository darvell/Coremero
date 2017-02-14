using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Coremero.Services;

namespace Coremero.Client.Mock
{
    public class MockUser : IUser
    {
        private List<IMessage> _messageBuffer = new List<IMessage>();
        private IMessageBus _messageBus;

        public MockUser(IMessageBus messageBus, string name)
        {
            _messageBus = messageBus;
            Name = name;
        }

        public Task SendAsync(IMessage message)
        {
            _messageBuffer.Add(message);

            return Task.FromResult(true);
        }

        public void Send(IMessage message)
        {
            SendAsync(message);
        }

        public string Name { get; private set; }

        public string Mention
        {
            get { return $"{Name}: "; }
        }
    }
}