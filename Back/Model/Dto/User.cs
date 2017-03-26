using Newtonsoft.Json;

namespace RepostChecker.Model
{
    public class User
    {
        [JsonProperty("Id")]
        public string Id;
        [JsonProperty("name")]
        public string Name;
    }
}
