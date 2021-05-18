using System;

namespace Bource.Services.Crawlers.Tsetmc.Models
{
    public class TseShareInfo
    {
        public TseShareInfo()
        {

        }
        public TseShareInfo(string row)
        {
            string[] items = row.Split(',');
            Idn = Convert.ToInt64(items[0].ToString());
            InsCode = Convert.ToInt64(items[1].ToString());
            DEven = Convert.ToInt32(items[2].ToString());
            NumberOfShareNew = Convert.ToDecimal(items[3].ToString());
            NumberOfShareOld = Convert.ToDecimal(items[4].ToString());
        }
        public long Idn { get; set; }
        public long InsCode { get; set; }
        public int DEven { get; set; }
        public decimal NumberOfShareNew { get; set; }
        public decimal NumberOfShareOld { get; set; }
    }
}
