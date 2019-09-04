using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PDFServiceAWS.Enums
{
    [DataContract]
    public enum ReportTypes
    {
        [EnumMember]
        ContactReport = 1,
        [EnumMember]
        TransactionReport = 2
    }
}