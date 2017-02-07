using System;
using System.Text;

namespace Coremero.Plugin.Converter
{
    public static class UnicodeStringExtensions
    {
        public static string ToUnicodeHandwrittenScript(this string input)
        {
            var builder = new StringBuilder();

            foreach (var c in input)
            {
                if (c > 64 && c < 91)
                {
                    builder.Append(char.ConvertFromUtf32(c + 120107));
                }
                else if (c > 96 && c < 123)
                {
                    builder.Append(char.ConvertFromUtf32(c + 119841));
                }
                else
                {
                    builder.Append(char.ConvertFromUtf32(c));
                }
            }

            return builder.ToString();
        }

        public static string ToUnicodeFullWidth(this string input)
        {
            var builder = new StringBuilder();

            foreach (var c in input)
            {
                if (c == 32)
                {
                    builder.Append('\u3000');
                }
                // pass special chars through
                else if (c <= 31)
                {
                    builder.Append(char.ConvertFromUtf32(c));
                }
                else if (c > 176)
                {
                    builder.Append(c);
                }
                else
                {
                    builder.Append(char.ConvertFromUtf32(c + 65248));
                }
            }

            return builder.ToString();
        }
    }
}
