﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coremero.Attachments;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Utilities;
using HtmlAgilityPack;

namespace Coremero.Plugin.Classic
{
    public class WorkInProgress : IPlugin
    {
        private List<string> _wipUrls = new List<string>();

        [Command("wip", Help = "Returns an under construction GIF.")]
        public async Task<IMessage> WorkInProgressGif()
        {
            using (HttpClient client = new HttpClient())
            {
                if (_wipUrls.Count == 0)
                {
                    HtmlDocument document = new HtmlDocument();
                    document.Load(await client.GetStreamAsync("http://www.textfiles.com/underconstruction/"));
                    var nodes = document.DocumentNode.SelectNodes("//img/@src");
                    _wipUrls.AddRange(nodes.Select(x => x.GetAttributeValue("src", "help")));
                }

                string imageName = _wipUrls.GetRandom();

                MemoryStream imageStream =
                    await client.GetStreamAndBufferToMemory($"http://www.textfiles.com/underconstruction/{imageName}");
                return Message.Create(null, new StreamAttachment(imageStream, imageName));
            }
        }
    }
}