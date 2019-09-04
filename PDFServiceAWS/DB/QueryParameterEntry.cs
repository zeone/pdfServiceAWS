using System.Data.SqlClient;

namespace PDFServiceAWS.DB
{
    internal class QueryParameterEntry
    {
        /// <summary>
        /// Database parameter 
        /// </summary>
        public SqlParameter Parameter { get; set; }

        /// <summary>
        /// Parameter configuration
        /// </summary>
        public QueryParameterConfig Config { get; set; }
    }
}