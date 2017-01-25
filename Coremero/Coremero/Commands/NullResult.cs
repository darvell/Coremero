namespace Coremero.Commands
{
    /// <summary>
    /// For usage in long-running tasks.
    /// </summary>
    public class NullResult : IResult
    {
        public static NullResult Empty()
        {
            return new NullResult();
        }
    }
}
