using System;
using PDFServiceAWS.Enums;

namespace PDFServiceAWS.Dto
{
    public class ContactReportAddress : ICloneable
    {
        public int AddressID { get; set; }
        public int? AddressTypeID { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Label { get; set; }
        public string HisLabel { get; set; }
        public string HerLabel { get; set; }
        public string Salutation { get; set; }
        public bool AddrPrimary { get; set; }
        public bool AddrCurrent { get; set; }
        public bool AddrNoMail { get; set; }
        public SalutationType SalSetting { get; set; }
        public object Clone()
        {
            return new ContactReportAddress
            {
                AddressID = this.AddressID,
                AddressTypeID = this.AddressTypeID,
                CompanyName = this.CompanyName,
                Address = this.Address,
                Address2 = this.Address2,
                Country = this.Country,
                City = this.City,
                State = this.State,
                Zip = this.Zip,
                AddrPrimary = this.AddrPrimary,
                AddrCurrent = this.AddrCurrent,
                AddrNoMail = this.AddrNoMail,
                SalSetting = this.SalSetting,
                Label = this.Label,
                HisLabel = this.HisLabel,
                HerLabel = this.HerLabel
            };
        }
    }
}