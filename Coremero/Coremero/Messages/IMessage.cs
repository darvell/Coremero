using System;
using System.Collections.Generic;
using Coremero.Attachments;

namespace Coremero.Messages
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