using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Utilities;

namespace Coremero.Plugin.Playground
{
    public class ImageSnatch
    {
        [Command("geturl")]
        public async Task<IMessage> GetUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                var result = await client.GetAsync(url);
                if (result.IsSuccessStatusCode && result.Content.Headers.ContentLength < 6000000)
                {
                    return Message.Create(null,
                        new StreamAttachment(await (await result.Content.ReadAsStreamAsync()).CopyToMemoryStreamAsync(), Path.GetFileName(url)));
                }
            }

            throw new Exception("Can't get content.");
        }
    }
}
