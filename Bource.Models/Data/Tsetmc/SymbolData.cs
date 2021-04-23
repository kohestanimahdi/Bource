using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Bource.Models.Data.Tsetmc
{
    public class SymbolData : DataEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string SymbolId { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdate { get; set; }
        public string Status { get; set; }


        [Display(Name = "تعداد تراکنش")]
        public long NumberOfTransaction { get; set; }

        [Display(Name = "حجم معاملات")]
        public long Turnover { get; set; }

        [Display(Name = "ارزش معاملات")]
        public long ValueOfTransaction { get; set; }

        [Display(Name = "ارزش بازار")]
        public long ValueOfMarket { get; set; }

        [Display(Name = "قیمت دیروز")]
        public long YesterdayPrice { get; set; }

        [Display(Name = "اولین قیمت")]
        public long FirstPrice { get; set; }

        [Display(Name = "کمترین قیمت")]
        public long MinPrice { get; set; }

        [Display(Name = "بیشترین قیمت")]
        public long MaxPrice { get; set; }

        [Display(Name = "آخرین قیمت")]
        public long LastPrice { get; set; }

        [Display(Name = "تغییر آخرین قیمت")]
        public long LastPriceChange { get; set; }

        [Display(Name = "درصد تغییر آخرین قیمت")]
        public double PercentLastPriceChange { get; set; }

        [Display(Name = "قیمت پایانی")]
        public long FinishPrice { get; set; }

        [Display(Name = "تغییر قیمت پایانی")]
        public long FinishPriceChange { get; set; }

        [Display(Name = "درصد تغییر قیمت پایانی")]
        public double PercentFinishPriceChange { get; set; }

        [Display(Name = "EPS")]
        public long? EPS { get; set; }

        [Display(Name = "PE")]
        public double PE { get; set; }

        [Display(Name = "PE گروه")]
        public double GroupPE { get; set; }

        [Display(Name = "تعداد سهام")]
        public long Count { get; set; }

        [Display(Name = "حجم مبنا")]
        public long BaseValue { get; set; }

        [Display(Name = "کمترین قیمت مجاز")]
        public double MinAllowedPrice { get; set; }

        [Display(Name = "بیشترین قیمت مجاز")]
        public double MaxAllowedPrice { get; set; }

        [Display(Name = "سهام شناور")]
        public double? FloatingStock { get; set; }

        [Display(Name = "میانگین حجم ماه")]
        public double? MonthAverageValue { get; set; }
    }
}
