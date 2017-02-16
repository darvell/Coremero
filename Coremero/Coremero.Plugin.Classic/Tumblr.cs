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

        private Dictionary<string, TumblrImageUrlCache> _tumblrImageUrlCaches =
            new Dictionary<string, TumblrImageUrlCache>();

        public Tumblr(ICredentialStorage credentialStorage)
        {
            TUMBLR_API_KEY = credentialStorage.GetKey("tumblr", "fuiKNFp9vQFvjLNvx4sUwti4Yb5yGutBN4Xh10LXZhhRKjWlV4");
                // Public testing API key from Tumblr.
        }

        private async Task<Tuple<Stream, string>> GetRandomTumblrImage(string tumblrUsername)
        {
            if (!_tumblrImageUrlCaches.ContainsKey(tumblrUsername))
            {
                _tumblrImageUrlCaches[tumblrUsername] = new TumblrImageUrlCache(tumblrUsername, TUMBLR_API_KEY,
                    TimeSpan.FromHours(12));
            }

            string imageUrl = await _tumblrImageUrlCaches[tumblrUsername].Pop();

            // Store image in RAM and pass back.
            MemoryStream imageStream = await _httpClient.GetStreamAndBufferToMemory(imageUrl);

            // TODO: C# 7.0 when VS15 is RTM.
            return new Tuple<Stream, string>(imageStream, imageUrl);
        }

        [Command("homero", Help = "Obtener una imagen aleatoria de Los Simpson.")]
        public async Task<IMessage> Homero(IInvocationContext context, string message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("simpsons-latino");
            return Message.Create(message,
                new StreamAttachment(image.Item1, $"homero.{Path.GetExtension(image.Item2)}"));
        }

        [Command("dog", Help = "Get a random image of a dog.")]
        public async Task<IMessage> Dog(IInvocationContext context, string message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("goodassdog");
            return Message.Create(message, new StreamAttachment(image.Item1, $"good_pupper.{Path.GetExtension(image.Item2)}"));
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

        [Command("ceo", Help = "Get a random image of a business man.")]
        public async Task<IMessage> RealBusinessMan(IInvocationContext context, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                IUser randomUser = context.Channel?.Users.GetRandom();
                message = $"{_businessTitles.GetRandom()} {randomUser?.Name} hard at work.";
            }
            return Message.Create(message,
                new StreamAttachment((await GetRandomTumblrImage("realbusinessmen")).Item1, "white_male_over_50.jpg"));
        }

        [Command("y2k", Help = "Get a random image of ａｅｓｔｈｅｔｉｃ.")]
        public async Task<IMessage> Y2K(IInvocationContext context, string message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("y2kaestheticinstitute");
            return Message.Create(message,
                new StreamAttachment(image.Item1, $"aesthetics.{Path.GetExtension(image.Item2)}"));
        }

        [Command("koth", Help = "Get a random image from King of the Hill.")]
        public async Task<IMessage> KingOfTheHill(IInvocationContext context, string message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("kingofthehillcaps");
            return Message.Create(message,
                new StreamAttachment(image.Item1, $"propane.{Path.GetExtension(image.Item2)}"));
        }

        [Command("xfiles", Help = "Get a random image from The X-Files.")]
        public async Task<IMessage> XFiles(IInvocationContext context, string message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("outofcontextfiles");
            return Message.Create(message,
                new StreamAttachment(image.Item1, $"alien.{Path.GetExtension(image.Item2)}"));
        }

        [Command("spongebob", Help = "Get a random image from SpongeBob SquarePants.")]
        public async Task<IMessage> Spongebob(IInvocationContext context, string message)
        {
            Tuple<Stream, string> image = await GetRandomTumblrImage("spongecaps");
            return Message.Create(message,
                new StreamAttachment(image.Item1, $"spongebob.{Path.GetExtension(image.Item2)}"));
        }


        [Command("tumblrcache", MinimumPermissionLevel = UserPermission.BotOwner)]
        public async Task<string> CacheSize(IInvocationContext context, IMessage message)
        {
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