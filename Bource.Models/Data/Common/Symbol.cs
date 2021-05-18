using Bource.Common.Utilities;
using Bource.Models.Data.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Bource.Models.Data.Common
{
    public class Symbol : MongoDataEntity
    {
        public Symbol()
        {

        }
        public Symbol(string[] array)
        {
            UpdateFromTseClientSoap(array);
        }
        public SymbolExistInType ExistInType { get; set; }
        public string Name { get; set; }
        public string Sign { get; set; }
        public string LatinName { get; set; }

        public long InsCode { get; set; }
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



        [BsonIgnore]
        public string InstrumentId
        {
            get
            {
                return Code12;
            }
            set
            {
                Code12 = value;
            }
        }

        [BsonIgnore]
        public string LatinSymbol
        {
            get
            {
                return Code5;
            }
            set
            {
                Code5 = value;
            }
        }

        [BsonIgnore]
        public string CIsin
        {
            get
            {
                return CompanyCode;
            }
            set
            {
                CompanyCode = value;
            }
        }

        [BsonIgnore]
        public string LSoc30
        {
            get
            {
                return Name30;
            }
            set
            {
                Name30 = value;
            }
        }

        [BsonIgnore]
        public string CComVal
        {
            get
            {
                return PanelCode;
            }
            set
            {
                PanelCode = value;
            }
        }

        [BsonIgnore]
        public string CSecVal
        {
            get
            {
                return GroupId;
            }
            set
            {
                GroupId = value;
            }
        }

        [BsonIgnore]
        public string CSoSecVal
        {
            get
            {
                return SubGroupCode;
            }
            set
            {
                SubGroupCode = value;
            }
        }

        public string CGdSVal { get; set; }
        public string CGrValCot { get; set; }
        public int DEven { get; set; }
        public byte Flow { get; set; }
        public string YMarNSC { get; set; }
        public string YVal { get; set; }


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
            //ExistInType = SymbolExistInType.Tsetmc;
        }

        public void UpdateFromTseClientSoap(string[] array)
        {
            InsCode = Convert.ToInt64(array[0].ToString());
            InstrumentId = array[1].ToString();
            LatinSymbol = array[2].ToString();
            LatinName = array[3].ToString();
            CompanyCode = array[4].ToString();
            Sign = array[5].ToString();
            Name = array[6].ToString();
            CIsin = array[7].ToString();
            DEven = Convert.ToInt32(array[8].ToString());
            Flow = Convert.ToByte(array[9].ToString());
            LSoc30 = array[10].ToString();
            CGdSVal = array[11].ToString();
            CGrValCot = array[12].ToString();
            YMarNSC = array[13].ToString();
            CComVal = array[14].ToString();
            CSecVal = array[15].ToString();
            CSoSecVal = array[16].ToString();
            YVal = array[17].ToString();
            ExistInType = SymbolExistInType.TseClient;
        }

        public void UpdateFromTseClient(Symbol symbol)
        {

            InsCode = symbol.InsCode;
            InstrumentId = symbol.InstrumentId;
            LatinSymbol = symbol.LatinSymbol;
            LatinName = symbol.LatinName;
            CompanyCode = symbol.CompanyCode;
            Sign = symbol.Sign;
            Name = symbol.Name;
            CIsin = symbol.CIsin;
            DEven = symbol.DEven;
            Flow = symbol.Flow;
            LSoc30 = symbol.LSoc30;
            CGdSVal = symbol.CGdSVal;
            CGrValCot = symbol.CGrValCot;
            YMarNSC = symbol.YMarNSC;
            CComVal = symbol.CComVal;
            CSecVal = symbol.CSecVal;
            CSoSecVal = symbol.CSoSecVal;
            YVal = symbol.YVal;
            if (ExistInType != SymbolExistInType.Both)
                ExistInType = SymbolExistInType.TseClient;
        }

        public void UpdateFromTsetmc(Symbol symbol)
        {
            InsCode = symbol.InsCode;
            Code12 = symbol.Code12;
            Name = symbol.Name;
            Sign = symbol.Sign;
            GroupId = symbol.GroupId;
            ExistInType = symbol.ExistInType;

            if (ExistInType != SymbolExistInType.Both)
                ExistInType = SymbolExistInType.Tsetmc;
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