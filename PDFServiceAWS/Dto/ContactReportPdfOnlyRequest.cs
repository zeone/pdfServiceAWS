using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PDFServiceAWS.Dto
{
    [DataContract]
    public class ContactReportPdfOnlyRequest : BaseFilterRequest
    {

        /// <summary>
        /// list of prepared contact info
        /// </summary>
        [DataMember]
        public List<ContactReportResultDto> Contacts { get; set; }

    }
}