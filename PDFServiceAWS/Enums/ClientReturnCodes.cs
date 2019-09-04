using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFServiceAWS.Enums
{
    public enum ClientReturnCodes
    {
        None = -1,
        OK = 0,
        Error = 1,
        RVValidationError = 2,
        DeleteForbidden = 3,
        NeedId = 4
    }
}