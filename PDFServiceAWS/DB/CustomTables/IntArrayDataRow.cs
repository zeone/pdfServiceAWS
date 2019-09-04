using System;
using System.Data;

namespace PDFServiceAWS.DB.CustomTables
{
    public class IntArrayDataRow : DataRow
    {
        public IntArrayDataRow(DataRowBuilder builder) : base(builder) { }

        public int? Item
        {
            get { return this["Item"] == DBNull.Value ? (int?)this["Item"] : null; }
            set { this["Item"] = (object)value ?? DBNull.Value; }
        }
    }
}