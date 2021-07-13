using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bource.Services.Crawlers.Tse.Models
{
    internal class GetPapersResponse
    {
        [JsonProperty("companies")]
        public List<GetPapersCompaniesResponse> Companies { get; set; }
    }

    internal class GetPapersCompanyResponse
    {
        [JsonProperty("ic")]
        public string CompanyCode { get; set; }

        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("s")]
        public string Status { get; set; }

        [JsonProperty("sy")]
        public string Sign { get; set; }
    }

    internal class GetPapersCompaniesResponse
    {
        [JsonProperty("list")]
        public List<GetPapersCompanyResponse> Companies { get; set; }
    }
}
