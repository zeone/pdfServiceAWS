using System.Data;

namespace PDFServiceAWS.DB.SP
{
    public class SelectSettingsStoredProcedure : StoredProcedureReturningSelectResultQuery<AppSettingRecord>
    {
        public SelectSettingsStoredProcedure(IQueryProvider provider, PerSchemaSqlDbContext dbContext) : base(provider, dbContext)
        {

        }

        protected override string GetCommandText()
        {
            return "dbo.SelectSettings";
        }

        public int? SettingID
        {
            get { return GetParameter<short>("@SettingID"); }
            set
            {
                SetParameter(paramName: "@SettingID",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.SmallInt,
                    direction: ParameterDirection.Input);
            }
        }

        public string Setting
        {
            get { return GetParameter<string>("@Setting"); }
            set
            {
                SetParameter(paramName: "@Setting",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.VarChar,
                    direction: ParameterDirection.Input, size: 50);
            }
        }

    }
}