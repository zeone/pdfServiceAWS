using System.Collections.Generic;

namespace PDFServiceAWS.Dto
{
    public class PdfDocumentDto
    {
        public ReportDto ReportDto { get; set; }
        public List<ContactReportResultDto> Contacts { get; set; }
        public FilterTransactionReport Filter { get; set; }
        public TransactionGrouped Grouped { get; set; }
        public int CountTrans { get; set; }
    }

}