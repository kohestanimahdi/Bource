namespace Bource.Models.Data.Tsetmc
{
    public class Indicator : MongoDataEntity
    {
        public string Title { get; set; }
        public string IId { get; set; }


        public override bool Equals(object obj)
        {
            if (obj is Indicator indicator)
                return Title == indicator.Title && IId == indicator.IId;
            return false;
        }
    }
}
