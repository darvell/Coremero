using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coremero.Attachments;
using Coremero.Utilities;

namespace Coremero.Messages
{
    public class Message : IMessage
    {
        public string Text { get; set; }
        public List<IAttachment> Attachments { get; set; }
        public DateTime Timestamp { get; private set; } = DateTime.Now;

        public static Message Create(string text, params IAttachment[] attachments)
        {
            Message message = new Message()
            {
                Text = text,
                Attachments = attachments?.ToList()
            };
            return message;
        }

        public static Message Create(string text)
        {
            return Message.Create(text, null);
        }
        public static async Task<Message> CreateFromUrlAsync(string url)
        {
            Message result = new Message() 
            {
                Attachments = new List<IAttachment>()
            };

            using(HttpClient client = new HttpClient())
            {
                Stream fileStream = await client.GetStreamAndBufferToMemory(url);
                result.Attachments.Add(new StreamAttachment(fileStream ,Path.GetFileName(url)));
            }

            return result;
        }

    }
}