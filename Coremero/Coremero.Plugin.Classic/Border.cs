using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coremero.Client;
using Coremero.Commands;
using Coremero.Context;
using Coremero.Utilities;

namespace Coremero.Plugin.Classic
{
    public class Border : IPlugin
    {
        private const int BORDER_MAX_WIDTH = 40;

        #region Border code from Homeronet

        private List<string> WrapText(string text, int maxWidth)
        {
            var wrappedText = new List<string>();
            var words = text.Split(' ').ToList();
            var currentLine = new StringBuilder();

            foreach (var word in words)
            {
                var mutableWord = word;

                // if we will be too wide, add the line to the output list and start a new one
                if (currentLine.Length + word.Length + 1 > maxWidth)
                {
                    if (currentLine.Length > 0)
                    {
                        wrappedText.Add(currentLine.ToString());
                        currentLine.Clear();
                    }

                    while (mutableWord.Length > maxWidth)
                    {
                        // uh oh this word is huge, we need to break it apart
                        wrappedText.Add(mutableWord.Substring(0, maxWidth - 1));
                        mutableWord = mutableWord.Substring(maxWidth - 1);
                    }
                }

                currentLine.Append(" " + mutableWord);
            }

            if (currentLine.Length > 0)
            {
                wrappedText.Add(currentLine.ToString());
            }

            return wrappedText;
        }

        private List<string> FormatTextToHeadstone(List<string> lines)
        {
            var max = lines.Max(x => x.Length);
            var maxLine = max + 6;

            var outputLines = new List<string>();

            var headerBorder = new BorderedLine
            {
                Left = "  _.",
                Right = "._ ",
                Fill = '-'
            };
            var standardBorder = new BorderedLine
            {
                Left = " | ",
                Right = " |",
                Fill = ' '
            };
            var footerBorder = new BorderedLine
            {
                Left = " |",
                Right = "|",
                Fill = '_'
            };
            var baseBorder = new BorderedLine
            {
                Left = "|",
                Right = "|",
                Fill = '_'
            };

            outputLines.Add(headerBorder.FillToWidth(maxLine - 1)); // - 1 cause we want it to end 1 char early
            outputLines.Add(standardBorder.SurroundToWidth("RIP", max));

            outputLines.AddRange(lines.Select(line => standardBorder.SurroundToWidth(line.ToUpper(), max)));

            outputLines.Add(footerBorder.FillToWidth(maxLine - 1));
            outputLines.Add(baseBorder.FillToWidth(maxLine));

            return outputLines;
        }

        private List<string> FormatTextToBread(List<string> lines)
        {
            var max = lines.Max(x => x.Length);
            var maxLine = max + 5;

            var outputLines = new List<string>();

            var headerBorder = new BorderedLine
            {
                Left = " .",
                Right = ". ",
                Fill = '-'
            };
            var standardBorder = new BorderedLine
            {
                Left = "| ",
                Right = " |",
                Fill = ' '
            };
            var footerBorder = new BorderedLine
            {
                Left = "|",
                Right = "|",
                Fill = '_'
            };

            outputLines.Add(headerBorder.FillToWidth(maxLine - 1));
            outputLines.AddRange(lines.Select(line => standardBorder.SurroundToWidth(line.ToUpper(), max)));
            outputLines.Add(footerBorder.FillToWidth(maxLine - 1));

            return outputLines;
        }

        #region utility class

        private class BorderedLine
        {
            public string Left { get; set; }

            public string Right { get; set; }

            public char Fill { get; set; }

            public string FillToWidth(int width)
            {
                var toFill = width - Left.Length - Right.Length;
                return Left + new string(Fill, toFill) + Right;
            }

            public string SurroundToWidth(string text, int width)
            {
                var toFill = width - Left.Length - Right.Length - text.Length;
                return Left + PadToWidth(text, width) + Right;
            }

            private string PadToWidth(string text, int width)
            {
                var gapToFill = width - text.Length;
                return text.PadLeft(gapToFill / 2 + text.Length).PadRight(width);
            }
        }

        #endregion utility class

        #endregion Border code from Homeronet

        [Command("rip", Arguments = "WHO DIE", Help = "Creates a gravestone for [WHO DIE].")]
        public string RestInPeace(IInvocationContext context, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                text = context.Channel?.Users.GetRandom().Name;
            }
            // This is insane and clearly for the old bot.
            // TODO: API cleanup.
            string output = string.Join("\n", FormatTextToHeadstone(text.Split('\n').ToList()));

            if (context.OriginClient.Features.HasFlag(ClientFeature.Markdown))
            {
                output = $"```{output}```";
            }
            return output;
        }

        [Command("bread", Arguments = "Carb Eater", Help = "Creates a bread for [Carb Eater].")]
        public string Bread(IInvocationContext context, string text)
        {
            // TODO: API cleanup.
            string output = string.Join("\n", FormatTextToBread(text.Split('\n').ToList()));

            if (context.OriginClient.Features.HasFlag(ClientFeature.Markdown))
            {
                output = $"```{output}```";
            }
            return output;
        }
    }
}