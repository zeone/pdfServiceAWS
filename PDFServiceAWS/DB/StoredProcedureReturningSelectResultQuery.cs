using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using PDFServiceAWS.DB.Builder;

namespace PDFServiceAWS.DB
{
    public abstract class StoredProcedureReturningSelectResultQuery<TResult> : StoredProcedureBase<TResult> where TResult : new()
    {

        protected StoredProcedureReturningSelectResultQuery(IQueryProvider provider, PerSchemaSqlDbContext dbContext) :
            base(provider, dbContext)
        {
        }


        protected override object ProcessDbCommand(SqlCommand dbCommand)
        {
            SpExecResult result = new SpExecResult();

            SqlDataReader reader = null;
            try
            {

                // set stored procedure name
                result.StoredProcedureName = DbContext.GetFullStoredProcName(GetCommandText());

                // execute the stored procedure
                reader = dbCommand.ExecuteReader();
                if (!reader.HasRows)
                {
                    // if the stored procedure did not
                    // return any row
                    reader.Close();
                    result.ReturnCode = OutReturnCode;
                    return result;
                }

                IModelBuilder<TResult> builder = CreateBuilder();

                while (reader.Read())
                {
                    TResult newModel = builder.Build(reader);
                    result.AppendRow(newModel);
                }

                // close the reader
                reader.Close();

                // set return code
                result.ReturnCode = OutReturnCode;
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if ((reader != null) && !reader.IsClosed)
                {
                    // close the data reader if it was not closed
                    reader.Close();
                }
            }
        }

        protected override async Task<object> ProcessDbCommandAsync(SqlCommand dbCommand)
        {
            SpExecResult result = new SpExecResult();

            SqlDataReader reader = null;
            try
            {

                // set stored procedure name
                result.StoredProcedureName = DbContext.GetFullStoredProcName(GetCommandText());

                // execute the stored procedure
                reader = await dbCommand.ExecuteReaderAsync();
                if (!reader.HasRows)
                {
                    // if the stored procedure did not
                    // return any row
                    reader.Close();
                    result.ReturnCode = OutReturnCode;
                    return result;
                }

                IModelBuilder<TResult> builder = CreateBuilder();

                while (await reader.ReadAsync())
                {
                    TResult newModel = builder.Build(reader);
                    result.AppendRow(newModel);
                }

                // close the reader
                reader.Close();

                // set return code
                result.ReturnCode = OutReturnCode;
                return result;
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                if ((reader != null) && !reader.IsClosed)
                {
                    // close the data reader if it was not closed
                    reader.Close();
                }
            }
        }
    }
}