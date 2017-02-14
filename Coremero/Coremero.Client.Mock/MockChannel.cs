using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coremero.Client.Mock
{
    public class MockChannel : IChannel
    {
        private List<Tuple<IUser, IMessage>> _messages = new List<Tuple<IUser, IMessage>>();

        public Task SendAsync(IMessage message)
        {
            return Task.FromResult(true);
        }

        public void Send(IMessage message)
        {
            throw new NotImplementedException();
        }

        public string Name { get; }
        public string Topic { get; }
        public IEnumerable<IUser> Users { get; }
    }
}