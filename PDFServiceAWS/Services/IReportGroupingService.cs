using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.Services
{
    public interface IReportGroupingService
    {
        void InitializeCollections(Func<string, string> transFunc, IEnumerable<PaymentMethodDto> paymentMethods,
            IEnumerable<SolicitorDto> solicitors, IEnumerable<MailingDto> mailings,
            IEnumerable<DepartmentDto> departments, IEnumerable<CategoryDto> categoryTree);

        TransactionGrouped TotalBy(TransactionReportResultDto transDto, FilterTransactionReport filter);

        MatrixDTO TransactionTotalOnlyBy(TransactionReportResultDto transDto, FilterTransactionReport filter);
    }
}
