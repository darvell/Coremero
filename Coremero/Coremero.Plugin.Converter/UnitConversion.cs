using System;
using System.Collections.Generic;
using System.Text;
using Coremero.Commands;
using Coremero.Utilities;

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
    }
}