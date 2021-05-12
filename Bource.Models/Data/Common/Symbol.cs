using Bource.Common.Utilities;

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

        public SymbolIntroduction Introduction { get; set; }

        public void UpdateInforamtion(HtmlAgilityPack.HtmlNodeCollection nodes)
        {
            Code12 = nodes[1].GetText();
            Code5 = nodes[3].GetText();
            LatinName = nodes[5].GetText();
            Code4 = nodes[7].GetText();
            CompanyName = nodes[9].GetText();
            Name30 = nodes[13].GetText();
            CompanyCode = nodes[15].GetText();
            Market = nodes[17].GetText();
            PanelCode = nodes[19].GetText();
            GroupId = nodes[21].GetText();
            GroupName = nodes[23].GetText();
            SubGroupCode = nodes[25].GetText();
            SubGroupName = nodes[27].GetText();
        }
    }

    public class SymbolIntroduction
    {
        public SymbolIntroduction()
        {
        }

        public SymbolIntroduction(HtmlAgilityPack.HtmlNodeCollection nodes)
        {
            Subject = nodes[1].GetText();
            CEO = nodes[3].GetText();
            Address = nodes[5].GetText();
            Phone = nodes[7].GetText();
            Fax = nodes[9].GetText();
            OfficeAddress = nodes[11].GetText();
            StockAffairsAddress = nodes[13].GetText();
            WebSite = nodes[15].GetText();
            Email = nodes[17].GetText();
            Auditor = nodes[19].GetText();
            Found = nodes[21].GetText();
            Year = nodes[23].GetText();
            FinancialManager = nodes[25].GetText();
            NationalId = nodes[27].GetText();
        }

        public string Subject { get; set; }
        public string CEO { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string OfficeAddress { get; set; }
        public string StockAffairsAddress { get; set; }
        public string WebSite { get; set; }
        public string Email { get; set; }
        public string Auditor { get; set; }
        public string Found { get; set; }
        public string Year { get; set; }
        public string FinancialManager { get; set; }
        public string NationalId { get; set; }
    }
}