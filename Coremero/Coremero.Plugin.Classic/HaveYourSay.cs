using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Coremero.Commands;
using Coremero.Utilities;
using Newtonsoft.Json;

namespace Coremero.Plugin.Classic
{
    public class HaveYourSay : IPlugin
    {
        private Dictionary<string, List<string>> _hysData;
        private Random _rnd = new Random();

        public HaveYourSay()
        {
            _hysData =
                JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(
                    File.ReadAllText(Path.Combine(PathExtensions.ResourceDir, "hys.json")));
        }

        private string GenerateRandomIdiotPunctuation()
        {
            switch (_rnd.Next(5))
            {
                case 0:
                    return ",, ";

                case 1:
                case 2:
                    return ". ";

                case 3:
                case 4:
                    return ", ";

                default:
                    return "... ";
            }
        }

        private string GenerateFakeOutrageComment(string subject)
        {
            StringBuilder builder = new StringBuilder();

            string opening = _rnd.Next(0, 1) == 1 ? "opening1" : "opening2";

            if (String.IsNullOrEmpty(subject))
            {
                builder.Append(
                    $"{_hysData[opening].GetRandom()} {_hysData["hated_object"].GetRandom()} {_hysData["terrible_thing"].GetRandom()}");
            }
            else
            {
                string plural = subject.EndsWith("s", StringComparison.OrdinalIgnoreCase) ? "are" : "is";
                builder.Append(
                    $"{_hysData[opening].GetRandom()} {subject} {plural} {_hysData["terrible_thing"].GetRandom()}");
            }

            if (_rnd.NextDouble() > 0.5)
            {
                builder.Append($" because {_hysData["because"].GetRandom()}");
            }

            builder.Append(
                $"{GenerateRandomIdiotPunctuation()}{_hysData["imperative"].GetRandom().Replace("[number]", _rnd.Next(0, 14).ToString())} {_hysData["moronic_solution"].GetRandom()}{GenerateRandomIdiotPunctuation()}");

            if (_rnd.NextDouble() > 0.4)
            {
                builder.Append($"{_hysData["signoff"].GetRandom()}{_hysData["end_punctuation"].GetRandom()}");
            }
            else
            {
                builder.Append($"{_hysData["opening1"].GetRandom()}{GenerateRandomIdiotPunctuation()}");
            }

            if (_rnd.NextDouble() > 0.3)
            {
                return builder.ToString().ToUpper();
            }

            return builder.ToString();
        }

        [Command("hys", Arguments = "Subject",
            Help = "Generates a fake Have Your Say style comment targeting [Subject].")]
        public string OutrageCommand(string subject)
        {
            return GenerateFakeOutrageComment(subject);
        }
    }
}