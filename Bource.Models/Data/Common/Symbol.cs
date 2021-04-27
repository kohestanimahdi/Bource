namespace Bource.Models.Data.Common
{
    public class Symbol : MongoDataEntity
    {
        public string Name { get; set; }
        public string Sign { get; set; }
        public string LatinName { get; set; }
        public string LatinSign { get; set; }
        public string Panel { get; set; }

        public string IId { get; set; }
        public string Code12 { get; set; }
        public string Code5 { get; set; }
        public string Code4 { get; set; }
        public string CompanyName { get; set; }
        public string Name30 { get; set; }
        public string CompanyCode { get; set; }
        public string Market { get; set; }
        public string PanelCode { get; set; }
        public string GroupName { get; set; }
        public string GroupId { get; set; }
        public string SubGroupName { get; set; }
        public string SubGroupCode { get; set; }
    }
}
