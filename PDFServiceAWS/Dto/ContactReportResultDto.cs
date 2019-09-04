using System;
using System.Collections.Generic;
using System.Linq;

namespace PDFServiceAWS.Dto
{
    public class ContactReportResultDto
    {
        public int FamilyID { get; set; }
        public string FamilyName { get; set; }
        public string FamilySalutation { get; set; }
        public string HisSalutation { get; set; }
        public string HerSalutation { get; set; }
        public string FamilyLabel { get; set; }
        public string HisLabel { get; set; }
        public string HerLabel { get; set; }
        public IList<ContactReportAddress> Addresses { get; set; }
        public IList<ContactReportMambers> Members { get; set; }
        public int TotalCount { get; set; }
    }

    public class ContactReportMambers : ICloneable
    {
        public int MemberID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public string HebrewName { get; set; }
        public string HebrewFather { get; set; }
        public string HebrewMother { get; set; }
        public int TypeID { get; set; }
        public string Gender { get; set; }
        public IList<ContactReportContactInfo> Contacts { get; set; }
        public object Clone()
        {
            return new ContactReportMambers
            {
                MemberID = MemberID,
                Contacts = Contacts.Select(r => (ContactReportContactInfo)r.Clone()).ToList(),
                FirstName = FirstName,
                Title = Title,
                TypeID = TypeID,
                LastName = LastName,
                Gender = Gender
            };
        }
    }

  

   
}