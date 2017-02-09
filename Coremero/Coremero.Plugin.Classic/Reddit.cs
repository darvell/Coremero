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
        [Command("alligator")]
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

        [Command("hurt")]
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
        [Command("hyle")]
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

        [Command("danl")]
        public async Task<string> Daniel(IInvocationContext context, IMessage message)
        {
            bool format = message.Text.GetCommandArguments().Count == 0;

            return (format ? "<danl>" : "") + await GetRandomTitleFromSubreddit(_danlSubreddits.GetRandom());
        }

        #endregion Danl

        private async Task<string> GetRandomTitleFromSubreddit(string subreddit)
        {
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync($"http://reddit.com/r/{subreddit}.json");
                var posts = JObject.Parse(json)["data"]["children"].ToObject<List<dynamic>>();
                return posts.GetRandom()["data"]["title"];
            }
        }
    }


}
