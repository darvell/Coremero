namespace Coremero
{
    public interface IChannelTypingIndicator
    {
        void SetTyping(bool isTyping);

        bool IsTyping { get; }
    }
}