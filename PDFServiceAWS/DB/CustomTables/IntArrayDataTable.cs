using System;
using System.Data;

namespace PDFServiceAWS.DB.CustomTables
{
    public class IntArrayDataTable : DataTable
    {
        public IntArrayDataTable()
        {
            Columns.Add("Item", typeof(int)).AllowDBNull = true;
        }
        protected override DataRow NewRowFromBuilder(DataRowBuilder builder)
        {
            return new IntArrayDataRow(builder);
        }

        protected override Type GetRowType()
        {
            return typeof(IntArrayDataRow);
        }

        public void AddItems(int?[] items)
        {
            if (items == null) return;
            Rows.Clear();
            foreach (int? t in items)
            {
                var row = NewRow() as IntArrayDataRow;
                row.Item = t;
                Rows.Add(row);
            }
        }

        public int?[] GetItems()
        {
            var items = new int?[Rows.Count];
            for (int i = 0; i < Rows.Count; i++)
                items[i] = ((IntArrayDataRow)Rows[i]).Item;
            return items;
        }
    }
}