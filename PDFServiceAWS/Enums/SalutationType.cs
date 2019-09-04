using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PDFServiceAWS.Enums
{
    [DataContract]
    public enum SalutationType
    {
        [EnumMember]
        Personal = 1,
        [EnumMember]
        Formal = 2,
        [EnumMember]
        Manual = 3
    }
}