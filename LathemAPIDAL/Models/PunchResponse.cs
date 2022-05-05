using Newtonsoft.Json;

namespace LathemAPIDAL.Models
{
    public class PunchResponse
    {
        [JsonProperty("IsPunchedIn")]
        public bool IsPunchedIn { get; set; }
    }
}
