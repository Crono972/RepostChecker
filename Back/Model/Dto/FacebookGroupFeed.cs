using Newtonsoft.Json;
using System.Collections.Generic;

namespace RepostChecker.Model
{
    public class FacebookGroupFeed
    {
        [JsonProperty("data")]
        public List<FacebookPost> Posts;
        [JsonProperty("paging")]
        public Paging paging;
    }
}
