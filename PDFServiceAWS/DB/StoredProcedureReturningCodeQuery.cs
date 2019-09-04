using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using PDFServiceAWS.Helpers;

namespace PDFServiceAWS.DB
{
    public abstract class StoredProcedureReturningCodeQuery : StoredProcedureBase<int>
    {

        // throw error in case this SP returned a non-success code
        private bool _throwErrorOnNonSuccessCode;

        protected StoredProcedureReturningCodeQuery(IQueryProvider provider,
                                       PerSchemaSqlDbContext dbContext)
            : base(provider, dbContext)
        {



            // almost all CMS stored procedures have this 
            // OUTPUT parameter to hold error message
            SetParameter(paramName: "@ErrorMessage",
                          paramValue: default(string),
                          sqlDbType: SqlDbType.VarChar,
                          isNullable: false,
                          direction: ParameterDirection.Output,
                          size: 500);

            // default behavior is to throw error
            // when the stored procedure returns a non-success code
            ThrowErrorOnNonSuccessCode = true;
        }




        /// <summary>
        /// Throw exception of type SpResultException
        /// in case the stored procedure returned a non-success code
        /// </summary>
        public bool ThrowErrorOnNonSuccessCode
        {
            get
            {
                return _throwErrorOnNonSuccessCode;
            }
            set
            {
                _throwErrorOnNonSuccessCode = value;
            }
        }



        protected sealed override object ProcessDbCommand(SqlCommand dbCommand)
        {
            // execute the stored procedure and return the response code
            SpExecResult execResult = new SpExecResult();

            // execute the stored procedure
            try
            {

                // set the name of just executed stored procedure
                execResult.StoredProcedureName = DbContext.GetFullStoredProcName(GetCommandText());

                // execute the stored procedure
                dbCommand.ExecuteNonQuery();

                // get the return code from the @ReturnVal parameter
                execResult.ReturnCode = OutReturnCode;

                // set the error message from @ErrorMessage parameter.
                // If no error occured, then return NULL
                execResult.ErrorMessage = ErrorMessage(dbCommand);

                if ((execResult.ReturnCode != SpExecResult.NoErrorsCode) && ThrowErrorOnNonSuccessCode)
                {
                    // throw error here in case the stored procedure 
                    // returned a non-success code
                    throw new SpResultException(execResult);
                }
            }
            catch (SqlException sqlEx)
            {
                throw;
                // todo: log error here
            }
            catch (Exception e)
            {
                throw;
            }

            // return (-1) to denote that it's impossible to 
            // return the actual return code since the @ReturnVal parameter 
            // was not provided
            return execResult;
        }


        protected override async Task<object> ProcessDbCommandAsync(SqlCommand dbCommand)
        {
            // execute the stored procedure and return the response code
            SpExecResult execResult = new SpExecResult();

            // execute the stored procedure
            try
            {

                // set the name of just executed stored procedure
                execResult.StoredProcedureName = DbContext.GetFullStoredProcName(GetCommandText());

                // execute the stored procedure
                await dbCommand.ExecuteNonQueryAsync();

                // get the return code from the @ReturnVal parameter
                execResult.ReturnCode = OutReturnCode;

                // set the error message from @ErrorMessage parameter.
                // If no error occured, then return NULL
                execResult.ErrorMessage = ErrorMessage(dbCommand);

                if ((execResult.ReturnCode != SpExecResult.NoErrorsCode) && ThrowErrorOnNonSuccessCode)
                {
                    // throw error here in case the stored procedure 
                    // returned a non-success code
                    throw new SpResultException(execResult);
                }
            }
            catch (SqlException sqlEx)
            {
                throw;
                // todo: log error here
            }
            catch (Exception e)
            {
                throw;
            }

            // return (-1) to denote that it's impossible to 
            // return the actual return code since the @ReturnVal parameter 
            // was not provided
            return execResult;
        }

        /// <summary>
        /// Error message set in case the stored procedure executed with errors
        /// </summary>
        public string OutErrorMessage
        {
            get
            {
                return GetParameter<string>("@ErrorMessage");
            }
        }


        private string ErrorMessage(SqlCommand dbCommand)
        {
            if (dbCommand == null)
                throw new ArgumentNullException("dbCommand", "SQL DB command is not defined!");

            const string errorParamName = "@ErrorMessage";

            if (!dbCommand.Parameters.Contains(errorParamName))
            {
                // @ErrorMessage parameter was not found 
                // in the parameters collection
                return ReflectionUtils.Null<string>();
            }


            SqlParameter errorParam = dbCommand.Parameters[errorParamName];

            if ((errorParam.Value != null) && !DBNull.Value.Equals(errorParam.Value))
            {
                // if the @ErrorMessage parameter actually contains 
                // message and its value is defined
                return (string)errorParam.Value;
            }

            // no error occured, so the error message should be NULL
            return ReflectionUtils.Null<string>();
        }


        protected override string GetCommandText()
        {
            throw new NotImplementedException();
        }
    }
}