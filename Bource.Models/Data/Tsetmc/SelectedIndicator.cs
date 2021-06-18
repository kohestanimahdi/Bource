using Bource.Common.Utilities;
using HtmlAgilityPack;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bource.Models.Data.Tsetmc
{
    public class SelectedIndicator : MongoDataEntity
    {
        public SelectedIndicator()
        {
        }

        public SelectedIndicator(long insCode, HtmlNodeCollection tds)
        {
            //var title = tds[0].GetText().Split('-');
            //Title = title.Length > 1 ? title[1] : title[0];
            Title = tds[0].GetText();
            InsCode = insCode;
            PublishTime = DateTime.Parse(tds[1].GetText());
            Last = tds[2].ConvertToDecimal();
            Change = tds[3].SelectSingleNode("div")?.ConvertToNegativePositiveDecimal() ?? 0;
            ChangePercent = tds[4].SelectSingleNode("div")?.ConvertToNegativePositiveNumber() ?? 0;
            Max = tds[5].ConvertToDecimal();
            Min = tds[6].ConvertToDecimal();
        }

        public string Title { get; set; }

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

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PublishTime { get; set; }

        public decimal Last { get; set; }
        public decimal Change { get; set; }
        public double ChangePercent { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
    }
}