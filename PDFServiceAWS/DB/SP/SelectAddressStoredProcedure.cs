using System.Data;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.DB.SP
{
    public class SelectAddressStoredProcedure : StoredProcedureReturningSelectResultQuery<FamilyAddressDto>
    {
        public SelectAddressStoredProcedure(IQueryProvider provider, PerSchemaSqlDbContext dbContext) : base(provider, dbContext)
        {
        }

        protected override string GetCommandText()
        {
            return "dbo.SelectAddress";
        }

        public int FamilyId
        {
            get { return GetParameter<int>("@FamilyId"); }
            set
            {
                SetParameter(paramName: "@FamilyId",
                    paramValue: value,
                    sqlDbType: SqlDbType.Int,
                    isNullable: false,
                    direction: ParameterDirection.Input);
            }
        }
    }
}