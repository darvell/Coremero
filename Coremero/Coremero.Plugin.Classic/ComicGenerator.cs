using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coremero.Attachments;
using Coremero.Commands;
using Coremero.Context;
using Coremero.Messages;
using Coremero.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Coremero.Plugin.Classic
{
    internal class ComicMessage
    {
        public string User { get; set; }
        public string Message { get; set; }
        public long Timestamp { get; set; }
    }

    internal class ComicPayload
    {
        public string Title { get; set; }
        public List<ComicMessage> Messages { get; set; } = new List<ComicMessage>();
    }

    internal class LowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }

    public class ComicGenerator : IPlugin
    {
        private Random _rnd = new Random();
        
        [Command("comic", Arguments = "Title", Help = "Creates a comic using the last random lines of chat.")]
        public async Task<IMessage> GenerateComic(IInvocationContext context, string title)
        {
            int panels = _rnd.Next(1, 4);

            title = title?.Trim() ?? "";
            if(!string.IsNullOrEmpty(title))
            {
                string panelCount = title.Split().LastOrDefault();
                if(int.TryParse(panelCount, out panels)) {
                    title = title.Replace(panelCount, "").Trim();
                    if(panelCount >= 6) {
                        panelCount = 6;
                    }
                }
            }

            if (string.IsNullOrEmpty(title))
            {
                // Force title to null to ensure the payload goes through fine.
                title = null;
            }
            if (context.Channel is IBufferedChannel bufferedChannel)
            {
                List<IBufferedMessage> messages = await bufferedChannel.GetLatestMessagesAsync(30);
                messages = messages.Where(x => !x.Text.IsCommand() && !string.IsNullOrEmpty(x.Text)).ToList();

                ComicPayload payload = new ComicPayload() { Title = title };

                for (int i = 0; i < panels; i++)
                {
                    if (messages.Count <= i)
                    {
                        break;
                    }
                    
                    string curMsg = messages[i].Text;
                    curMsg = string.Join(" ", curMsg.Split().Where(x => !x.StartsWith("<@") && !x.EndsWith(">"))).Trim();

                    payload.Messages.Add(new ComicMessage()
                    {
                        Message = curMsg,
                        Timestamp = messages[i].Timestamp.Ticks,
                        User = messages[i].User.Name
                    });
                }

                payload.Messages.Reverse();

                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(JsonConvert.SerializeObject(payload, new JsonSerializerSettings() { ContractResolver = new LowercaseContractResolver() }), Encoding.UTF8, "application/json");
                    var result = await client.PostAsync($"http://localhost:8000/create", content);
                    MemoryStream imageStream = await (await result.Content.ReadAsStreamAsync()).CopyToMemoryStreamAsync();
                    return Message.Create(null, new StreamAttachment(imageStream, $"{DateTime.Now} {bufferedChannel?.Name} Comic.jpg"));
                }
            }
            throw new Exception("Not a buffered channel.");
        }
    }
}
