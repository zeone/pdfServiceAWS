using System.Collections.Generic;

namespace PDFServiceAWS.Dto
{
    public class TransactioMatrixSubDetails
    {
        public string Name { get; set; }
        public List<decimal> Amounts { get; set; }
    }
}