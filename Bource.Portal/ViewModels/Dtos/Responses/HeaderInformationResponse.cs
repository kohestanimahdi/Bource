using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Bource.Portal.ViewModels.Dtos.Responses
{
    public class HeaderInformationResponse
    {
        public DateTime Time { get; set; }

        [Display(Name = "شاخص کل")]
        public decimal OverallIndex { get; set; }

        [Display(Name = "تغییرات شاخص کل")]
        public decimal OverallIndexChange { get; set; }

        [Display(Name = "شاخص کل-هم وزن")]
        public decimal OverallIndexEqualWeight { get; set; }

        [Display(Name = "تغییرات شاخص کل-هم وزن")]
        public decimal OverallIndexEqualWeightChange { get; set; }
    }
}
