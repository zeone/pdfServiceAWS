using System;
using System.Data;
using PDFServiceAWS.DB.CustomTables;

namespace PDFServiceAWS.DB.SP
{
    public class SelectTransactionsReportStoredProcedure : StoredProcedureReturningSelectResultQuery<TransactionsReportDto>
    {
        public SelectTransactionsReportStoredProcedure(IQueryProvider provider, PerSchemaSqlDbContext dbContext) : base(provider, dbContext)
        {
            CommandTimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CustomSqlTimeout"]);
        }

        protected override string GetCommandText()
        {
            return "dbo.SelectTransactionReport";
        }

        // 0 - bill 1 - payment, 2 - payment/bills
        public int ReportType
        {
            get { return GetParameter<int>("@ReportType"); }
            set
            {
                SetParameter(paramName: "@ReportType",
                    paramValue: value,
                    isNullable: false,
                    sqlDbType: SqlDbType.Int,
                    direction: ParameterDirection.Input);
            }
        }

        public int?[] TransSubCatIDs
        {
            get { return GetParameter<IntArrayDataTable>("@TransSubCatIDs").GetItems(); }
            set
            {
                var table = new IntArrayDataTable();
                table.AddItems(value);
                SetParameter(paramName: "@TransSubCatIDs",
                    paramValue: table,
                    isNullable: false,
                    sqlDbType: SqlDbType.Structured,
                    direction: ParameterDirection.Input);
            }
        }
        public bool? ExByTransSubCat
        {
            get { return GetParameter<bool?>("@ExByTransSubCat"); }
            set
            {
                SetParameter(paramName: "@ExByTransSubCat",
                    paramValue: value ?? false,
                    isNullable: false,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }

        public DateTime? TransactionStartDate
        {
            get { return GetParameter<DateTime?>("@TransactionStartDate"); }
            set
            {
                SetParameter(paramName: "@TransactionStartDate",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.SmallDateTime,
                    direction: ParameterDirection.Input);
            }
        }

        public DateTime? TransactionEndDate
        {
            get { return GetParameter<DateTime?>("@TransactionEndDate"); }
            set
            {
                SetParameter(paramName: "@TransactionEndDate",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.SmallDateTime,
                    direction: ParameterDirection.Input);
            }
        }

        public DateTime? DateDueFrom
        {
            get { return GetParameter<DateTime?>("@DateDueFrom"); }
            set
            {
                SetParameter(paramName: "@DateDueFrom",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.SmallDateTime,
                    direction: ParameterDirection.Input);
            }
        }

        public DateTime? DateDueTo
        {
            get { return GetParameter<DateTime?>("@DateDueTo"); }
            set
            {
                SetParameter(paramName: "@DateDueTo",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.SmallDateTime,
                    direction: ParameterDirection.Input);
            }
        }


        public decimal? MinAmount
        {
            get { return GetParameter<decimal?>("@MinAmount"); }
            set
            {
                SetParameter(paramName: "@MinAmount",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.Money,
                    direction: ParameterDirection.Input);
            }
        }

        public int? DueBilled
        {
            get { return GetParameter<int?>("@DueBilled"); }
            set
            {
                SetParameter(paramName: "@DueBilled",
                    paramValue: value ?? 0,
                    isNullable: false,
                    sqlDbType: SqlDbType.Int,
                    direction: ParameterDirection.Input);
            }
        }

        //0 - individual amount 1 - total amount
        public bool? CalcTotalSum
        {
            get { return GetParameter<bool?>("@CalcTotalSum"); }
            set
            {
                SetParameter(paramName: "@CalcTotalSum",
                    paramValue: value ?? false,
                    isNullable: false,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }
        //0 - All 1 - Paid 2- outstanding
        public int? PaidStatus
        {
            get { return GetParameter<int?>("@PaidStatus"); }
            set
            {
                SetParameter(paramName: "@PaidStatus",
                    paramValue: value,
                    isNullable: false,
                    sqlDbType: SqlDbType.Int,
                    direction: ParameterDirection.Input);
            }
        }


        public int?[] MethodTypes
        {
            get { return GetParameter<IntArrayDataTable>("@MethodTypes").GetItems(); }
            set
            {
                var table = new IntArrayDataTable();
                table.AddItems(value);
                SetParameter(paramName: "@MethodTypes",
                    paramValue: table,
                    isNullable: false,
                    sqlDbType: SqlDbType.Structured,
                    direction: ParameterDirection.Input);
            }
        }

        public int?[] SolicitorIDs
        {
            get { return GetParameter<IntArrayDataTable>("@SolicitorIDs").GetItems(); }
            set
            {
                var table = new IntArrayDataTable();
                table.AddItems(value);
                SetParameter(paramName: "@SolicitorIDs",
                    paramValue: table,
                    isNullable: false,
                    sqlDbType: SqlDbType.Structured,
                    direction: ParameterDirection.Input);
            }
        }

        public bool? ExSolicitors
        {
            get { return GetParameter<bool?>("@ExSolicitors"); }
            set
            {
                SetParameter(paramName: "@ExSolicitors",
                    paramValue: value ?? false,
                    isNullable: false,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }

        public int?[] DepartmentIDs
        {
            get { return GetParameter<IntArrayDataTable>("@DepartmentIDs").GetItems(); }
            set
            {
                var table = new IntArrayDataTable();
                table.AddItems(value);
                SetParameter(paramName: "@DepartmentIDs",
                    paramValue: table,
                    isNullable: false,
                    sqlDbType: SqlDbType.Structured,
                    direction: ParameterDirection.Input);
            }
        }

        public bool? ExByDepartment
        {
            get { return GetParameter<bool?>("@ExByDepartment"); }
            set
            {
                SetParameter(paramName: "@ExByDepartment",
                    paramValue: value ?? false,
                    isNullable: false,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }

        public int?[] ExFamilyIds
        {
            get { return GetParameter<IntArrayDataTable>("@ExFamilyIds").GetItems(); }
            set
            {
                var table = new IntArrayDataTable();
                table.AddItems(value);
                SetParameter(paramName: "@ExFamilyIds",
                    paramValue: table,
                    isNullable: false,
                    sqlDbType: SqlDbType.Structured,
                    direction: ParameterDirection.Input);
            }
        }

        public int?[] FamilyIds
        {
            get { return GetParameter<IntArrayDataTable>("@FamilyIds").GetItems(); }
            set
            {
                var table = new IntArrayDataTable();
                table.AddItems(value);
                SetParameter(paramName: "@FamilyIds",
                    paramValue: table,
                    isNullable: false,
                    sqlDbType: SqlDbType.Structured,
                    direction: ParameterDirection.Input);
            }
        }

        public int? CountRows
        {
            get { return GetParameter<int?>("@CountRows"); }
            set
            {
                SetParameter(paramName: "@CountRows",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.Int,
                    direction: ParameterDirection.Input);
            }
        }

        public int? ReceiptNumberMin
        {
            get { return GetParameter<int?>("@ReceiptNumberMin"); }
            set
            {
                SetParameter(paramName: "@ReceiptNumberMin",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.Int,
                    direction: ParameterDirection.Input);
            }
        }

        public int? ReceiptNumberMax
        {
            get { return GetParameter<int?>("@ReceiptNumberMax"); }
            set
            {
                SetParameter(paramName: "@ReceiptNumberMax",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.Int,
                    direction: ParameterDirection.Input);
            }
        }

        public bool? FamilyOtherCriteria
        {
            get { return GetParameter<bool?>("@FamilyOtherCriteria"); }
            set
            {
                SetParameter(paramName: "@FamilyOtherCriteria",
                    paramValue: value ?? false,
                    isNullable: false,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }

        public bool? ReturnOnlyTransactionIds
        {
            get { return GetParameter<bool?>("@ReturnOnlyTransactionIds"); }
            set
            {
                SetParameter(paramName: "@ReturnOnlyTransactionIds",
                    paramValue: value ?? false,
                    isNullable: false,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }


        //Address 
        public string Country
        {
            get { return GetParameter<string>("@Country"); }
            set
            {
                SetParameter(paramName: "@Country",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.NVarChar,
                    direction: ParameterDirection.Input);
            }
        }
        public bool? ExCountry
        {
            get { return GetParameter<bool?>("@ExCountry"); }
            set
            {
                SetParameter(paramName: "@ExCountry",
                    paramValue: value ?? false,
                    isNullable: true,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }
        public string City
        {
            get { return GetParameter<string>("@City"); }
            set
            {
                SetParameter(paramName: "@City",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.NVarChar,
                    direction: ParameterDirection.Input);
            }
        }
        public bool? ExCity
        {
            get { return GetParameter<bool?>("@ExCity"); }
            set
            {
                SetParameter(paramName: "@ExCity",
                    paramValue: value ?? false,
                    isNullable: true,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }
        public string State
        {
            get { return GetParameter<string>("@State"); }
            set
            {
                SetParameter(paramName: "@State",
                    paramValue: value,
                    isNullable: true,
                    sqlDbType: SqlDbType.NVarChar,
                    direction: ParameterDirection.Input);
            }
        }
        public bool? ExState
        {
            get { return GetParameter<bool?>("@ExState"); }
            set
            {
                SetParameter(paramName: "@ExState",
                    paramValue: value ?? false,
                    isNullable: true,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }
        public string[] Zip
        {
            get { return GetParameter<StringArrayDataTable>("@Zip").GetItems(); }
            set
            {
                var table = new StringArrayDataTable();
                table.AddItems(value);
                SetParameter(paramName: "@Zip",
                    paramValue: table,
                    isNullable: false,
                    sqlDbType: SqlDbType.Structured,
                    direction: ParameterDirection.Input);
            }
        }
        public bool? ExZip
        {
            get { return GetParameter<bool?>("@ExZip"); }
            set
            {
                SetParameter(paramName: "@ExZip",
                    paramValue: value ?? false,
                    isNullable: true,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }
        // false - AND, true - OR
        public bool? AddressAndOr
        {
            get { return GetParameter<bool?>("@AddressAndOr"); }
            set
            {
                SetParameter(paramName: "@AddressAndOr",
                    paramValue: value ?? false,
                    isNullable: true,
                    sqlDbType: SqlDbType.Bit,
                    direction: ParameterDirection.Input);
            }
        }
    }
}