using Bource.Common.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Bource.Services.Crawlers.Tsetmc.Models
{
    public class FillSymbolData
    {
        public FillSymbolData(long insCode)
        {
            InsCode = insCode;
        }

        public void FillDataFromPage(string html)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"QTotTran5JAvg\=\'([0-9]*|([0-9]*.[0-9]*))\'");
            if (regex.IsMatch(html))
            {
                var result = regex.Match(html);
                MonthAverageValue = result.Value.RegexConvertToDecimal();
            }

            regex = new System.Text.RegularExpressions.Regex(@"KAjCapValCpsIdx\=\'([0-9]*|([0-9]*.[0-9]*))\'");
            if (regex.IsMatch(html))
            {
                var result = regex.Match(html);
                FloatingStock = result.Value.RegexConvertToDecimal();
            }

            regex = new System.Text.RegularExpressions.Regex(@"SectorPE\=\'([0-9]*|([0-9]*.[0-9]*))\'");
            if (regex.IsMatch(html))
            {
                var result = regex.Match(html);
                GroupPE = result.Value.RegexConvertToDecimal();
            }
        }
        public long InsCode { get; set; }


        [Display(Name = "سهام شناور")]
        public decimal? FloatingStock { get; set; }

        [Display(Name = "میانگین حجم ماه")]
        public decimal? MonthAverageValue { get; set; }

        [Display(Name = "PE گروه")]
        public decimal? GroupPE { get; set; }
    }
}
