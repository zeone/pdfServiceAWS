using System.Data;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.DB.SP
{
    public class SelectSolicitorsStoredProcedure : StoredProcedureReturningSelectResultQuery<SolicitorDto>
    {
        public SelectSolicitorsStoredProcedure(IQueryProvider provider, PerSchemaSqlDbContext dbContext) : base(provider, dbContext) { }

        protected override string GetCommandText()
        {
            return "dbo.SelectSolicitors";
        }

        public int? SolicitorID
        {
            get { return GetParameter<int?>("@SolicitorID"); }
            set
            {
                SetParameter(paramName: "@SolicitorID",
                    paramValue: value,
                    sqlDbType: SqlDbType.Int,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }
    }
}