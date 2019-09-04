using System;
using System.Data;
using PDFServiceAWS.DB.Builder;

namespace PDFServiceAWS.DB.SP
{
    public class SelectPaymentsStoredProcedure : StoredProcedureReturningSelectResultQuery<PaymentDto>
    {
        public SelectPaymentsStoredProcedure(IQueryProvider provider, PerSchemaSqlDbContext dbContext) : base(provider, dbContext) { }

        protected override string GetCommandText()
        {
            return "dbo.SelectPayments";
        }

        protected override void ConfigureQuery(CustomQueryConfiguration config)
        {
            base.ConfigureQuery(config);
            config.ModelBuilderType = typeof(TransactionPaymentModelBuilder);
        }

        public int? FamilyId
        {
            get { return GetParameter<int?>("@FamilyID"); }
            set
            {
                SetParameter(paramName: "@FamilyID",
                    paramValue: value,
                    sqlDbType: SqlDbType.Int,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        public int? TransactionId
        {
            get { return GetParameter<int?>("@TransactionID"); }
            set
            {
                SetParameter(paramName: "@TransactionID",
                    paramValue: value,
                    sqlDbType: SqlDbType.Int,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        public bool? IsReceipt
        {
            get { return GetParameter<bool?>("@IsReceipt"); }
            set
            {
                SetParameter(paramName: "@IsReceipt",
                    paramValue: value,
                    sqlDbType: SqlDbType.Bit,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        public DateTime? DateMin
        {
            get { return GetParameter<DateTime?>("@DateMin"); }
            set
            {
                SetParameter(paramName: "@DateMin",
                    paramValue: value,
                    sqlDbType: SqlDbType.DateTime,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        public DateTime? DateMax
        {
            get { return GetParameter<DateTime?>("@DateMax"); }
            set
            {
                SetParameter(paramName: "@DateMax",
                    paramValue: value,
                    sqlDbType: SqlDbType.DateTime,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        public decimal? AmountMin
        {
            get { return GetParameter<decimal?>("@AmountMin"); }
            set
            {
                SetParameter(paramName: "@AmountMin",
                    paramValue: value,
                    sqlDbType: SqlDbType.Decimal,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        public decimal? AmountMax
        {
            get { return GetParameter<decimal?>("@AmountMax"); }
            set
            {
                SetParameter(paramName: "@AmountMax",
                    paramValue: value,
                    sqlDbType: SqlDbType.Decimal,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        public int? CategoryID
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

        public int? DepartmentID
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

        public bool? ReceiptSent
        {
            get { return GetParameter<bool?>("@ReceiptSent"); }
            set
            {
                SetParameter(paramName: "@ReceiptSent",
                    paramValue: value,
                    sqlDbType: SqlDbType.Bit,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        public int? SubcategoryID
        {
            get { return GetParameter<int?>("@SubcategoryID"); }
            set
            {
                SetParameter(paramName: "@SubcategoryID",
                    paramValue: value,
                    sqlDbType: SqlDbType.Int,
                    isNullable: true,
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
                    sqlDbType: SqlDbType.Int,
                    isNullable: true,
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
                    sqlDbType: SqlDbType.Int,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        public string SearchFamilyName
        {
            get { return GetParameter<string>("@SearchFamilyName"); }
            set
            {
                SetParameter(paramName: "@SearchFamilyName",
                    paramValue: value,
                    sqlDbType: SqlDbType.NVarChar,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        public bool? PreviewOnly
        {
            get { return GetParameter<bool?>("@PreviewOnly"); }
            set
            {
                SetParameter(paramName: "@PreviewOnly",
                    paramValue: value,
                    sqlDbType: SqlDbType.Bit,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }
        public bool? FirstOnly
        {
            get { return GetParameter<bool?>("@FirstOnly"); }
            set
            {
                SetParameter(paramName: "@FirstOnly",
                    paramValue: value,
                    sqlDbType: SqlDbType.Bit,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }


        public bool? UsePagination
        {
            get { return GetParameter<bool?>("@UsePagination"); }
            set
            {
                SetParameter(paramName: "@UsePagination",
                    paramValue: value,
                    sqlDbType: SqlDbType.Bit,
                    isNullable: true,
                    direction: ParameterDirection.Input);
            }
        }

        /// <summary>
        /// start position for pagination
        /// </summary>
        public int Offset
        {
            get { return GetParameter<int>("@Offset"); }
            set
            {
                SetParameter(paramName: "@Offset",
                    paramValue: value,
                    sqlDbType: SqlDbType.Int,
                    isNullable: false,
                    direction: ParameterDirection.Input);
            }
        }

        /// <summary>
        /// count rows on the page
        /// </summary>
        public int PageSize
        {
            get { return GetParameter<int>("@PageSize"); }
            set
            {
                SetParameter(paramName: "@PageSize",
                    paramValue: value,
                    sqlDbType: SqlDbType.Int,
                    isNullable: false,
                    direction: ParameterDirection.Input);
            }
        }

        /// <summary>
        /// Date, Category, Class, Amount, PaymentMethod, ReceiptNo, ReceiptSent
        /// </summary>
        public string OrderBy
        {
            get { return GetParameter<string>("@OrderBy"); }
            set
            {
                SetParameter(paramName: "@OrderBy",
                    paramValue: value,
                    sqlDbType: SqlDbType.NVarChar,
                    isNullable: false,
                    direction: ParameterDirection.Input);
            }
        }

        /// <summary>
        /// false - ascending, true - descending
        /// </summary>
        public bool AscDesc
        {
            get { return GetParameter<bool>("@AscDesc"); }
            set
            {
                SetParameter(paramName: "@AscDesc",
                    paramValue: value,
                    sqlDbType: SqlDbType.Bit,
                    isNullable: false,
                    direction: ParameterDirection.Input);
            }
        }
    }
}