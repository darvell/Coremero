using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class Gif : IPlugin
    {
        /*
        [Command("rightgif")]
        public async Task<IMessage> RightGif(IInvocationContext context, IMessage message)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:19.0) Gecko/20100101 Firefox/19.0");

                var request = new
                {
                    text = message.Text.TrimCommand()
                };

                var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");

                var result = await client.PostAsync($"http://rightgif.com/search/web", content);

                var jsonObj = JsonConvert.DeserializeObject<JObject>(await result.Content.ReadAsStringAsync());
                var url = jsonObj["url"];

                MemoryStream ms = new MemoryStream();
                await (await client.GetStreamAsync(url.ToString())).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return Message.Create(null, new StreamAttachment(ms, Path.GetFileName(url.ToString())));
            }
        }
        */

        [Command("gif", Arguments = "Query", Help = "Get a random GIF from Riffy.")]
        public async Task<IMessage> Riffsy(IInvocationContext context, IMessage message)
        {
            using (HttpClient client = new HttpClient())
            {
                string json =
                    await client.GetStringAsync(
                        $"http://api.riffsy.com/v1/search?key=KXSAYTVBST24&limit=50&tag={WebUtility.UrlEncode(message.Text.TrimCommand())}");

                var jsonObj = JsonConvert.DeserializeObject<JObject>(json);
                var smallGifs = jsonObj["results"].Where(x =>
                {
                    try
                    {
                        return (long) x["media"][0]["gif"]["size"] < 6000000;
                    }
                    catch
                    {
                        return false;
                    }
                });

                if (!smallGifs.Any())
                {
                    return null;
                }

                var url = smallGifs.GetRandom()?["media"][0]["gif"]["url"];

                MemoryStream ms = new MemoryStream();
                await (await client.GetStreamAsync(url.ToString())).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return Message.Create(null, new StreamAttachment(ms, message.Text.TrimCommand() + ".gif"));
            }
        }

        [Command("giphy", Arguments = "Query", Help = "Get a random GIF from Giphy.")]
        public async Task<IMessage> Giphy(IInvocationContext context, IMessage message)
        {
            using (HttpClient client = new HttpClient())
            {
                string json =
                    await client.GetStringAsync(
                        $"http://api.giphy.com/v1/gifs/random?api_key=dc6zaTOxFJmzC&tag={WebUtility.UrlEncode(message.Text.TrimCommand())}");

                var jsonObj = JsonConvert.DeserializeObject<JObject>(json);
                var url = jsonObj["data"]["image_url"];

                MemoryStream ms = new MemoryStream();
                await (await client.GetStreamAsync(url.ToString())).CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return Message.Create(null, new StreamAttachment(ms, Path.GetFileName(url.ToString())));
            }
        }

        [Command("howitsmade", Help = "Get a random How It's Made GIF.")]
        public async Task<IMessage> RandomHowItsMade(IInvocationContext context, IMessage message)
        {
            return await Giphy(context, Message.Create("hows its made"));
        }
    }
}