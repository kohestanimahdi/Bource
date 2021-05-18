using Bource.Common.Utilities;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bource.Models.Data.Tsetmc
{
    public class NaturalAndLegalEntity : MongoDataEntity
    {
        public NaturalAndLegalEntity()
        {
        }

        public NaturalAndLegalEntity(long insCode, string[] items)
        {
            Date = DateTime.ParseExact(items[0], "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
            BuyCountNatural = items[1].ConvertToLong();
            BuyCountLegal = items[2].ConvertToLong();
            SellCountNatural = items[3].ConvertToLong();
            SellCountLegal = items[4].ConvertToLong();
            BuyValueNatural = items[5].ConvertToDecimal();
            BuyValueLegal = items[6].ConvertToDecimal();
            SellValueNatural = items[7].ConvertToDecimal();
            SellValueLegal = items[8].ConvertToDecimal();
            BuyPriceNatural = items[9].ConvertToDecimal();
            BuyPriceLegal = items[10].ConvertToDecimal();
            SellPriceNatural = items[11].ConvertToDecimal();
            SellPriceLegal = items[12].ConvertToDecimal();
            InsCode = insCode;
            CreateDate = DateTime.Now;
        }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Date { get; set; }

        public long InsCode { get; set; }

        public long BuyCountNatural { get; set; }
        public long BuyCountLegal { get; set; }
        public long SellCountNatural { get; set; }
        public long SellCountLegal { get; set; }

        public decimal BuyValueNatural { get; set; }
        public decimal BuyValueLegal { get; set; }
        public decimal SellValueNatural { get; set; }
        public decimal SellValueLegal { get; set; }

        public decimal BuyPriceNatural { get; set; }
        public decimal BuyPriceLegal { get; set; }
        public decimal SellPriceNatural { get; set; }
        public decimal SellPriceLegal { get; set; }

        public decimal Change
        {
            get
            {
                return BuyValueNatural - SellValueNatural;
            }
        }

        public decimal BuyValueNaturalPercent
        {
            get
            {
                return BuyValueNatural / (BuyValueNatural + BuyValueLegal);
            }
        }

        public decimal BuyValueLegalPercent
        {
            get
            {
                return BuyValueLegal / (BuyValueNatural + BuyValueLegal);
            }
        }

        public decimal SellValueNaturalPercent
        {
            get
            {
                return SellValueNatural / (SellValueNatural + SellValueLegal);
            }
        }

        public decimal SellValueLegalPercent
        {
            get
            {
                return SellValueLegal / (SellValueNatural + SellValueLegal);
            }
        }
    }
}