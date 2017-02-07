namespace Coremero.Plugin.Classic.TumblrJson
{
    public class Response
    {
        public Blog blog { get; set; }
        public Post[] posts { get; set; }
        public int total_posts { get; set; }
    }
}