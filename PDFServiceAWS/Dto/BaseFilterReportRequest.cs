using System;
using System.Runtime.Serialization;

namespace PDFServiceAWS.Dto
{

    public class BaseFilterReportRequest
    {

        public int PdfReportId { get; set; }

        /// <summary>
        /// used for security
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// db schema name
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// organization country name
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// required filters
        /// </summary>
        public ReportDto ReportDto { get; set; }

        /// <summary>
        /// filter criteria
        /// </summary>
        public FilterTransactionReport Filter { get; set; }
    }
}