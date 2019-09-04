using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PDFServiceAWS.Enums
{
    [DataContract]
    public enum ContactTypes
    {
        [EnumMember]
        HomePhone = 1,
        [EnumMember]
        WorkPhone = 2,
        [EnumMember]
        MobilePhone = 3,
        [EnumMember]
        OtherPhone = 4,
        [EnumMember]
        Pager = 5,
        [EnumMember]
        Fax = 6,
        [EnumMember]
        Email = 7,
        [EnumMember]
        EmergencyPhone = 8
    }
}