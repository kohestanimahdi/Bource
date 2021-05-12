using Bource.Common.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Bource.Models.Data.Tsetmc
{
    public class SymbolData : MongoDataEntity
    {
        public SymbolData()
        {
        }

        public SymbolData(string dataLine, DateTime lastModified)
        {
            LastUpdate = lastModified;

            var data = dataLine.Split(",");

            IId = data[0];
            InsCode = data[1];
            Symbol = data[2].FixPersianLetters();
            Name = data[3].FixPersianLetters();
            FirstPrice = Convert.ToInt64(data[5]);
            FinishPrice = Convert.ToInt64(data[6]);
            LastPrice = Convert.ToInt64(data[7]);
            NumberOfTransaction = Convert.ToInt64(data[8]);
            Turnover = Convert.ToInt64(data[9]);
            ValueOfTransaction = Convert.ToInt64(data[10]);
            MinPrice = Convert.ToInt64(data[11]);
            MaxPrice = Convert.ToInt64(data[12]);
            YesterdayPrice = Convert.ToInt64(data[13]);
            EPS = string.IsNullOrWhiteSpace(data[14]) ? null : Convert.ToInt64(data[14]);
            BaseValue = Convert.ToInt64(data[15]);
            SymbolGroup = data[18];
            MaxAllowedPrice = Convert.ToDouble(data[19]);
            MinAllowedPrice = Convert.ToDouble(data[20]);
            Count = Convert.ToInt64(data[21]);
            FinishPriceChange = FinishPrice - YesterdayPrice;
            PercentFinishPriceChange = Math.Round(100d * FinishPriceChange / YesterdayPrice, 2);
            LastPriceChange = NumberOfTransaction == 0 ? 0 : LastPrice - YesterdayPrice;
            PercentLastPriceChange = NumberOfTransaction == 0 ? 0 : Math.Round(100d * LastPriceChange / YesterdayPrice, 2);
            PE = EPS.HasValue ? Math.Round(100d * FinishPrice / EPS.Value, 2) : null;

            BuyTransactions = new List<SymbolTransaction>();
            SellTransactions = new List<SymbolTransaction>();
        }

        //[BsonIgnore]
        public string InsCode { get; set; }

        //[BsonIgnore]
        public string IId { get; set; }

        //[BsonIgnore]
        public string Name { get; set; }

        //[BsonIgnore]
        public string Symbol { get; set; }

        //[BsonIgnore]
        public string SymbolGroup { get; set; }

        //[BsonRepresentation(BsonType.ObjectId)]
        //public string SymbolId { get; set; }

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
        public double? PE { get; set; }

        [Display(Name = "PE گروه")]
        public double? GroupPE { get; set; }

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

        public override bool Equals(object obj)
        {
            if (obj is SymbolData symbolData)
            {
                if (symbolData.BuyTransactions.Count != BuyTransactions.Count)
                    return false;

                if (symbolData.SellTransactions.Count != SellTransactions.Count)
                    return false;

                if (symbolData.NumberOfTransaction != NumberOfTransaction)
                    return false;

                if (BuyTransactions.Any())
                    for (short i = BuyTransactions.First().Order; i <= BuyTransactions.Last().Order; i++)
                    {
                        var item = symbolData.BuyTransactions.FirstOrDefault(j => j.Order == i);
                        var secondItem = BuyTransactions.FirstOrDefault(j => j.Order == i);
                        if (item is null && secondItem is null)
                            continue;

                        if ((item is null && secondItem is not null) || (item is not null && secondItem is null))
                            return false;

                        if (!item.Equals(secondItem))
                            return false;
                    }

                if (SellTransactions.Any())
                    for (short i = SellTransactions.First().Order; i <= SellTransactions.Last().Order; i++)
                    {
                        var item = symbolData.SellTransactions.FirstOrDefault(j => j.Order == i);
                        var secondItem = SellTransactions.FirstOrDefault(j => j.Order == i);
                        if (item is null && secondItem is null)
                            continue;

                        if ((item is null && secondItem is not null) || (item is not null && secondItem is null))
                            return false;

                        if (!item.Equals(secondItem))
                            return false;
                    }

                return true;
            }
            return false;
        }

        public void FillTransactions(IEnumerable<string> transactions)
        {
            BuyTransactions = new List<SymbolTransaction>();
            SellTransactions = new List<SymbolTransaction>();

            foreach (var data in transactions)
            {
                var items = data.Split(",");
                BuyTransactions.Add(new SymbolTransaction
                {
                    Order = Convert.ToInt16(items[1]),
                    Number = Convert.ToInt64(items[3]),
                    Value = Convert.ToInt64(items[6]),
                    Price = Convert.ToInt64(items[4])
                });

                SellTransactions.Add(new SymbolTransaction
                {
                    Order = Convert.ToInt16(items[1]),
                    Number = Convert.ToInt64(items[2]),
                    Value = Convert.ToInt64(items[7]),
                    Price = Convert.ToInt64(items[5])
                });
            }

            BuyTransactions = BuyTransactions.Where(i => i.Price <= MaxAllowedPrice && i.Price >= MinAllowedPrice).OrderBy(i => i.Order).ToList();
            SellTransactions = SellTransactions.Where(i => i.Price <= MaxAllowedPrice && i.Price >= MinAllowedPrice).OrderBy(i => i.Order).ToList();

            BuyPrice = BuyTransactions.Any() ? BuyTransactions.First().Price : 0;
            SalePrice = SellTransactions.Any() ? SellTransactions.First().Price : 0;
        }

        public void FillClientValues(string[] values)
        {
            NaturalBuyNumber = Convert.ToInt64(values[1]);
            LegalEntityBuyNumber = Convert.ToInt64(values[2]);
            NaturalBuyValue = Convert.ToInt64(values[3]);
            LegalEntityBuyValue = Convert.ToInt64(values[4]);

            NaturalSellNumber = Convert.ToInt64(values[5]);
            LegalEntitySellNumber = Convert.ToInt64(values[6]);
            NaturalSellValue = Convert.ToInt64(values[7]);
            LegalEntitySellValue = Convert.ToInt64(values[8]);
        }
    }

    public class SymbolTransaction
    {
        public short Order { get; set; }
        public long Number { get; set; }
        public long Value { get; set; }
        public long Price { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is SymbolTransaction transaction)
                return Order == transaction.Order && Number == transaction.Number && Value == transaction.Value && Price == transaction.Price;

            return false;
        }
    }
}