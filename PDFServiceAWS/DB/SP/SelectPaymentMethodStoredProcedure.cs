using System.Data;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.DB.SP
{
    public class SelectPaymentMethodStoredProcedure : StoredProcedureReturningSelectResultQuery<PaymentMethodDto>
    {
        public SelectPaymentMethodStoredProcedure(IQueryProvider provider, PerSchemaSqlDbContext dbContext) : base(provider, dbContext) { }

        protected override string GetCommandText()
        {
            return "dbo.SelectPaymentMethods";
        }

        public int? PaymentMethodID
        {
            get { return GetParameter<int?>("@PaymentMethodID"); }
            set
            {
                SetParameter(paramName: "@PaymentMethodID",
                    paramValue: value,
                    sqlDbType: SqlDbType.Int,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }
    }
}