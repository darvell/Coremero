using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coremero
{
    public interface IBufferedChannel : IChannel
    {
        List<IMessage> GetLatestMessages(int limit = 100);
        Task<List<IMessage>> GetLatestMessagesAsync(int limit = 100);
    }
}
