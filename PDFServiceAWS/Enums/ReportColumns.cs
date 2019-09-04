using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PDFServiceAWS.Enums
{
    [DataContract]
    public enum ReportColumns
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
        EmergencyPhone = 8,
        [EnumMember]
        Address = 9,
        [EnumMember]
        City = 10,
        [EnumMember]
        State = 11,
        [EnumMember]
        Zip = 12,
        [EnumMember]
        Country = 13,
        [EnumMember]
        Company = 14
    }
}