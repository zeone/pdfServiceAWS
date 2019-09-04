using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PDFServiceAWS.Enums
{
    /// <summary>
    /// Used for contactReportAnd dor family report
    /// </summary>
    [DataContract]
    public enum TransFilterType
    {
        [EnumMember]
        Payment = 1,
        [EnumMember]
        Bill = 2,
        [EnumMember]
        Family = 3,
        [EnumMember]
        Member = 4
    }
}