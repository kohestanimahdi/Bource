using Bource.Models.Data.Tsetmc;
using System.Collections.Generic;

namespace Bource.Models.Data.Common
{
    public class SymbolGroup : MongoDataEntity
    {
        public string Title { get; set; }
        public string Code { get; set; }

        public List<IndicatorSymbol> Symbols { get; set; }
    }
}