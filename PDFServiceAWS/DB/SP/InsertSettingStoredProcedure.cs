using System.Data;

namespace PDFServiceAWS.DB.SP
{
    public class InsertSettingStoredProcedure : StoredProcedureReturningCodeQuery
    {
        public InsertSettingStoredProcedure(IQueryProvider provider, PerSchemaSqlDbContext dbContext) : base(provider, dbContext)
        {
            SetParameter
                (
                paramName: "@SettingID",
                paramValue: default(short),
                sqlDbType: SqlDbType.SmallInt,
                isNullable: false,
                direction: ParameterDirection.Output
                );

            ParentID = default(short);
        }

        protected override string GetCommandText()
        {
            // name of stored procedure to execute
            return "dbo.InsertSettings";
        }

        public string Setting
        {
            get
            {
                return GetParameter<string>("@Setting");
            }
            set
            {
                SetParameter
                    (
                    paramName: "@Setting",
                    paramValue: value,
                    sqlDbType: SqlDbType.VarChar,
                    isNullable: false,
                    direction: ParameterDirection.Input,
                    size: 50
                    );
            }
        }
        public string Value
        {
            get
            {
                return GetParameter<string>("@Value");
            }
            set
            {
                SetParameter
                    (
                    paramName: "@Value",
                    paramValue: value,
                    sqlDbType: SqlDbType.NVarChar,
                    isNullable: false,
                    direction: ParameterDirection.Input,
                    size: 500
                    );
            }
        }

        public short? ParentID
        {
            get
            {
                return GetParameter<short>("@ParentID");
            }
            set
            {
                SetParameter
                    (
                    paramName: "@ParentID",
                    paramValue: value,
                    sqlDbType: SqlDbType.SmallInt,
                    isNullable: true,
                    direction: ParameterDirection.Input
                    );
            }
        }

        public short SettingID
        {
            get
            {
                return GetParameter<short>("@SettingID");
            }
            set
            {
                SetParameter
                    (
                    paramName: "@SettingID",
                    paramValue: value,
                    sqlDbType: SqlDbType.SmallInt,
                    isNullable: true,
                    direction: ParameterDirection.Input
                    );
            }
        }

    }
}