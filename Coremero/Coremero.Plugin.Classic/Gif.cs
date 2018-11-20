using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Coremero.Attachments;
using Coremero.Commands;
using Coremero.Context;
using Coremero.Messages;
using Coremero.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Coremero.Plugin.Classic
{
    public class Gif : IPlugin
    {
        [Command("gif", Arguments = "Query", Help = "Get a random GIF from Riffy.")]
        public async Task<IMessage> Riffsy(IInvocationContext context, string message)
        {
            using (HttpClient client = new HttpClient())
            {
                string json =
                    await client.GetStringAsync(
                        $"http://api.riffsy.com/v1/search?key=KXSAYTVBST24&limit=50&tag={WebUtility.UrlEncode(message)}");

                var jsonObj = JsonConvert.DeserializeObject<JObject>(json);
                var url = jsonObj["results"].Where(x =>
                {
                    try
                    {
                        return (long)x["media"][0]["gif"]["size"] < 6000000;
                    }
                    catch
                    {
                        return false;
                    }
                }).GetRandom()["media"][0]["gif"]["url"].ToString();

                MemoryStream imageStream = await client.GetStreamAndBufferToMemory(url);
                return Message.Create(null, new StreamAttachment(imageStream, message + ".gif"));
            }
        }

        [Command("giphy", Arguments = "Query", Help = "Get a random GIF from Giphy.")]
        public async Task<IMessage> Giphy(IInvocationContext context, string message)
        {
            using (HttpClient client = new HttpClient())
            {
                string json =
                    await client.GetStringAsync(
                        $"http://api.giphy.com/v1/gifs/random?api_key=dc6zaTOxFJmzC&tag={WebUtility.UrlEncode(message)}");

                var jsonObj = JsonConvert.DeserializeObject<JObject>(json);
                var url = jsonObj["data"]["image_url"].ToString();

                MemoryStream imageStream = await client.GetStreamAndBufferToMemory(url);
                return Message.Create(null, new StreamAttachment(imageStream, Path.GetFileName(url)));
            }
        }

        [Command("howitsmade", Help = "Get a random How It's Made GIF.")]
        public async Task<IMessage> RandomHowItsMade(IInvocationContext context, IMessage message)
        {
            return await Giphy(context, "hows its made");
        }
    }
}