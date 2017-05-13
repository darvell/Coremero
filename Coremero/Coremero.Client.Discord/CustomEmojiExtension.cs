using System;
using System.Collections.Generic;
using System.Text;
using Discord;

namespace Coremero.Client.Discord
{
    public static class CustomEmojiExtension
    {
        public static string GetApiName(this Emoji emoji)
        {
            return emoji.Name;
        }
    }
}
