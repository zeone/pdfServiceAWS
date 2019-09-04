using System;

namespace PDFServiceAWS.Dto
{
    public class ContactReportContactInfo : ICloneable
    {
        public string PhoneNumber { get; set; }
        public int PhoneTypeID { get; set; }
        //is primary contact
        public bool? IsPrimary { get; set; }
        //chack if can we call for member
        public bool? NoCall { get; set; }
        public int MemberType { get; set; }
        public object Clone()
        {
            return new ContactReportContactInfo
            {
                PhoneTypeID = this.PhoneTypeID,
                MemberType = this.MemberType,
                PhoneNumber = this.PhoneNumber,
                NoCall = this.NoCall,
                IsPrimary = IsPrimary
            };
        }
    }

}