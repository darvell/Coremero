namespace Coremero
{
    /// <summary>
    /// Any object that has a unique ID.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The ID of the entity.
        /// </summary>
        ulong ID { get; }
    }
}