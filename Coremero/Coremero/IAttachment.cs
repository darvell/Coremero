using System.IO;

namespace Coremero
{
    public interface IAttachment
    {
        /// <summary>
        /// The name of the attachment, generally a filename.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The attachment contents.
        /// </summary>
        Stream Contents { get; }
    }
}