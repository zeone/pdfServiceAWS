using System;
using System.Collections.Generic;

namespace PDFServiceAWS.Dto
{
    public class TransactionsReportList
    {
        public int TransactionID { get; set; }
        public int FamilyID { get; set; }
        public int? BillID { get; set; }
        public int? InvoiceNo { get; set; }
        public int? AddressID { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public decimal? AmountDue { get; set; }
        public string CheckNo { get; set; }
        public int TransType { get; set; }
        public int? PayID { get; set; }
        public string AuthNumber { get; set; }
        public int? PaymentMethodID { get; set; }
        public bool? IsReceipt { get; set; }
        public int? ReceiptNo { get; set; }
        public bool? ReceiptSent { get; set; }
        public int? LetterID { get; set; }
        public int? SolicitorID { get; set; }
        public int DepartmentID { get; set; }
        public int? MailingID { get; set; }
        public string HonorMemory { get; set; }
        public string Note { get; set; }
        public string AuthTransactionId { get; set; }
        public List<TransactionDetailReportList> Details { get; set; }
    }
}