using System.Collections.Generic;

namespace PDFServiceAWS.Dto
{
    public class FamilyDetails
    {
        public IList<ContactReportContactInfo> Contacts { get; set; }
        public IList<ContactReportAddress> Addresses { get; set; }
    }
}