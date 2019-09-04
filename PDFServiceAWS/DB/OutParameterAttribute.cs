using System;
using System.Data;

namespace PDFServiceAWS.DB
{
    [AttributeUsage(AttributeTargets.Property)]
    public class OutParameterAttribute : System.Attribute
    {
        public string ParamName { get; private set; }
        public SqlDbType SqlType { get; private set; }
        public OutParameterAttribute(string paramName, SqlDbType type)
        {
            ParamName = paramName;
            SqlType = type;
        }
    }
}