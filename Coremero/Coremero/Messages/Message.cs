using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coremero.Messages
{
    public class Message : IMessage
    {
        public string Text { get; set; }
        public List<IAttachment> Attachments { get; set; }
        public DateTime Timestamp { get; private set; }

        public static Message Create(string text, params IAttachment[] attachments)
        {
            Message message = new Message()
            {
                Text = text,
                Attachments = attachments?.ToList()
            };
            message.Timestamp = DateTime.Now;
            return message;
        }

        public static Message Create(string text)
        {
            return Message.Create(text, null);
        }

    }
}