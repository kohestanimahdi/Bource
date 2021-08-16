using Bource.Models.Data.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Bource.Models.Data.Tsetmc
{
    public class ClosingPriceInfo : MongoDataEntity
    {
        public ClosingPriceInfo()
        {
        }

        public ClosingPriceInfo(string row, ClosingPriceTypes types)
        {
            string[] array9 = row.Split(',');
            InsCode = Convert.ToInt64(array9[0].ToString());
            DEven = Convert.ToInt32(array9[1].ToString());
            PClosing = Convert.ToDecimal(array9[2].ToString());
            PDrCotVal = Convert.ToDecimal(array9[3].ToString());
            ZTotTran = Convert.ToDecimal(array9[4].ToString());
            QTotTran5J = Convert.ToDecimal(array9[5].ToString());
            QTotCap = Convert.ToDecimal(array9[6].ToString());
            PriceMin = Convert.ToDecimal(array9[7].ToString());
            PriceMax = Convert.ToDecimal(array9[8].ToString());
            PriceYesterday = Convert.ToDecimal(array9[9].ToString());
            PriceFirst = Convert.ToDecimal(array9[10].ToString());
            Type = types;
        }

        public ClosingPriceInfo(SymbolData data, ClosingPriceTypes type)
        {
            InsCode = data.InsCode;
            DEven = Convert.ToInt32(data.LastUpdate.ToString("yyyyMMdd"));
            PClosing = data.FinishPrice;
            PDrCotVal = data.LastPrice;
            ZTotTran = data.NumberOfTransaction;
            QTotTran5J = data.Turnover;
            QTotCap = data.ValueOfTransaction;
            PriceMin = data.MinPrice;
            PriceMax = data.MaxPrice;
            PriceYesterday = data.YesterdayPrice;
            PriceFirst = data.FirstPrice;
            Type = type;
        }

        [BsonIgnore]
        public DateTime Date
        {
            get
            {
                return DateTime.ParseExact(DEven.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
            }
        }

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
        public int DEven { get; set; }

        [Display(Name = "قیمت پایانی")]
        public decimal PClosing { get; set; }

        [Display(Name = "قیمت آخرین معامله")]
        public decimal PDrCotVal { get; set; }

        [Display(Name = "تعداد معاملات")]
        public decimal ZTotTran { get; set; }

        [Display(Name = "حجم معاملات")]
        public decimal QTotTran5J { get; set; }

        [Display(Name = "ارزش معاملات")]
        public decimal QTotCap { get; set; }

        [Display(Name = "کمترین قیمت")]
        public decimal PriceMin { get; set; }

        [Display(Name = "بیشترین قیمت")]
        public decimal PriceMax { get; set; }

        [Display(Name = "قیمت دیروز")]
        public decimal PriceYesterday { get; set; }

        [Display(Name = "اولین قیمت")]
        public decimal PriceFirst { get; set; }


        public ClosingPriceTypes Type { get; set; }

        //public ClosingPriceInfo ConvertToType(ClosingPriceTypes types)
        //{
        //    return types switch
        //    {
        //        ClosingPriceTypes.NoPriceAdjustment => this,
        //        ClosingPriceTypes.CapitalIncreaseAndProfit => ConvertToCapitalIncreaseAndProfitType(),
        //        ClosingPriceTypes.CapitalIncrease => ConvertToCapitalIncreaseType(),
        //        _ => null
        //    };
        //}

        //private ClosingPriceInfo ConvertToCapitalIncreaseAndProfitType()
        //{
        //}

        //private ClosingPriceInfo ConvertToCapitalIncreaseType()
        //{
        //}
    }
}