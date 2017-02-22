using System.Collections.Generic;
using System.Threading.Tasks;
using Coremero.Messages;

namespace Coremero
{
    public interface IBufferedChannel : IChannel
    {
        List<IBufferedMessage> GetLatestMessages(int limit = 100);
        Task<List<IBufferedMessage>> GetLatestMessagesAsync(int limit = 100);
    }
}
