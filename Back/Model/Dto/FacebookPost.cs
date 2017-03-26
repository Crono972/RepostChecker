using Newtonsoft.Json;

namespace RepostChecker.Model
{
    public class FacebookPost
    {
        [JsonProperty("id")]
        public string Id;
        [JsonProperty("from")]
        public User Author;
        [JsonProperty("message")]
        public string Title;
        [JsonProperty("link")]
        public string Link;
        [JsonProperty("picture")]
        public string PictureUrl;
        [JsonProperty("source")]
        public string Source;
    }
}
