using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Messages;
using Coremero.Plugin.Classic.TumblrJson;
using Coremero.Storage;
using Newtonsoft.Json;
using Coremero.Utilities;

namespace Coremero.Plugin.Classic
{
    public class Tumblr : IPlugin, IDisposable
    {
        private readonly string TUMBLR_API_KEY;

        private HttpClient _httpClient = new HttpClient();
        private Dictionary<string, TumblrImageUrlCache> _tumblrImageUrlCaches = new Dictionary<string, TumblrImageUrlCache>();

        public Tumblr(ICredentialStorage credentialStorage)
        {
            TUMBLR_API_KEY = credentialStorage.GetKey("tumblr", "fuiKNFp9vQFvjLNvx4sUwti4Yb5yGutBN4Xh10LXZhhRKjWlV4"); // Public testing API key from Tumblr.
        }

        private async Task<Tuple<Stream, string>> GetRandomTumblrImage(string tumblrUsername)
        {
            if (!_tumblrImageUrlCaches.ContainsKey(tumblrUsername))
            {
                _tumblrImageUrlCaches[tumblrUsername] = new TumblrImageUrlCache(tumblrUsername, TUMBLR_API_KEY, TimeSpan.FromHours(12));
            }

            string imageUrl = await _tumblrImageUrlCaches[tumblrUsername].Pop();

            // Store image in RAM and pass back.
            MemoryStream ms = new MemoryStream();
            using (Stream httpImageStream = await _httpClient.GetStreamAsync(imageUrl))
            {
                httpImageStream.CopyTo(ms);
            }
            ms.Seek(0, SeekOrigin.Begin);
            // TODO: C# 7.0 when VS15 is RTM.
            return new Tuple<Stream, string>(ms, imageUrl);
        }

        [Command("homero")]
        public async Task<IMessage> Homero(IInvocationContext context, IMessage message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("simpsons-latino");
            return Message.Create(message.Text?.TrimCommand(), new StreamAttachment(image.Item1, $"homero.{Path.GetExtension(image.Item2)}"));
        }

        [Command("dog")]
        public async Task<IMessage> Dog(IInvocationContext context, IMessage message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("goodassdog");
            return Message.Create(message.Text?.TrimCommand(), new StreamAttachment(image.Item1, $"good_pupper.{Path.GetExtension(image.Item2)}"));
        }

        #region Business Titles

        List<string> _businessTitles = new List<string>()
        {
            "CEO",
            "CFO",
            "HR Strong Boy",
            "Executive Pillow Man",
            "Quake Live Developer",
            "Senior Executive Backend Engineer",
            "Sad Full Stack Lad",
            "Hard Working Middle Management Sad Sack",
            "Child of CEO",
            "Nepotism Hire"
        };

        #endregion

        [Command("ceo")]
        public async Task<IMessage> RealBusinessMan(IInvocationContext context, IMessage message)
        {
            string outputText = message.Text?.TrimCommand();
            if (string.IsNullOrEmpty(outputText))
            {
                IUser randomUser = context.Channel?.Users.GetRandom();
                outputText = $"{_businessTitles.GetRandom()} {randomUser?.Name} hard at work.";
            }
            return Message.Create(outputText, new StreamAttachment((await GetRandomTumblrImage("realbusinessmen")).Item1, "white_male_over_50.jpg"));
        }

        [Command("y2k")]
        public async Task<IMessage> Y2K(IInvocationContext context, IMessage message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("y2kaestheticinstitute");
            return Message.Create(message.Text?.TrimCommand(), new StreamAttachment(image.Item1, $"aesthetics.{Path.GetExtension(image.Item2)}"));
        }

        [Command("koth")]
        public async Task<IMessage> KingOfTheHill(IInvocationContext context, IMessage message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("kingofthehillcaps");
            return Message.Create(message.Text?.TrimCommand(), new StreamAttachment(image.Item1, $"propane.{Path.GetExtension(image.Item2)}"));
        }

        [Command("xfiles")]
        public async Task<IMessage> XFiles(IInvocationContext context, IMessage message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("outofcontextfiles");
            return Message.Create(message.Text?.TrimCommand(), new StreamAttachment(image.Item1, $"alien.{Path.GetExtension(image.Item2)}"));
        }

        [Command("spongebob")]
        public async Task<IMessage> Spongebob(IInvocationContext context, IMessage message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("spongecaps");
            return Message.Create(message.Text?.TrimCommand(), new StreamAttachment(image.Item1, $"spongebob.{Path.GetExtension(image.Item2)}"));
        }


        [Command("tumblrcache")]
        public async Task<string> CacheSize(IInvocationContext context, IMessage message)
        {
            if (context.User.Permissions != UserPermission.BotOwner)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, TumblrImageUrlCache> cache in _tumblrImageUrlCaches)
            {
                sb.AppendLine($"{cache.Key}: {(await cache.Value.GetImagesAsync()).Count()} images.");
            }
            return sb.ToString();
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
