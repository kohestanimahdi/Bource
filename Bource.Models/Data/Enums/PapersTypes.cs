using System.ComponentModel.DataAnnotations;

namespace Bource.Models.Data.Enums
{
    public enum PapersTypes
    {
        [Display(Name = "بازار نقد")]
        Cash = 1,

        [Display(Name = "آتی")]
        Future = 2,

        [Display(Name = "تبعی")]
        Option = 3,

        [Display(Name = "بدهی")]
        Debt = 4,

        [Display(Name = "ETF")]
        ETF = 5,

        [Display(Name = "اختیار")]
        TradeOption = 7,

        [Display(Name = "فرابورس")]
        OTC = 8,

        [Display(Name = "تسهیلات مسکن")]
        HousingFacilities = 9,

        [Display(Name = "بورس کالا")]
        CommodityExchange = 20
    }
}