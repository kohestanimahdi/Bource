using System.ComponentModel.DataAnnotations;

namespace Bource.Models.Data.Enums
{
    public enum PapersTypes
    {
        [Display(Name = "بازار نقد")]
        Cash = 0,

        [Display(Name = "آتی")]
        Future = 1,

        [Display(Name = "تبعی")]
        Option = 2,

        [Display(Name = "بدهی")]
        Debt = 3,

        [Display(Name = "ETF")]
        ETF = 4,

        [Display(Name = "اختیار")]
        TradeOption = 5,

        [Display(Name = "فرابورس")]
        OTC = 6,

        [Display(Name = "تسهیلات مسکن")]
        HousingFacilities = 7
    }
}
