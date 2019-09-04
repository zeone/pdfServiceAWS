using System.Collections.Generic;

namespace PDFServiceAWS.Dto
{
    public class GroupedContactReport
    {
        public string Name { get; set; }
        public List<ContactReportResultDto> Contacts { get; set; }
    }
}