using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coremero.Attachments;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coremero.Plugin.Classic
{
    public class AwwYiss : IPlugin
    {
        [Command("awwyiss", Arguments = "Text", Help = "Generates a comic with awwyisser.com using [Text].")]
        public async Task<IMessage> AwwYissGenerator(string message)
        {
            using (HttpClient client = new HttpClient())
            {
                if (message.Length > 44)
                {
                    message = message.Substring(0, 43);
                }
                var request = new
                {
                    phrase = message
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = await client.PostAsync("http://awyisser.com/api/generator", content);
                JObject payload = JObject.Parse(await result.Content.ReadAsStringAsync());
                MemoryStream imageStream = await client.GetStreamAndBufferToMemory(payload["link"].ToString());
                return Message.Create(null, new StreamAttachment(imageStream, "awwyiss.png"));
            }
        }
    }
}