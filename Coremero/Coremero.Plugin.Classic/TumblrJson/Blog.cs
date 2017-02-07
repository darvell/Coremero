namespace Coremero.Plugin.Classic.TumblrJson
{
    public class Blog
    {
        public string title { get; set; }
        public string name { get; set; }
        public int total_posts { get; set; }
        public int posts { get; set; }
        public string url { get; set; }
        public int updated { get; set; }
        public string description { get; set; }
        public bool is_nsfw { get; set; }
        public bool ask { get; set; }
        public string ask_page_title { get; set; }
        public bool ask_anon { get; set; }
        public bool share_likes { get; set; }
    }
}