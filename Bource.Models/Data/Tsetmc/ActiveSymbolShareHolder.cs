using System;
using System.Collections.Generic;

namespace Bource.Models.Data.Tsetmc
{
    public class ActiveSymbolShareHolder : MongoDataEntity
    {
        public long InsCode
        {
            get
            {
                return Convert.ToInt64(InsCodeValue);
            }
            set
            {
                InsCodeValue = value.ToString();
            }
        }
        public string InsCodeValue { get; set; }
        public string SymbolName { get; set; }
        public DateTime Time { get; set; }
        public List<ActiveSymbolShareHolderCompany> Companies { get; set; }
    }

    public class ActiveSymbolShareHolderCompany
    {
        public string Name { get; set; }
        public decimal Share { get; set; }
        public decimal ShareChange { get; set; }
    }
}