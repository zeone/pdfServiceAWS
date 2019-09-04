using System;
using System.Data;

namespace PDFServiceAWS.DB.CustomTables
{
    public class StringArrayDataRow : DataRow
    {
        public StringArrayDataRow(DataRowBuilder builder) : base(builder) { }

        public string Item
        {
            get { return this["Item"] == DBNull.Value ? (string)this["Item"] : null; }
            set { this["Item"] = (object)value ?? DBNull.Value; }
        }
    }
}