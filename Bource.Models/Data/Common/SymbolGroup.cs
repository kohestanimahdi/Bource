﻿using System.Collections.Generic;

namespace Bource.Models.Data.Common
{
    public class SymbolGroup : DataEntity
    {
        public string Title { get; set; }
        public int Code { get; set; }

        public List<SymbolSubGroup> SubGroups { get; set; }
    }


}
