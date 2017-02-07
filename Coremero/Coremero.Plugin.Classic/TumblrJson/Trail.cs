namespace Coremero.Plugin.Classic.TumblrJson
{
    public class Trail
    {
        public Blog1 blog { get; set; }
        public Post1 post { get; set; }
        public string content_raw { get; set; }
        public string content { get; set; }
        public bool is_root_item { get; set; }
        public bool is_current_item { get; set; }
    }
}