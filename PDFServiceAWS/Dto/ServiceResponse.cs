using System;
using System.Runtime.Serialization;
using PDFServiceAWS.Enums;

namespace PDFServiceAWS.Dto
{

    public class UploadPdfReportDto
    {

        public int PdfReportId { get; set; }

        public Guid Key { get; set; }

        public string Schema { get; set; }

        public PDFReportStatus Status { get; set; }

        public byte[] PdfByteArr { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }
    }
}