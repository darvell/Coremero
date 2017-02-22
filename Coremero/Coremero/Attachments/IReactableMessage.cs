using System.Threading.Tasks;
using Coremero.Messages;

namespace Coremero.Attachments
{
    public interface IReactableMessage : IMessage
    {
        Task React(string emoji);
    }
}