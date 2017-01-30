using System;
using System.Collections.Generic;
using System.Text;

namespace Coremero.Client.Mock
{
    public class MockServer : IServer
    {
        public MockServer(string name)
        {
            Name = name;
        }

        public void AddChannel(string name)
        {
            
        }

        public string Name { get; private set; }
        public IEnumerable<IChannel> Channels { get; }
    }
}
