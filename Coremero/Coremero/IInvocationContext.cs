namespace Coremero
{
    public interface IInvocationContext
    {
        /// <summary>
        /// The ISendable object that raised the invocation.
        /// </summary>
        ISendable Raiser { get; }

        /// <summary>
        /// The user that raised invocation.
        /// </summary>
        IUser User { get; }

        /// <summary>
        /// The channel in which the invocation was raised.
        /// </summary>
        ISendable Channel { get; }
    }
}