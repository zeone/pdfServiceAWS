using System.Collections.Generic;

namespace PDFServiceAWS.Dto
{
    public class MatrixDTO : TransactionGrouped
    {
        public List<TransactionMatrixColumn> Columns { get; set; }
        public List<TransactionMatrixRows> Rows { get; set; }
    }
}