using System;
using System.Collections.Generic;
using System.Text;
using Coremero.Commands;
using Coremero.Utilities;

namespace Coremero.Plugin.Playground
{
    public class Mystical : IPlugin
    {
        private List<string> _eightBallAnswers = new List<string>()
        {
            "It is certain.",
            "It is decidedly so.",
            "Without a doubt.",
            "Yes definitely.",
            "You may rely on it.",
            "As I see it, yes.",
            "Most likely.",
            "Outlook good.",
            "Yes.",
            "Signs point to yes.",
            "Reply hazy try again.",
            "Ask again later.",
            "Better not tell you now.",
            "Cannot predict now.",
            "Concentrate and ask again.",
            "Don't count on it.",
            "My reply is no.",
            "My sources say no.",
            "Outlook not so good.",
            "Very doubtful."
        };

        private Random _random = new Random();

        [Command("8ball", Arguments = "Question")]
        public string MagicEightBall(string question)
        {
            return $"You asked \"{question}\" and the answer is...\n{_eightBallAnswers.GetRandom()} ";
        }

        [Command("coinflip")]
        public string CoinFlip()
        {
            return $"The coin has been flipped and the answer is {(_random.NextDouble() >= 0.5 ? "Heads" : "Tails")}!";
        }

        [Command("askurbit")]
        public string AskUrbit(string question)
        {
            bool yes = _random.NextDouble() <= 0.5;
            return
                $"You asked if {question} and the ~ magic urbit ~ has said that is {(yes ? "true" : "false")}. Which in the real world means it is {(yes ? "false" : "true")}.";
        }
    }
}
