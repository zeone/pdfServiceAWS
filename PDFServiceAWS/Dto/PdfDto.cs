namespace PDFServiceAWS.Dto
{
    public class PdfDto
    {
        public int TransactionId { get; set; }
        public int? LetterId { get; set; }
        public object HtmlBody { get; set; }
        public string PlaintText { get; set; }
    }
}