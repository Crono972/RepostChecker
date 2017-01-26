using Newtonsoft.Json;
using System.Collections.Generic;

namespace RepostChecker
{

    public class User
    {
        [JsonProperty("Id")]
        public string Id;
        [JsonProperty("name")]
        public string Name;
    }

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

    public class FacebookGroupFeed
    {
        [JsonProperty("data")]
        public List<FacebookPost> Posts;
        [JsonProperty("paging")]
        public Paging paging;
    }

    public class Paging
    {
        [JsonProperty("previous")]
        public string Previous;
        [JsonProperty("next")]
        public string Next;
    }
}
