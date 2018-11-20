namespace Coremero.Messages
{
    public interface IBufferedMessage : IMessage
    {
        IUser User { get; }
    }
}