using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDFServiceAWS.Enums
{
    public enum PDFReportStatus
    {
        NotCreated = 1,
        Ready = 2,
        InProgress = 3,
        Outdated = 4,
        Error = 5,
        GenerationError = 6
    }
}
