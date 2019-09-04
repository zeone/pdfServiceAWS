using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace PDFServiceAWS.Enums
{
    [DataContract]
    public enum TransactioReportColumns
    {
        [EnumMember]
        Company = 0,
        [EnumMember]
        Address = 1,
        [EnumMember]
        CatSubcat = 2,
        [EnumMember]
        CheckNum = 3,
        [EnumMember]
        Date = 4,
        [EnumMember]
        ReceiptNum = 5,
        [EnumMember]
        Solicitor = 6,
        [EnumMember]
        Mailing = 7,
        [EnumMember]
        Department = 8,
        [EnumMember]
        InvoiceNum = 9,
        [EnumMember]
        Note = 10,
        [EnumMember]
        Method = 11,
        [EnumMember]
        DateDue = 12,
        [EnumMember]
        Email = 13,
        [EnumMember]
        MobilePhone = 14,
        [EnumMember]
        HomePhone = 15,
        [EnumMember]
        WorkPhone = 16,
        [EnumMember]
        OtherPhone = 17,
        [EnumMember]
        Pager = 18,
        [EnumMember]
        EmergencyPhone = 19,
        [EnumMember]
        Fax = 20,
        [EnumMember]
        Quantity = 21
    }
}