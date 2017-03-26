using Newtonsoft.Json;

namespace RepostChecker.Model
{

    public class Paging
    {
        [JsonProperty("previous")]
        public string Previous;
        [JsonProperty("next")]
        public string Next;
    }
}
