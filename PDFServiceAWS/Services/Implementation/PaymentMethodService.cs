using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PDFServiceAWS.Enums;
using PDFServiceAWS.DB.SP;
using PDFServiceAWS.Dto;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public class PaymentMethodService : BaseService, IPaymentMethodService
    {
        private string _schema;

        public PaymentMethodService(IQueryProvider queryProvider, string schema) : base(queryProvider)
        {
            _schema = schema;
        }

       public IEnumerable<PaymentMethodDto> GetAllPaymentMethods()
        {
            return GetPaymentMethods(null);
        }

        private IEnumerable<PaymentMethodDto> GetPaymentMethods(int? paymentMethodId)
        {
            var selectPaymentMethodsSp = QueryProvider.CreateQuery<SelectPaymentMethodStoredProcedure>(_schema);
            selectPaymentMethodsSp.PaymentMethodID = paymentMethodId;
            var result = selectPaymentMethodsSp.Execute();
            var res = result.ResultToArray<PaymentMethodDto>();
            return result.DataRows != null ? result.ResultToArray<PaymentMethodDto>() : null;
        }

       
    }
}