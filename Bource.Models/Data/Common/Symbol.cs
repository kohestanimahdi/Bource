﻿using Bource.Common.Utilities;
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

        public string Subject { get; set; }
        public string Logo { get; set; }
        public string CodalURL { get; set; }
        public string CodalCompanyId { get; set; }
        public string CodalTitle { get; set; }
        public string CodalType { get; set; }
        public string CodalClassification { get; set; }
        public string CodalCode { get; set; }

        public SymbolExistInType ExistInType { get; set; }
        public string Name { get; set; }
        public string Sign { get; set; }
        public string LatinName { get; set; }

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

        public string Status { get; set; }
        public string PaperTitle { get; set; }
        public PapersTypes? PaperType { get; set; }

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

        public void UpdateFromTseClientSoap(string[] array)
        {
            InsCode = Convert.ToInt64(array[0].ToString());
            InstrumentId = array[1].ToString()?.Replace('‌', ' ').FixPersianLetters();
            LatinSymbol = array[2].ToString()?.Replace('‌', ' ').FixPersianLetters();
            LatinName = array[3].ToString()?.Replace('‌', ' ').FixPersianLetters();
            CompanyCode = array[4].ToString()?.Replace('‌', ' ').FixPersianLetters();
            Sign = array[5].ToString()?.Replace('‌', ' ').FixPersianLetters();
            Name = array[6].ToString()?.Replace('‌', ' ').FixPersianLetters();
            CIsin = array[7].ToString()?.Replace('‌', ' ').FixPersianLetters();
            DEven = Convert.ToInt32(array[8].ToString());
            Flow = Convert.ToByte(array[9].ToString());
            LSoc30 = array[10].ToString()?.Replace('‌', ' ').FixPersianLetters();
            CGdSVal = array[11].ToString()?.Replace('‌', ' ').FixPersianLetters();
            CGrValCot = array[12].ToString()?.Replace('‌', ' ').FixPersianLetters();
            YMarNSC = array[13].ToString()?.Replace('‌', ' ').FixPersianLetters();
            CComVal = array[14].ToString()?.Replace('‌', ' ').FixPersianLetters();
            CSecVal = array[15].ToString()?.Replace('‌', ' ').FixPersianLetters();
            CSoSecVal = array[16].ToString()?.Replace('‌', ' ').FixPersianLetters();
            YVal = array[17].ToString()?.Replace('‌', ' ').FixPersianLetters();
            ExistInType = SymbolExistInType.TseClient;
        }

        public void UpdateFromTseClient(Symbol symbol)
        {
            InsCode = symbol.InsCode;
            InstrumentId = symbol.InstrumentId.FixPersianLetters();
            LatinSymbol = symbol.LatinSymbol.FixPersianLetters();
            LatinName = symbol.LatinName.FixPersianLetters();
            CompanyCode = symbol.CompanyCode.FixPersianLetters();
            Sign = symbol.Sign.FixPersianLetters();
            Name = symbol.Name.FixPersianLetters();
            CIsin = symbol.CIsin.FixPersianLetters();
            DEven = symbol.DEven;
            Flow = symbol.Flow;
            LSoc30 = symbol.LSoc30.FixPersianLetters();
            CGdSVal = symbol.CGdSVal.FixPersianLetters();
            CGrValCot = symbol.CGrValCot.FixPersianLetters();
            YMarNSC = symbol.YMarNSC.FixPersianLetters();
            CComVal = symbol.CComVal.FixPersianLetters();
            CSecVal = symbol.CSecVal.FixPersianLetters();
            CSoSecVal = symbol.CSoSecVal.FixPersianLetters();
            YVal = symbol.YVal.FixPersianLetters();
            if (ExistInType != SymbolExistInType.Both)
                ExistInType = SymbolExistInType.TseClient;
        }

        public void UpdateFromTsetmc(Symbol symbol)
        {
            InsCode = symbol.InsCode;
            Code12 = symbol.Code12.FixPersianLetters();
            Name = symbol.Name.FixPersianLetters();
            Sign = symbol.Sign.FixPersianLetters();
            GroupId = symbol.GroupId.FixPersianLetters();
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