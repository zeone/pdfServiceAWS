using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PDFServiceAWS.Dto
{
    
    public class ContactReportPdfOnlyRequest : BaseFilterRequest
    {

        /// <summary>
        /// list of prepared contact info
        /// </summary>
         
        public List<ContactReportResultDto> Contacts { get; set; }

    }
}