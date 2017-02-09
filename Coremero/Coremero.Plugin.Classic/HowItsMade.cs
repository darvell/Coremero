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
    public class HowItsMade : IPlugin
    {
        [Command("howitsmade")]
        public async Task<IMessage> GiphyHIM(IInvocationContext context, IMessage message)
        {
            using (HttpClient client = new HttpClient())
            {
                string json =
                    await client.GetStringAsync(
                        "http://api.giphy.com/v1/gifs/random?api_key=dc6zaTOxFJmzC&tag=how+its+made");

                var jsonObj = JsonConvert.DeserializeObject<JObject>(json);
                var url = jsonObj["data"]["image_url"];

                MemoryStream ms = new MemoryStream();
                await (await client.GetStreamAsync(url.ToString())).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return Message.Create(null, new StreamAttachment(ms, Path.GetFileName(url.ToString())));
            }
        }

    }
}
