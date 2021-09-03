using Newtonsoft.Json;

namespace Bource.Services.Crawlers.Shakhesban.Models
{
    internal class SymbolPriorityResponse
    {
        [JsonProperty("tbody")]
        public string Body { get; set; }
    }
}
