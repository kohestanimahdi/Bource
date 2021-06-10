using System;

namespace Bource.Models.Data.Tsetmc
{
    public class Indicator : MongoDataEntity
    {
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

        public override bool Equals(object obj)
        {
            if (obj is Indicator indicator)
                return Title == indicator.Title && InsCode == indicator.InsCode;
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
