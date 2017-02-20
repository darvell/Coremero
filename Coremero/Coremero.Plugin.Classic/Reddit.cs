using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Utilities;
using Newtonsoft.Json.Linq;
using MarkovSharpNetCore.TokenisationStrategies;

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
        public async Task<string> Daniel()
        {
            return "<danl>" + await GetRandomTitleFromSubreddit(_danlSubreddits.GetRandom());
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
        public async Task<string> Drug()
        {
            return await RandomReddit(string.Join(" ", _drugSubreddits));
        }

        [Command("benzo", Help = "Get a random post title from the benzodiazepines subreddit.")]
        public async Task<string> Benzo(IInvocationContext context, IMessage message)
        {
            return await GetRandomTitleFromSubreddit("benzodiazepines");
        }

        #endregion

        [Command("reddit", Arguments = "Subreddit Name", Help = "Get a random post title from a subreddit.")]
        public async Task<string> RandomReddit(string subreddits)
        {
            return await GetRandomTitleFromSubreddit(string.Join("+", subreddits.GetCommandArguments()));
        }

        private async Task<string> GetRandomTitleFromSubreddit(string subreddit)
        {
            return (await GetTitlesFromSubreddit(subreddit)).GetRandom();
        }

        private async Task<List<string>> GetTitlesFromSubreddit(string subreddit)
        {
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync($"https://reddit.com/r/{subreddit}.json");
                var token = JToken.Parse(json);
                var posts = token["data"]["children"].AsJEnumerable();
                return posts.Select(x => x["data"]["title"].ToString()).ToList();
            }
        }

        [Command("imireddit", Arguments = "Subreddit Name", Help = "Imitates a subreddit.")]
        public async Task<string> MarkovReddit(string subreddit)
        {
            int walkSize = 1;
            if (subreddit.Split(' ').Length > 5)
            {
                walkSize = 2;
            }
            var model = new StringMarkov(walkSize) {EnsureUniqueWalk = true};
            foreach (string sub in subreddit.Split(' '))
            {
                model.Learn(await GetTitlesFromSubreddit(sub));
            }
            return model.Walk(15).Skip(5).OrderByDescending(x => x.Length).Take(5).GetRandom();
        }
    }
}