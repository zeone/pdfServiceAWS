using System.Data;

namespace PDFServiceAWS.DB.SP
{
    public class SelectDepartmentsStoredProcedure : StoredProcedureReturningSelectResultQuery<DepartmentDto>
    {
        public SelectDepartmentsStoredProcedure(IQueryProvider provider, PerSchemaSqlDbContext dbContext) : base(provider, dbContext) { }

        protected override string GetCommandText()
        {
            return "dbo.SelectDepartments";
        }

        public int? DepartmentId
        {
            get { return GetParameter<int?>("@DepartmentID"); }
            set
            {
                SetParameter(paramName: "@DepartmentID",
                    paramValue: value,
                    sqlDbType: SqlDbType.Int,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        public bool? IsPrimary
        {
            get { return GetParameter<bool?>("@IsPrimary"); }
            set
            {
                SetParameter(paramName: "@IsPrimary",
                    paramValue: value,
                    sqlDbType: SqlDbType.Bit,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }
    }
}