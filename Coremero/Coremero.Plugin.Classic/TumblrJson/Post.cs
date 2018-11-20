namespace Coremero.Plugin.Classic.TumblrJson
{
    public class Post
    {
        public string type { get; set; }
        public string blog_name { get; set; }
        public long id { get; set; }
        public string post_url { get; set; }
        public string slug { get; set; }
        public string date { get; set; }
        public int timestamp { get; set; }
        public string state { get; set; }
        public string format { get; set; }
        public string reblog_key { get; set; }
        public string[] tags { get; set; }
        public string short_url { get; set; }
        public string summary { get; set; }
        public object recommended_source { get; set; }
        public object recommended_color { get; set; }
        public int note_count { get; set; }
        public string caption { get; set; }
        public Reblog reblog { get; set; }
        public Trail[] trail { get; set; }
        public string image_permalink { get; set; }
        public Photo[] photos { get; set; }
        public bool can_like { get; set; }
        public bool can_reblog { get; set; }
        public bool can_send_in_message { get; set; }
        public bool can_reply { get; set; }
        public bool display_avatar { get; set; }
        public string link_url { get; set; }
        public string source_url { get; set; }
        public string source_title { get; set; }
        public string photoset_layout { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string video_url { get; set; }
        public bool html5_capable { get; set; }
        public string thumbnail_url { get; set; }
        public int thumbnail_width { get; set; }
        public int thumbnail_height { get; set; }
        public int duration { get; set; }
        public Player[] player { get; set; }
        public string video_type { get; set; }
    }
}