using System.Runtime.Serialization;

namespace PDFServiceAWS.Dto
{
    [DataContract]
    public class BaseFilterRequest
    {
        [DataMember]
        public int ReportQId { get; set; }
        /// <summary>
        /// db schema name
        /// </summary>
        [DataMember]
        public string Schema { get; set; }

        /// <summary>
        /// organization country name
        /// </summary>
        [DataMember]
        public string CountryName { get; set; }

        /// <summary>
        /// required filters
        /// </summary>
        [DataMember]
        public ReportDto ReportDto { get; set; }

        /// <summary>
        /// filter criteria
        /// </summary>
        [DataMember]
        public FilterTransactionReport Filter { get; set; }
    }
}