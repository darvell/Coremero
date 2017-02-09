using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coremero.Plugin.Classic
{
    public class HowItsMade : IPlugin
    {
        [Command("giphy")]
        public async Task<IMessage> Giphy(IInvocationContext context, IMessage message)
        {
            using (HttpClient client = new HttpClient())
            {
                string json =
                    await client.GetStringAsync($"http://api.giphy.com/v1/gifs/random?api_key=dc6zaTOxFJmzC&tag={WebUtility.HtmlEncode(message.Text.TrimCommand())}");

                var jsonObj = JsonConvert.DeserializeObject<JObject>(json);
                var url = jsonObj["data"]["image_url"];

                MemoryStream ms = new MemoryStream();
                await (await client.GetStreamAsync(url.ToString())).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return Message.Create(null, new StreamAttachment(ms, Path.GetFileName(url.ToString())));
            }
        }

        [Command("howitsmade")]
        public async Task<IMessage> RandomHowItsMade(IInvocationContext context, IMessage message)
        {
            return await Giphy(context, Message.Create("hows its made"));
        }

    }
}
