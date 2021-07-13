using Bource.Models.Data.Common;
using Bource.Models.Data.Enums;
using System.Collections.Generic;

namespace Bource.Models.Data.Tsetmc
{
    public class Papers : MongoDataEntity
    {
        public Papers()
        {
            Symbols = new List<Symbol>();
        }
        public string Title { get; set; }
        public PapersTypes Type { get; set; }
        public List<Symbol> Symbols { get; set; }
    }
}
