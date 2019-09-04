using System.Collections.Generic;

namespace PDFServiceAWS.Dto
{
    public class TransactionGrouped
    {
        public decimal? GrandAmount { get; set; }
        public decimal? GrandDue { get; set; }
        public decimal? GrandPaid { get; set; }
        public decimal? GrandUnasignedAmount { get; set; }
        public List<TransactionReportGroupedDTO> GroupedObj { get; set; }
    }
}