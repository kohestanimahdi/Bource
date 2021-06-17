using Bource.Models.Data.Common;

namespace Bource.Services.Crawlers.Codal360.Models
{
    internal class CodalSearchSymbolResponse
    {
        [Newtonsoft.Json.JsonProperty("id")]
        public int Id { get; set; }

        [Newtonsoft.Json.JsonProperty("title")]
        public string Title { get; set; }

        [Newtonsoft.Json.JsonProperty("symbol")]
        public string Symbol { get; set; }

        [Newtonsoft.Json.JsonProperty("code")]
        public string Code { get; set; }

        [Newtonsoft.Json.JsonProperty("classification")]
        public string Classification { get; set; }

        [Newtonsoft.Json.JsonProperty("type")]
        public string Type { get; set; }

        [Newtonsoft.Json.JsonProperty("company_id")]
        public string CompanyId { get; set; }

        public void UpdateSymbol(Symbol symbol, string baseUrl)
        {
            symbol.CodalClassification = Classification;
            symbol.CodalTitle = Title;
            symbol.CodalType = Type;
            symbol.CodalCode = Code;
            symbol.CodalCompanyId = CompanyId;
            symbol.CodalURL = $"{baseUrl}fa/publisher/{CompanyId}/";
        }
    }
}