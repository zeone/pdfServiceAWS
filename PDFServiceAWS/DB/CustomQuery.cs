using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PDFServiceAWS.Helpers;

namespace PDFServiceAWS.DB
{
    public abstract class CustomQuery : BaseQuery
    {
        // parameters map that hold a collection of parameters needed to run the query
        protected readonly Hashtable ParametersMap = new Hashtable();
        protected readonly CustomQueryConfiguration Config = new CustomQueryConfiguration();

        public int CommandTimeout { get; set; } = 30;

        protected CustomQuery(IQueryProvider provider,
                              PerSchemaSqlDbContext dbContext)
            : base(provider, dbContext)
        {

        }

        protected virtual void ConfigureQuery(CustomQueryConfiguration config)
        {
            // default behavior is to handle parameters 
            // by inserting them into SqlCommand objects
            config.UseDefaultParameterHandling = true;
        }

        #region === ABSTRACT PROPERTIES & METHODS ===

        protected abstract object ProcessDbCommand(SqlCommand dbCommand);

        protected abstract Task<object> ProcessDbCommandAsync(SqlCommand dbCommand);
        /// <summary>
        /// Get a string represenation of a command text (name of a stored procedure, text of an SQL query)
        /// </summary>
        protected abstract string GetCommandText(
            /*
             *  SELECT fm.*
             *  FROM FamiliesMembers fm
             *  WHERE (fm.MemberId = @MemberId) && (fm.FamilyId = @FamilyId)
             */
            );

        /// <summary>
        /// Get the type of current command/query: StoredProcedure/Text/TableDirect
        /// </summary>
        public abstract CommandType CurrentCommandType { get; }

        #endregion



        private string UnParameterizeSqlQuery(string sql)
        {
            foreach (var parameterKey in ParametersMap.Keys)
            {
                // take all entries
                QueryParameterEntry entry = (QueryParameterEntry)ParametersMap[parameterKey];

                // in case the parameter is set with NoQuotes
                bool noQuotes = (entry.Parameter.Value is string) && entry.Config.NoQuotes;

                // 
                sql = sql.Replace(entry.Parameter.ParameterName, ValueToString(entry.Parameter.Value, noQuotes));
            }

            return sql;
        }

        private string ValueToString(object parameterValue, bool noQuotes = false)
        {
            if (DBNull.Value.Equals(parameterValue) || (parameterValue == null))
            {
                // return the NULL literal for parameters with no value  
                return ("NULL");
            }

            if (IsQuotedType(parameterValue) && !noQuotes)
            {
                // enclose the value with quote symbols 
                return string.Format("'{0}'", parameterValue);
            }

            return parameterValue.ToString();
        }

        /// <summary>
        /// Method needed to re-create parameters on each new execution of 
        /// the query cause 
        /// </summary>
        private void ResetParameters()
        {
            foreach (var entryKey in ParametersMap.Keys)
            {
                QueryParameterEntry entry = (QueryParameterEntry)ParametersMap[entryKey];
                // create a new SQL parameter
                SqlParameter newDbParameter = CreateParameterFromEntry(entry);
                // set the new parameter
                entry.Parameter = newDbParameter;
            }
        }

        private SqlParameter CreateParameterFromEntry(QueryParameterEntry entry)
        {

            SqlParameter newDbParameter = DbContext.CreateParameter();

            newDbParameter.ParameterName = entry.Config.ParameterName;
            newDbParameter.SqlDbType = entry.Config.DbType;
            newDbParameter.IsNullable = entry.Config.IsNullable;
            newDbParameter.Size = entry.Config.Size;
            newDbParameter.Direction = entry.Config.Direction;
            newDbParameter.Value = entry.Parameter.Value;

            return newDbParameter;
        }

        /// <summary>
        /// Quoted types are those that need additional quote symbols around them 
        /// set [Name] = 'Jim Anderson',
        /// set [Id] = '3FA6861B-4D8D-48F8-ABF7-0764B23C256F',
        /// set [StartDate] = '09/03/1972 12:31:45'
        /// </summary>
        /// <param name="paramValue">Parameter value</param>
        /// <returns></returns>
        private bool IsQuotedType(object paramValue)
        {
            return (paramValue is string) ||
                   (paramValue is char) ||
                   (paramValue is Guid) ||
                   (paramValue is DateTime);
        }

        /// <summary>
        /// Appends the Schema [USE] statement
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        private string AppendSchemaUse(string commandText)
        {

            // is the SQL Select statement;
            string newSqlCommandText = string.Format("USE {0}; ", Schema);
            newSqlCommandText += commandText;

            return newSqlCommandText;
        }



        protected override object ExecuteInternal()
        {

            if (ExecutedTimes > 0)
            {
                // if this query was previously run,
                // then we must re-create parameters in order
                // to use this query multiple times
                ResetParameters();
            }

            // configure the query 
            ConfigureQuery(this.Config);

            // SQL text command to execute
            // For SPs this stands for the stored procedure
            // name. (without Schema). 
            string commandText = GetCommandText();


            if (ReflectionUtils.InheritsFromType<DataSelectCustomQuery>(this.GetType()))
            {
                // append the "USE {Schema};" statement if needed
                commandText = AppendSchemaUse(commandText);
            }

            commandText = Regex.Replace(commandText, @"\s+", " ");


            using (SqlConnection dbConnection = DbContext.CreateConnection())
            {
                using (SqlCommand dbCommand = DbContext.CreateSqlCommand(commandText, dbConnection, CurrentCommandType))
                {
                    dbCommand.CommandTimeout = CommandTimeout;
                    //inject parameters into command when needed
                    if (Config.UseDefaultParameterHandling)
                    {
                        // use DEFAULT parameters handling
                        // insert DB paraneters into DB commands
                        InsertParametersIntoDbCommand(dbCommand);
                    }
                    else if (!Config.UseDefaultParameterHandling &&
                             ReflectionUtils.InheritsFromType<DataSelectCustomQuery>(this.GetType()))
                    {
                        // replace any parameter placeholders with 
                        // actual values
                        string sqlNoParameters = UnParameterizeSqlQuery(commandText);

                        // assign a parameter-less SQL text to be executed
                        // by the SQL command query
                        dbCommand.CommandText = sqlNoParameters;
                    }


                    // open connection to the database
                    dbCommand.Connection.Open();
                    return ProcessDbCommand(dbCommand);
                }
            }
        }

        protected override async Task<object> ExecuteInternalAsync()
        {

            if (ExecutedTimes > 0)
            {
                // if this query was previously run,
                // then we must re-create parameters in order
                // to use this query multiple times
                ResetParameters();
            }

            // configure the query 
            ConfigureQuery(this.Config);

            // SQL text command to execute
            // For SPs this stands for the stored procedure
            // name. (without Schema). 
            string commandText = GetCommandText();


            if (ReflectionUtils.InheritsFromType<DataSelectCustomQuery>(this.GetType()))
            {
                // append the "USE {Schema};" statement if needed
                commandText = AppendSchemaUse(commandText);
            }

            commandText = Regex.Replace(commandText, @"\s+", " ");


            using (SqlConnection dbConnection = DbContext.CreateConnection())
            {
                using (SqlCommand dbCommand = DbContext.CreateSqlCommand(commandText, dbConnection, CurrentCommandType))
                {
                    dbCommand.CommandTimeout = CommandTimeout;
                    //inject parameters into command when needed
                    if (Config.UseDefaultParameterHandling)
                    {
                        // use DEFAULT parameters handling
                        // insert DB paraneters into DB commands
                        InsertParametersIntoDbCommand(dbCommand);
                    }
                    else if (!Config.UseDefaultParameterHandling &&
                             ReflectionUtils.InheritsFromType<DataSelectCustomQuery>(this.GetType()))
                    {
                        // replace any parameter placeholders with 
                        // actual values
                        string sqlNoParameters = UnParameterizeSqlQuery(commandText);

                        // assign a parameter-less SQL text to be executed
                        // by the SQL command query
                        dbCommand.CommandText = sqlNoParameters;
                    }


                    // open connection to the database
                    dbCommand.Connection.Open();
                    return await ProcessDbCommandAsync(dbCommand);
                }
            }
        }


        public bool UsesDefaultParameterHandling
        {

            get
            {
                // use default parameter handling of ADO.NET
                // This setting is used by default to make SQL queries append actual
                // DB parameters with values rather than add strings
                return Config.UseDefaultParameterHandling;
            }
        }

        private void InsertParametersIntoDbCommand(SqlCommand dbCommand)
        {
            // clear any previous parameters
            dbCommand.Parameters.Clear();

            // take all parameters and insert them into DB command
            foreach (var parameterKey in ParametersMap.Keys)
            {
                QueryParameterEntry entry = (QueryParameterEntry)ParametersMap[parameterKey];
                dbCommand.Parameters.Add(entry.Parameter);
            }
        }


        protected virtual TParamType GetParameter<TParamType>(string paramName)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException("Parameter name is not defined!", "paramName");

            QueryParameterEntry entry = (QueryParameterEntry)ParametersMap[paramName];
            if (entry == null)
            {
                // parameter entry was not found by parameter name
                string errorMsg = string.Format("Parameter [{0}] was not found within query parameters!", paramName);
                throw new InvalidOperationException(errorMsg);
            }

            object parameterValue = entry.Parameter.Value;
            return ((parameterValue != null) && !DBNull.Value.Equals(parameterValue))
                                                 ? (TParamType)parameterValue
                                                  : default(TParamType);
        }

        protected bool RemoveParameter(string paramName)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException("Parameter name is not defined!", "paramName");
            QueryParameterEntry entry = (QueryParameterEntry)ParametersMap[paramName];

            if (entry == null)
                return false;

            ParametersMap.Remove(paramName);
            return true;
        }


        protected virtual void SetParameter(string paramName, object paramValue, QueryParameterConfig config)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName");
            if (config == null)
                throw new ArgumentNullException("config", "Parameter configuration is not defined!");




            // get a parameter from the parameters hashmap
            QueryParameterEntry entry = (QueryParameterEntry)ParametersMap[paramName];

            if ((entry != null) && (entry.Config != null) && entry.Config.Equals(config))
            {
                // entry is found for parameter
                // just create a new DB parameter
                // leave the parameter config untouched if it did not change
                if (ExecutedTimes > 0)
                {
                    SqlParameter newDbParameter = config.IsStoredProcedureParameter
                        ? DbContext.CreateStoredProcParameter(
                            paramName: paramName, paramValue: paramValue,
                            direction: config.Direction, sqlDbType: config.DbType,
                            isNullable: config.IsNullable, size: config.Size)
                        : DbContext.CreateParameter(paramName, paramValue);

                    // reset parameter with a one with updated values
                    entry.Parameter = newDbParameter;
                    return;
                }

                // just reset the parameter value 
                // without re-creating parameter
                entry.Parameter.Value = paramValue ?? DBNull.Value;
            }
            else
            {

                SqlParameter newDbParameter = config.IsStoredProcedureParameter
                    ? DbContext.CreateStoredProcParameter(
                        paramName: paramName,
                        paramValue: paramValue,
                        direction: config.Direction,
                        sqlDbType: config.DbType,
                        isNullable: config.IsNullable,
                        size: config.Size)
                    : DbContext.CreateParameter(paramName, paramValue);

                // create a new parameter entry, contating the parameter itself and its configuration
                QueryParameterEntry newEntry = new QueryParameterEntry
                {
                    Parameter = newDbParameter,
                    Config = config
                };

                // save entry in the parameters map
                ParametersMap[paramName] = newEntry;
            }
        }

        /// <summary>
        /// Check if the parameter is set in a query
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public bool IsParameterSet(string paramName)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentException("Parameter name is not defined!", "paramName");

            QueryParameterEntry entry = (QueryParameterEntry)ParametersMap[paramName];

            return (entry != null) &&
                   (entry.Parameter != null) &&
                   string.Equals(entry.Parameter.ParameterName, paramName);
        }
    }
}