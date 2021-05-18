namespace Bource.Models.Data.Tsetmc
{
    public class Indicator : MongoDataEntity
    {
        public string Title { get; set; }
        public long InsCode { get; set; }


        public override bool Equals(object obj)
        {
            if (obj is Indicator indicator)
                return Title == indicator.Title && InsCode == indicator.InsCode;
            return false;
        }
    }
}
