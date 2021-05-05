using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bource.Models.Data.Tsetmc
{
    public class OTCCashMarketAtGlance : MongoDataEntity
    {
        [Display(Name = "وضعیت بازار")]
        public string Status { get; set; }

        [Display(Name = "شاخص کل")]
        public decimal OverallIndex { get; set; }

        [Display(Name = "تغییرات شاخص کل")]
        public decimal OverallIndexChange { get; set; }

        [Display(Name = "ارزش بازار اول و دوم")]
        public decimal ValueOfFirstAndSecondMarket { get; set; }

        [Display(Name = "اطلاعات قیمت")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }

        [Display(Name = "تعداد معاملات")]
        public decimal NumberOfTransaction { get; set; }

        [Display(Name = "ارزش معاملات")]
        public decimal ValueOfTransaction { get; set; }

        [Display(Name = "حجم معاملات")]
        public decimal Turnover { get; set; }

    }
}
