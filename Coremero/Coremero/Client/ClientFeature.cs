using System;

namespace Coremero.Client
{
    [Flags]
    public enum ClientFeature
    {
        None = 0, // DO NOT USE
        Text = 1 << 0,
        Markdown = 2 << 1,
        AudioChat = 3 << 2 ,
        MediaAttachments = 4 << 3,
        UrlInlining = 5 << 4,
        ColorControlCodes = 6 << 5,
        All = ~(-1 << 6) // FOR MOCKING ONLY
    }
}