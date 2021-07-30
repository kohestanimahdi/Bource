using Bource.Common.Utilities;
using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using Bource.WebConfiguration.Api;
using Newtonsoft.Json;

namespace Bource.Portal.ViewModels.Dtos.SymbolDtos
{
    public class SymbolListResponse : ResponseBaseDto<SymbolListResponse, Symbol, string>
    {
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Sign { get; set; }
        public string CompanyName { get; set; }
        public long InsCode { get; set; }
        public PapersTypes? PaperType { get; set; }
        public string PaperTypeName
        {
            get
            {
                if (PaperType.HasValue)
                    return PaperType.Value.ToDisplay();

                return string.Empty;
            }
        }

        [JsonIgnore]
        public override string Id { get; set; }
    }
}
