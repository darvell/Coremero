using Coremero.Commands;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Coremero.Utilities;
using Newtonsoft.Json.Linq;

namespace Coremero.Plugin.Playground
{
    public class NorrisFact : IPlugin
    {
        [Command("fact")]
        public async Task<string> RandomFact(IInvocationContext context, string target)
        {
            if (String.IsNullOrEmpty(target))
            {
                target = context.Channel.Users.GetRandom().Name;
            }
            using (HttpClient client = new HttpClient())
            {
                JObject payload = JObject.Parse(await client.GetStringAsync("https://api.chucknorris.io/jokes/random"));
                return payload["value"].ToString().ReplaceString("Chuck Norris", target, StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
