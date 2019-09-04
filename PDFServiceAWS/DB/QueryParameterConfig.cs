using System.Data;

namespace PDFServiceAWS.DB
{
    public class QueryParameterConfig
    {
        protected bool Equals(QueryParameterConfig other)
        {
            return DbType == other.DbType &&
                   Size == other.Size &&
                   IsStoredProcedureParameter == other.IsStoredProcedureParameter &&
                   NoQuotes == other.NoQuotes &&
                   IsNullable == other.IsNullable &&
                   Direction == other.Direction;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Equals((QueryParameterConfig)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)DbType;
                hashCode = (hashCode * 397) ^ Size;
                hashCode = (hashCode * 397) ^ IsStoredProcedureParameter.GetHashCode();
                hashCode = (hashCode * 397) ^ NoQuotes.GetHashCode();
                hashCode = (hashCode * 397) ^ IsNullable.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)Direction;
                return hashCode;
            }
        }

        /// <summary>
        /// Name of the DB parameter
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// SQL DB type
        /// </summary>
        public SqlDbType DbType { get; set; }

        /// <summary>
        /// Size/scale of a DB parameter value
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Mark this parameter as a stored procedure parameter
        /// </summary>
        public bool IsStoredProcedureParameter { get; set; }


        /// <summary>
        /// No quotes needed when un-parameterizing query parameters
        /// </summary>
        /// <summary>
        /// Value of this parameter should not be quoted 
        /// </summary>
        public bool NoQuotes { get; set; }

        /// <summary>
        /// This parameter allows NULL values
        /// </summary>
        /// <summary>
        /// Parameter can set NULL values
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Direction of a parameter (input, output, return value)
        /// </summary>
        /// <summary>
        /// Direction of a DB parameter (input, output)
        /// </summary>
        public ParameterDirection Direction { get; set; }

    }
}