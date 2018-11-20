using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Coremero.Attachments;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Utilities;
using HtmlAgilityPack;

namespace Coremero.Plugin.Playground
{
    public class Shutterstock : IPlugin
    {
        private const string SHUTTERSTOCK_SEARCH_URL = @"https://www.shutterstock.com/search?searchterm={0}&image_type=all";
        private const string SHUTTERSTOCK_CDN_URL = @"https://image.shutterstock.com/z/";

        [Command("stock", Arguments = "Query", Help = "Gets an image from shutterstock using [Query].")]
        public async Task<IMessage> StockImage(string query)
        {
            using (HttpClient client = new HttpClient())
            {
                var page =
                    await client.GetStringAsync(string.Format(SHUTTERSTOCK_SEARCH_URL, WebUtility.UrlEncode(query)));

                List<string> imageNames = new List<string>();

                var document = new HtmlDocument();
                document.LoadHtml(page);
                foreach (var image in document.DocumentNode.SelectNodes("//img"))
                {
                    var src = image.GetAttributeValue("src", null);
                    if (src.Contains("thumb"))
                    {
                        imageNames.Add(Path.GetFileName(src));
                    }
                }

                var imageName = imageNames.GetRandom();

                MemoryStream ms = new MemoryStream();
                Stream s = await client.GetStreamAsync(SHUTTERSTOCK_CDN_URL + imageName);
                await s.CopyToAsync(ms);
                ms.Seek(0, SeekOrigin.Begin);
                return Message.Create(null, new StreamAttachment(ms, imageName));
            }
        }
    }
}