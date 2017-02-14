using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Messages;
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
                var request = new
                {
                    phrase = message
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                var result = await client.PostAsync("http://awyisser.com/api/generator", content);
                JObject payload = JObject.Parse(await result.Content.ReadAsStringAsync());
                MemoryStream ms = new MemoryStream();
                await (await client.GetStreamAsync(payload["link"].ToString())).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return Message.Create(null, new StreamAttachment(ms, "awwyiss.png"));
            }
        }
    }
}
