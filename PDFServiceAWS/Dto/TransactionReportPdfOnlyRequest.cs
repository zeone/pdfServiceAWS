using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PDFServiceAWS.Dto
{
    
    public class TransactionReportPdfOnlyRequest : BaseFilterRequest
    {
         
        public TransactionGrouped Grouped { get; set; }

         
        public int TransactionCount { get; set; }

        #region Collections
         
        public IEnumerable<PaymentMethodDto> PaymentMethods { get; set; }

         
        public IEnumerable<SolicitorDto> Solicitors { get; set; }

         
        public IEnumerable<MailingDto> Mailings { get; set; }

         
        public IEnumerable<DepartmentDto> Departments { get; set; }

         
        public IEnumerable<CategoryDto> CategoryTree { get; set; }


        #endregion
    }
}