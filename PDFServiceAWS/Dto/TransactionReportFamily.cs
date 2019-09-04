using System.Collections.Generic;

namespace PDFServiceAWS.Dto
{
    public class TransactionReportFamily
    {
        public int FamilyID { get; set; }
        public string Family { get; set; }
        public int AddressID { get; set; }
        public string FamilySalutation { get; set; }
        public string HisSalutation { get; set; }
        public string HerSalutation { get; set; }
        public string FamilyLabel { get; set; }
        public string HisLabel { get; set; }
        public string HerLabel { get; set; }
        public string Company { get; set; }
        public IList<ContactReportContactInfo> Contacts { get; set; }
        public IList<ContactReportAddress> Addresses { get; set; }

    }
}