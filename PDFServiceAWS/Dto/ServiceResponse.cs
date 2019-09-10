using System;
using System.Runtime.Serialization;

namespace PDFServiceAWS.Dto
{

    public class ServiceResponse
    {

        public int PdfReportId { get; set; }

        public Guid Key { get; set; }

        public string Schema { get; set; }

        public bool IsSuccess { get; set; }

        public byte[] PdfByteArr { get; set; }

        public string ErrorMessage { get; set; }

        public string StackTrace { get; set; }
    }
}