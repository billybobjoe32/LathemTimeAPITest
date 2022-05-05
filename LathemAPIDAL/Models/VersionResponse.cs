using Newtonsoft.Json;

namespace LathemAPIDAL.Models
{
    public class VersionResponse
    {
        [JsonProperty("Version")]
        public string Version { get; set; }
    }
}
