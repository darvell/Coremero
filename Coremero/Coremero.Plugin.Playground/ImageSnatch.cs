using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coremero.Attachments;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Utilities;

namespace Coremero.Plugin.Playground
{
    public class ImageSnatch : IPlugin
    {
        [Command("geturl")]
        public async Task<IMessage> GetUrl(string url)
        {
            if (!url.StartsWith("http") && !url.Contains("127.0.0.1") && !url.Contains("//localhost"))
            {
                throw new ArgumentException("Not a HTTP/S URL. You trying to be sneaky?");
            }

            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync(url);
                if (result.IsSuccessStatusCode && result.Content.Headers.ContentLength < 6000000)
                {
                    return Message.Create(null,
                        new StreamAttachment(await (await result.Content.ReadAsStreamAsync()).CopyToMemoryStreamAsync(), result.Content.Headers.ContentDisposition.FileName));
                }
            }

            throw new Exception("Can't get content.");
        }
    }
}
