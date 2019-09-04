﻿namespace PDFServiceAWS.Dto
{
    public class ContactReportDto
    {
        public int FamilyID { get; set; }
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string FamilyName { get; set; }
        public string HebrewName { get; set; }
        public string HebrewFather { get; set; }
        public string HebrewMother { get; set; }
        public string Gender { get; set; }
        public int TypeID { get; set; }
        public string PhoneNumber { get; set; }
        public int PhoneTypeID { get; set; }
        //is primary contact
        public bool? IsPrimary { get; set; }
        public bool? NoCall { get; set; }
        public int AddressID { get; set; }
        public int AddressTypeID { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool AddrPrimary { get; set; }
        public bool AddrCurrent { get; set; }
        public bool AddrNoMail { get; set; }
        public string FamilySalutation { get; set; }
        public string HisSalutation { get; set; }
        public string HerSalutation { get; set; }
        public string FamilyLabel { get; set; }
        public string HisLabel { get; set; }
        public string HerLabel { get; set; }
        public SalutationType SalSetting { get; set; }
    }
}