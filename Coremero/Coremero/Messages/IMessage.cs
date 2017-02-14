using System;
using System.Collections.Generic;

namespace Coremero
{
    public interface IMessage
    {
        /// <summary>
        /// The message text contents.
        /// </summary>
        string Text { get; }

        /// <summary>
        /// A collection of all attachments in the message. List order is equal to attachment order.
        /// </summary>
        List<IAttachment> Attachments { get; }

        DateTime Timestamp { get; }
    }
}