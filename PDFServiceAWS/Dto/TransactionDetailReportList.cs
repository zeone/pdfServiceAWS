using System;

namespace PDFServiceAWS.Dto
{
    public class TransactionDetailReportList
    {
        public int? TransactionDetailID { get; set; }
        public int? BillID { get; set; }
        public DateTime? DateDue { get; set; }
        public int? CategoryID { get; set; }
        public int? SubcategoryID { get; set; }
        public int? ClassID { get; set; }
        public int? SubclassID { get; set; }
        public int? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }

        public decimal Amount { get; set; }
        public decimal? AmountDue { get; set; }
        public string Note { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public string Class { get; set; }
        public string Subclass { get; set; }
        public int TransType { get; set; }
        public bool? CardCharged { get; set; }
        public decimal? PaidAmount { get; set; }
    }
}