using Bource.Common.Utilities;
using Bource.Models.Data.Enums;
using HtmlAgilityPack;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Text.RegularExpressions;

namespace Bource.Models.Data.FipIran
{
    public class FipIranNews : MongoDataEntity
    {
        private static Regex regex = new("^[0-9][0-9][0-9][0-9]/[0-9][0-9]/[0-9][0-9] [0-9][0-9]:[0-9][0-9]:[0-9][0-9]");

        public FipIranNews()
        {
        }

        public FipIranNews(HtmlNode div, FipIranNewsTypes type)
        {
            var time = regex.Match(div.SelectSingleNode("span").GetText());
            CreateDate = DateTime.Now;
            Url = div.SelectSingleNode("a").Attributes["href"].Value;
            Title = div.SelectSingleNode("a/b").GetText();
            Description = div.SelectSingleNode("a/div").GetText();
            NewsAgency = div.SelectSingleNode("span/span").GetText();
            Type = type;
            Time = time.Value.GetAsDateTime();
        }

        public string Title { get; set; }
        public string Description { get; set; }
        public string NewsAgency { get; set; }
        public string Url { get; set; }

        public FipIranNewsTypes Type { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is FipIranNews news)
            {
                return news.Time == Time && news.Title == Title;
            }
            return false;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}