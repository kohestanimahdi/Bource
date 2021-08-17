using Bource.Common.Utilities;
using Bource.Models.Data.Common;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Bource.Models.Data.Tsetmc
{
    public class SymbolData : MongoDataEntity
    {
        #region Constructors
        public SymbolData()
        {
        }

        public SymbolData(string[] data, DateTime lastModified)
        {
            LastUpdate = lastModified;

            InsCode = Convert.ToInt64(data[0]);
            SymbolCode = data[1];
            Symbol = data[2].FixPersianLetters();
            Name = data[3].FixPersianLetters();
            FirstPrice = Convert.ToDecimal(data[5]);
            FinishPrice = Convert.ToDecimal(data[6]);
            LastPrice = Convert.ToDecimal(data[7]);
            NumberOfTransaction = Convert.ToDecimal(data[8]);
            Turnover = Convert.ToDecimal(data[9]);
            ValueOfTransaction = Convert.ToDecimal(data[10]);
            MinPrice = Convert.ToDecimal(data[11]);
            MaxPrice = Convert.ToDecimal(data[12]);
            YesterdayPrice = Convert.ToDecimal(data[13]);
            EPS = string.IsNullOrWhiteSpace(data[14]) ? null : Convert.ToDecimal(data[14]);
            BaseValue = Convert.ToDecimal(data[15]);
            Status = Convert.ToInt32(data[17]);
            SymbolGroup = data[18];
            MaxAllowedPrice = Convert.ToDecimal(data[19]);
            MinAllowedPrice = Convert.ToDecimal(data[20]);
            Count = Convert.ToDecimal(data[21]);
            FinishPriceChange = FinishPrice - YesterdayPrice;
            LastPriceChange = NumberOfTransaction == 0 ? 0 : LastPrice - YesterdayPrice;
            ValueOfMarket = Count * FinishPrice;

            PercentFinishPriceChange = YesterdayPrice != 0 ? Math.Round(100 * FinishPriceChange / YesterdayPrice, 2) : 0;
            PercentLastPriceChange = NumberOfTransaction == 0 || YesterdayPrice == 0 ? 0 : Math.Round(100 * LastPriceChange / YesterdayPrice, 2);
            PE = EPS.HasValue && EPS.Value != 0 ? Math.Round(FinishPrice / EPS.Value, 2) : null;

            BuyTransactions = new List<SymbolTransaction>();
            SellTransactions = new List<SymbolTransaction>();
        }
        #endregion

        #region Properties
        public string SymbolCode { get; set; }

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
        public string Name { get; set; }

        public string Symbol { get; set; }

        public string SymbolGroup { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdate { get; set; }

        [Display(Name = "وضعیت")]
        public int Status { get; set; }

        [Display(Name = "تعداد معاملات")]
        public decimal NumberOfTransaction { get; set; }

        [Display(Name = "حجم معاملات")]
        public decimal Turnover { get; set; }

        [Display(Name = "ارزش معاملات")]
        public decimal ValueOfTransaction { get; set; }

        [Display(Name = "ارزش بازار")]
        public decimal ValueOfMarket { get; set; }

        [Display(Name = "قیمت دیروز")]
        public decimal YesterdayPrice { get; set; }

        [Display(Name = "خرید")]
        public decimal? BuyPrice { get; set; }

        [Display(Name = "فروش")]
        public decimal? SalePrice { get; set; }

        [Display(Name = "اولین قیمت")] //pf
        public decimal FirstPrice { get; set; }

        [Display(Name = "کمترین بازه روز")] //pmin
        public decimal MinPrice { get; set; }

        [Display(Name = "بیشترین بازه روز")] //pmax
        public decimal MaxPrice { get; set; }

        [Display(Name = "قیمت معامله")] //pl
        public decimal LastPrice { get; set; }

        [Display(Name = "تغییر قیمت معامله")] //plc
        public decimal LastPriceChange { get; set; }

        [Display(Name = "درصد تغییر قیمت معامله")] //plp
        public decimal PercentLastPriceChange { get; set; }

        [Display(Name = "قیمت پایانی")] //pc
        public decimal FinishPrice { get; set; }

        [Display(Name = "تغییر قیمت پایانی")] //pcc
        public decimal FinishPriceChange { get; set; }

        [Display(Name = "درصد تغییر قیمت پایانی")]//pcl
        public decimal PercentFinishPriceChange { get; set; }

        [Display(Name = "EPS")]
        public decimal? EPS { get; set; }

        [Display(Name = "PE")]
        public decimal? PE { get; set; }

        [Display(Name = "PS")]
        public decimal? PS { get; set; }

        [Display(Name = "PSR")]
        public decimal? PSR { get; set; }

        [Display(Name = "PE گروه")]
        public decimal? GroupPE { get; set; }

        [Display(Name = "تعداد سهام")]
        public decimal Count { get; set; }

        [Display(Name = "حجم مبنا")]
        public decimal BaseValue { get; set; }

        [Display(Name = "کمترین قیمت مجاز")]
        public decimal MinAllowedPrice { get; set; }

        [Display(Name = "بیشترین قیمت مجاز")]
        public decimal MaxAllowedPrice { get; set; }

        [Display(Name = "سهام شناور")]
        public decimal? FloatingStock { get; set; }

        [Display(Name = "میانگین حجم ماه")]
        public decimal? MonthAverageValue { get; set; }

        [Display(Name = "تعداد خریدار حقیقی")]
        public decimal NaturalBuyNumber { get; set; }

        [Display(Name = "تعداد خریدار حقوقی")]
        public decimal LegalEntityBuyNumber { get; set; }

        [Display(Name = "حجم خرید حقیقی")]
        public decimal NaturalBuyValue { get; set; }

        [Display(Name = "حجم خرید حقوقی")]
        public decimal LegalEntityBuyValue { get; set; }

        [Display(Name = "تعداد فروشنده حقیقی")]
        public decimal NaturalSellNumber { get; set; }

        [Display(Name = "تعداد فروشنده حقوقی")]
        public decimal LegalEntitySellNumber { get; set; }

        [Display(Name = "حجم فروش حقیقی")]
        public decimal NaturalSellValue { get; set; }

        [Display(Name = "حجم فروش حقوقی")]
        public decimal LegalEntitySellValue { get; set; }

        public List<SymbolTransaction> BuyTransactions { get; set; }
        public List<SymbolTransaction> SellTransactions { get; set; }
        #endregion

        #region Functions
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
                    Number = Convert.ToDecimal(items[3]),
                    Value = Convert.ToDecimal(items[6]),
                    Price = Convert.ToDecimal(items[4])
                });

                SellTransactions.Add(new SymbolTransaction
                {
                    Order = Convert.ToInt16(items[1]),
                    Number = Convert.ToDecimal(items[2]),
                    Value = Convert.ToDecimal(items[7]),
                    Price = Convert.ToDecimal(items[5])
                });
            }

            BuyTransactions = BuyTransactions.Where(i => i.Price <= MaxAllowedPrice && i.Price >= MinAllowedPrice).OrderBy(i => i.Order).ToList();
            SellTransactions = SellTransactions.Where(i => i.Price <= MaxAllowedPrice && i.Price >= MinAllowedPrice).OrderBy(i => i.Order).ToList();

            BuyPrice = BuyTransactions.Any() ? BuyTransactions.First().Price : 0;
            SalePrice = SellTransactions.Any() ? SellTransactions.First().Price : 0;
        }

        public void FillClientValues(string[] values)
        {
            NaturalBuyNumber = Convert.ToDecimal(values[1]);
            LegalEntityBuyNumber = Convert.ToDecimal(values[2]);
            NaturalBuyValue = Convert.ToDecimal(values[3]);
            LegalEntityBuyValue = Convert.ToDecimal(values[4]);

            NaturalSellNumber = Convert.ToDecimal(values[5]);
            LegalEntitySellNumber = Convert.ToDecimal(values[6]);
            NaturalSellValue = Convert.ToDecimal(values[7]);
            LegalEntitySellValue = Convert.ToDecimal(values[8]);
        }

        public void FillData(decimal? monthAverageValue, decimal? floatingStock, decimal? groupPE, decimal? pSR)
        {
            MonthAverageValue = monthAverageValue;

            FloatingStock = floatingStock;

            GroupPE = groupPE;

            PSR = pSR;

            PS = PSR.HasValue && PSR.Value != 0 ? Math.Round(FinishPrice / PSR.Value, 2) : null;
        }

        public Symbol GetSymbol()
            => new Symbol
            {
                InsCode = InsCode,
                Code12 = SymbolCode,
                Name = Name,
                Sign = Symbol,
                GroupId = SymbolGroup,
                ExistInType = Enums.SymbolExistInType.Tsetmc
            };

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        #endregion

        #region Calculates

        [Display(Name = "سرانه خرید حقیقی")]
        public decimal BuyPerNatural =>
            NaturalBuyNumber == 0 ? 0 : Math.Round(BuyValueNatural / NaturalBuyNumber, 2);


        [Display(Name = "سرانه فروش حقیقی")]
        public decimal SellPerNatural =>
            NaturalSellNumber == 0 ? 0 : Math.Round(SellValueNatural / NaturalSellNumber, 2);


        [Display(Name = "سرانه خرید حقوقی")]
        public decimal BuyPerLegal =>
            LegalEntityBuyNumber == 0 ? 0 : Math.Round(BuyValueLegal / LegalEntityBuyNumber, 2);


        [Display(Name = "سرانه فروش حقوقی")]
        public decimal SellPerLegal =>
            LegalEntitySellNumber == 0 ? 0 : Math.Round(SellValueLegal / LegalEntitySellNumber, 2);


        [Display(Name = "ارزش خرید حقیقی")]
        public decimal BuyValueNatural =>
            NaturalBuyValue * FinishPrice;


        [Display(Name = "ارزش فروش حقیقی")]
        public decimal SellValueNatural =>
            NaturalSellValue * FinishPrice;


        [Display(Name = "ارزش خرید حقوقی")]
        public decimal BuyValueLegal =>
           LegalEntityBuyValue * FinishPrice;


        [Display(Name = "ارزش فروش حقوقی")]
        public decimal SellValueLegal =>
            LegalEntitySellValue * FinishPrice;

        #endregion 
    }

    public class SymbolTransaction
    {
        public short Order { get; set; }
        public decimal Number { get; set; }
        public decimal Value { get; set; }
        public decimal Price { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is SymbolTransaction transaction)
                return Order == transaction.Order && Number == transaction.Number && Value == transaction.Value && Price == transaction.Price;

            return false;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }
}