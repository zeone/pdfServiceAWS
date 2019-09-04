using System.Collections.Generic;

namespace PDFServiceAWS.Dto
{
    public class TransactionReportResultDto
    {
        public List<TransactionReportFamily> Families { get; set; }
        public List<int> CategoriesIDs { get; set; }
        public List<TransactionsReportList> Transactions { get; set; }
        public List<TransactionsReportList> UnassignedPayments { get; set; }
        public int TotalCount { get; set; }
    }
}