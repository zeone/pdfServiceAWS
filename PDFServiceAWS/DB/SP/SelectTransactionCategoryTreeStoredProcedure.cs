using System.Data;
using PDFServiceAWS.DB.Builder;
using PDFServiceAWS.DB.CustomTables;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Helpers;

namespace PDFServiceAWS.DB.SP
{
    public class SelectTransactionCategoryTreeStoredProcedure : StoredProcedureReturningSelectResultQuery<CategoryDto>
    {
        public SelectTransactionCategoryTreeStoredProcedure(IQueryProvider provider, PerSchemaSqlDbContext dbContext) : base(provider, dbContext)
        {
            SubCategoryIds = ArrayUtils.Empty<int?>();
        }

        protected override string GetCommandText()
        {
            return "dbo.SelectTransactionCategoryTree";
        }

        protected override void ConfigureQuery(CustomQueryConfiguration config)
        {
            base.ConfigureQuery(config);
            config.ModelBuilderType = typeof(TransactionCategoryModelBuilder);
        }

        public int? CategoryId
        {
            get { return GetParameter<int?>("@CategoryID"); }
            set
            {
                SetParameter(paramName: "@CategoryID",
                    paramValue: value,
                    sqlDbType: SqlDbType.Int,
                    isNullable: true,
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
    }
}