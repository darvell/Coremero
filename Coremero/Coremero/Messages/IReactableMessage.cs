using System.Collections.Generic;
using System.Threading.Tasks;

namespace Coremero.Messages
{
    public interface IReactableMessage : IMessage
    {
        Task React(string emoji);

        Task<List<Reaction>> GetReactions();
    }
}