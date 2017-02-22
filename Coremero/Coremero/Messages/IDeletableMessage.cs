using System.Threading.Tasks;

namespace Coremero.Messages
{
    public interface IDeletableMessage : IMessage
    {
        Task DeleteAsync();
    }
}