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
    
    public enum TransFilterType
    {
         
        Payment = 1,
         
        Bill = 2,
         
        Family = 3,
         
        Member = 4
    }
}