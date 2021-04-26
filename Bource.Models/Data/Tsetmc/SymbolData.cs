using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
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


        [Display(Name = "تعداد معاملات")]
        public long NumberOfTransaction { get; set; }

        [Display(Name = "حجم معاملات")]
        public long Turnover { get; set; }

        [Display(Name = "ارزش معاملات")]
        public long ValueOfTransaction { get; set; }

        [Display(Name = "ارزش بازار")]
        public long ValueOfMarket { get; set; }

        [Display(Name = "قیمت دیروز")]
        public long YesterdayPrice { get; set; }

        [Display(Name = "خرید")]
        public long? BuyPrice { get; set; }

        [Display(Name = "فروش")]
        public long? SalePrice { get; set; }

        [Display(Name = "اولین قیمت")] //pf
        public long FirstPrice { get; set; }

        [Display(Name = "کمترین بازه روز")] //pmin
        public long MinPrice { get; set; }

        [Display(Name = "بیشترین بازه روز")] //pmax
        public long MaxPrice { get; set; }

        [Display(Name = "قیمت معامله")] //pl
        public long LastPrice { get; set; }

        [Display(Name = "تغییر قیمت معامله")] //plc
        public long LastPriceChange { get; set; }

        [Display(Name = "درصد تغییر قیمت معامله")] //plp
        public double PercentLastPriceChange { get; set; }

        [Display(Name = "قیمت پایانی")] //pc
        public long FinishPrice { get; set; }

        [Display(Name = "تغییر قیمت پایانی")] //pcc
        public long FinishPriceChange { get; set; }

        [Display(Name = "درصد تغییر قیمت پایانی")]//pcl
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

        [Display(Name = "تعداد خریدار حقیقی")]
        public long NaturalBuyNumber { get; set; }

        [Display(Name = "تعداد خریدار حقوقی")]
        public long LegalEntityBuyNumber { get; set; }

        [Display(Name = "حجم خرید حقیقی")]
        public long NaturalBuyValue { get; set; }

        [Display(Name = "حجم خرید حقوقی")]
        public long LegalEntityBuyValue { get; set; }

        [Display(Name = "تعداد فروشنده حقیقی")]
        public long NaturalSellNumber { get; set; }

        [Display(Name = "تعداد فروشنده حقوقی")]
        public long LegalEntitySellNumber { get; set; }

        [Display(Name = "حجم فروش حقیقی")]
        public long NaturalSellValue { get; set; }

        [Display(Name = "حجم فروش حقوقی")]
        public long LegalEntitySellValue { get; set; }

        public List<SymbolTransaction> BuyTransactions { get; set; }
        public List<SymbolTransaction> SellTransactions { get; set; }

    }

    public class SymbolTransaction
    {
        public short Order { get; set; }
        public long Number { get; set; }
        public long Value { get; set; }
        public long Price { get; set; }

    }
}
