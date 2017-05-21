using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Coremero.Commands;
using Coremero.Context;
using Coremero.Messages;
using Coremero.Utilities;
using Newtonsoft.Json;

namespace Coremero.Plugin.Converter
{
    public class UnitConversion : IPlugin
    {
        [Command("beats", Help = "Return the current time in .beats.")]
        public string BeatTime(IInvocationContext context, IMessage message)
        {
            var now = DateTime.UtcNow + TimeSpan.FromHours(1);
            var beatsTime = Math.Floor((now.Second + (now.Minute * 60) + (now.Hour * 3600)) / 86.4f);
            return $"UTC: {now:H:mm:ss} | Beats: @{beatsTime}";
        }

        [Command("temperature", Help = ".temperature <temp> - Convert <temp> from Celsius to Fahrenheit and vice versa."
        )]
        public string Temperature(IInvocationContext context, IMessage message)
        {
            List<String> args = message.Text.GetCommandArguments();
            double temperature = double.Parse(args[0]);

            var c = (temperature - 32) * (5.0 / 9.0);
            var f = (temperature * (9.0 / 5.0)) + 32;

            return $"{temperature:0.0}F is {c:0.0}C. {temperature:0.0}C is {f:0.0}F";
        }

        [Command("convert", Help = "Converts [Amount] [Currency From] to [Current To].")]
        public async Task<string> CurrencyConvert(string message)
        {
            double amount = 1.0;
            var args = message.GetCommandArguments();
            amount = Convert.ToDouble(args[0]);
            using (HttpClient client = new HttpClient())
            {
                string payload = await client.GetStringAsync($"http://api.fixer.io/latest?base={args[1]}&symbols={args[2]}");
                var json = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(payload);
                double converted = amount * Convert.ToDouble(json["rates"][args[2]].ToString());
                return $"{args[0]} {args[1]} is {converted:#.00##} {args[2]}.";
            }
        }
    }
}