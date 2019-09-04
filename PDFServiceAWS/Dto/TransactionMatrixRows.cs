using System.Collections.Generic;

namespace PDFServiceAWS.Dto
{
    public class TransactionMatrixRows
    {
        public string Name { get; set; }
        public string TotalName { get; set; }
        public FamilyDetails FamilyDetails { get; set; }
        public List<decimal> Amounts { get; set; }
        public List<TransactioMatrixSubDetails> SubDetails { get; set; }

    }
}