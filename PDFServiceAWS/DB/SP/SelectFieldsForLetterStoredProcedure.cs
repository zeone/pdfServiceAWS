using System.Data;
using PDFServiceAWS.DB.CustomTables;

namespace PDFServiceAWS.DB.SP
{
    public class SelectFieldsForLetterStoredProcedure : StoredProcedureReturningSelectResultQuery<LetterFieldsDto>
    {
        public SelectFieldsForLetterStoredProcedure(IQueryProvider provider, PerSchemaSqlDbContext dbContext)
            : base(provider, dbContext) { }

        protected override string GetCommandText()
        {
            return "dbo.SelectFieldsForLetter";
        }

        public int TransactionId
        {
            get { return GetParameter<int>("@TransactionId"); }
            set
            {
                SetParameter(paramName: "@TransactionId",
                    paramValue: value,
                    sqlDbType: SqlDbType.Int,
                    isNullable: false,
                    direction: ParameterDirection.Input);
            }
        }

        public bool ClassesOn
        {
            get { return GetParameter<bool>("@ClassesOn"); }
            set
            {
                SetParameter(paramName: "@ClassesOn",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }

        public int?[] SubCategoryIds
        {
            get { return GetParameter<IntArrayDataTable>("@SubCategoryIds").GetItems(); }
            set
            {
                var table = new IntArrayDataTable();
                table.AddItems(value);
                SetParameter(paramName: "@SubCategoryIds",
                    paramValue: table,
                    isNullable: false,
                    sqlDbType: SqlDbType.Structured,
                    direction: ParameterDirection.Input);
            }
        }

        public bool SubCatInclude
        {
            get { return GetParameter<bool>("@SubCatInclude"); }
            set
            {
                SetParameter(paramName: "@SubCatInclude",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }
    }
}