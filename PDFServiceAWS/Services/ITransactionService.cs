using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.Services
{
    public interface ITransactionService
    {
        PaymentDto GetAnyPayment();

        PaymentDto GetPaymentById(int id);
    }
}
