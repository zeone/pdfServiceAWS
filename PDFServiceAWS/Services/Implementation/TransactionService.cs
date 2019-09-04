using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PDFServiceAWS.DB.SP;
using PDFServiceAWS.Helpers;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public class TransactionService : BaseService, ITransactionService
    {
        private string _schema;
       public TransactionService( IQueryProvider queryProvider, string schema) : base( queryProvider)
       {
           _schema = schema;
       }

        public PaymentDto GetAnyPayment()
        {
            var selectPaymentsSp = QueryProvider.CreateQuery<SelectPaymentsStoredProcedure>(_schema);
            selectPaymentsSp.FirstOnly = true;
            var result = selectPaymentsSp.Execute();
            return result.NonZeroReturnCode || result.HasNoDataRows
                ? new PaymentDto()
                : result.ResultToArray<PaymentDto>().FirstOrDefault();
        }

        public PaymentDto GetPaymentById(int id)
        {
            return GetAllPayments(transactionId: id).FirstOrDefault();
        }

        /// <summary>
        /// Return list of payments by criteria
        /// </summary>
        /// <param name="familyId"></param>
        /// <param name="transactionId"></param>
        /// <param name="isReceipt"></param>
        /// <param name="dateMin"></param>
        /// <param name="dateMax"></param>
        /// <param name="amountMin"></param>
        /// <param name="amountMax"></param>
        /// <param name="categoryId"></param>
        /// <param name="solicitorId"></param>
        /// <param name="departmentId"></param>
        /// <param name="receiptSent"></param>
        /// <param name="subcategoryId"></param>
        /// <param name="receiptNumberMin"></param>
        /// <param name="receiptNumberMax"></param>
        /// <param name="isPreview"></param>
        /// <param name="usePagination"></param>
        /// <param name="offset">start position for pagination</param>
        /// <param name="pageSize">count rows on the page</param>
        /// <param name="orderBy">Date, Category, Class, Amount, PaymentMethod, ReceiptNo, ReceiptSent</param>
        /// <param name="ascDesc">false - ascending, true - descending</param>
        /// <param name="searchFamName">Family name</param>
        /// <returns></returns>

        public IEnumerable<PaymentDto> GetAllPayments(int? familyId = null, int? transactionId = null, bool? isReceipt = null, DateTime? dateMin = null, DateTime?
            dateMax = null, decimal? amountMin = null, decimal? amountMax = null, int? categoryId = null, int? solicitorId = null, int? departmentId = null, bool?
            receiptSent = null, int? subcategoryId = null, int? receiptNumberMin = null, int? receiptNumberMax = null, bool? isPreview = false,
          bool? usePagination = false, int offset = 1, int pageSize = 10, string orderBy = "Date", bool ascDesc = false, string searchFamName = null)
        {
            var selectPaymentsSp = QueryProvider.CreateQuery<SelectPaymentsStoredProcedure>(_schema);
            selectPaymentsSp.FamilyId = familyId;
            selectPaymentsSp.TransactionId = transactionId;
            selectPaymentsSp.IsReceipt = isReceipt;
            selectPaymentsSp.DateMin = dateMin;
            selectPaymentsSp.DateMax = dateMax;
            selectPaymentsSp.AmountMin = amountMin;
            selectPaymentsSp.AmountMax = amountMax;
            selectPaymentsSp.CategoryID = categoryId;
            selectPaymentsSp.SolicitorID = solicitorId;
            selectPaymentsSp.DepartmentID = departmentId;
            selectPaymentsSp.ReceiptSent = receiptSent;
            selectPaymentsSp.SubcategoryID = subcategoryId;
            selectPaymentsSp.ReceiptNumberMin = receiptNumberMin;
            selectPaymentsSp.ReceiptNumberMax = receiptNumberMax;
            selectPaymentsSp.PreviewOnly = isPreview;
            selectPaymentsSp.UsePagination = usePagination;
            selectPaymentsSp.Offset = offset;
            selectPaymentsSp.PageSize = pageSize;
            selectPaymentsSp.OrderBy = orderBy;
            selectPaymentsSp.AscDesc = ascDesc;
            selectPaymentsSp.SearchFamilyName = searchFamName;
            var result = selectPaymentsSp.Execute();
            return result.DataRows != null ? result.ResultToArray<PaymentDto>() : ArrayUtils.Empty<PaymentDto>();
        }
    }
}