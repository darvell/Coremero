using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Utilities;
using Newtonsoft.Json.Linq;

namespace Coremero.Plugin.Classic
{
    public class Reddit : IPlugin
    {
        [Command("alligator", Help = "Impersonate alligator.")]
        public async Task<string> BritishProblems(IInvocationContext context, IMessage message)
        {
            bool format = message.Text.GetCommandArguments().Count == 0;

            return (format ? "<alligator>" : "") + await GetRandomTitleFromSubreddit("britishproblems");
        }

        #region Hurt

        private List<string> _hurtSubreddits = new List<string>()
        {
            "Buddhism",
            "explainlikeimfive",
            "gifs",
            "Eve",
            "weightlifting",
            "MensRights",
            "hardbodies",
            "DoesAnybodyElse",
            "The_Donald",
            "4chan",
            "AskTrumpSupporters"

        };

        [Command("hurt", Help = "Impersonate hurt.")]
        public async Task<string> Hurt(IInvocationContext context, IMessage message)
        {
            bool format = message.Text.GetCommandArguments().Count == 0;

            return (format ? "<Hurt>" : "") + await GetRandomTitleFromSubreddit(_hurtSubreddits.GetRandom());
        }

        #endregion

        #region Hyle

        private List<string> _hyleSubreddits = new List<string>()
        {
            "conspiracy",
            "The_Donald",
            "suicidewatch",
            "actualconspiracies",
            "paranormal",
            "mormon"
        };

        // TODO: Hyle is supposed to be a markov chain of multiple posts.
        [Command("hyle", Help = "Impersonate hyle.")]
        public async Task<string> Hyle(IInvocationContext context, IMessage message)
        {
            bool format = message.Text.GetCommandArguments().Count == 0;

            return (format ? "<hyle>" : "") + await GetRandomTitleFromSubreddit(_hyleSubreddits.GetRandom());
        }

        #endregion


        #region Danl
        private List<string> _danlSubreddits = new List<string>()
        {
                            "TheRedPill",
                            "seduction",
                            "CuckoldCommunity",
                            "BikePorn",
                            "electronic_cigarette",
                            "snooker",
                            "UFC",
                            "Kratom",
                            "bird",
                            "government",
                            "joerogan"

        };

        [Command("danl", Help = "Impersonate danl.")]
        public async Task<string> Daniel(IInvocationContext context, IMessage message)
        {
            bool format = message.Text.GetCommandArguments().Count == 0;

            return (format ? "<danl>" : "") + await GetRandomTitleFromSubreddit(_danlSubreddits.GetRandom());
        }

        #endregion Danl

        #region Drug
        private List<string> _drugSubreddits = new List<string>()
        {
            "drugs",
            "drugscirclejerk",
            "opiates",
            "kratom",
            "researchchemicals",
            "trees",
            "stims",
            "psychonaut",
            "benzodiazepines",
            "tripsit",
            "nootropics",
            "drugsmart",
            "lsd"
        };

        [Command("drug", Help = "Get a random post title from the drugs subreddit.")]
        public async Task<string> Drug(IInvocationContext context, IMessage message)
        {
            return await GetRandomTitleFromSubreddit(_drugSubreddits.GetRandom());
        }

        [Command("benzo", Help = "Get a random post title from the benzodiazepines subreddit.")]
        public async Task<string> Benzo(IInvocationContext context, IMessage message)
        {
            return await GetRandomTitleFromSubreddit("benzodiazepines");
        }

        #endregion

        [Command("reddit", Help = ".reddit <subreddit> - Get a random post title from a subreddit.")]
        public async Task<string> RandomReddit(IInvocationContext context, IMessage message)
        {
            return await GetRandomTitleFromSubreddit(message.Text.GetCommandArguments().First());
        }

        private async Task<string> GetRandomTitleFromSubreddit(string subreddit)
        {
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync($"http://reddit.com/r/{subreddit}/random.json");
                var token = JToken.Parse(json);
                if (token.Type == JTokenType.Array)
                {
                    return token[0]["data"]["children"][0]["data"]["title"].ToString();
                }
                // Doesn't support random. Boo.
                var posts = token["data"]["children"].ToObject<List<dynamic>>();
                return posts.GetRandom()["data"]["title"];
            }
        }
    }


}
