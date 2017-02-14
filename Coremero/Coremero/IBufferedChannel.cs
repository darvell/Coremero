using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Coremero
{
    public interface IBufferedChannel : IChannel
    {
        List<IBufferedMessage> GetLatestMessages(int limit = 100);
        Task<List<IBufferedMessage>> GetLatestMessagesAsync(int limit = 100);
    }
}
