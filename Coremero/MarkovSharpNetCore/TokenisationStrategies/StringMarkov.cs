using System;
using System.Collections.Generic;
using System.Text;

namespace MarkovSharpNetCore.TokenisationStrategies
{
    public class StringMarkov : GenericMarkov<string, string>
    {
        public StringMarkov(int level = 2)
            : base(level)
        {
        }

        public override IEnumerable<string> SplitTokens(string input)
        {
            if (input == null)
            {
                return new List<string>() {GetPrepadGram()};
            }

            return input?.Split(' ');
        }

        public override string RebuildPhrase(IEnumerable<string> tokens)
        {
            return string.Join(" ", tokens);
        }

        public override string GetTerminatorGram()
        {
            return null;
        }

        public override string GetPrepadGram()
        {
            return "";
        }
    }
}