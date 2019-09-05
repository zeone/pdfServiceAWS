using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Ninject.Infrastructure.Language;
using PDFServiceAWS.DB.Builder;
using PDFServiceAWS.Helpers;

namespace PDFServiceAWS.DB
{
    public abstract class StoredProcedureBase<TResult> : CustomQuery where TResult : new()
    {
        protected StoredProcedureBase(IQueryProvider provider,
                                      PerSchemaSqlDbContext dbContext)
            : base(provider, dbContext)
        {

            // append the @ReturnVal OUTPUT parameter
            // so it's always known what code stored procedure
            // returns after execution
            SetParameter(paramName: "@ReturnVal",
                          paramValue: default(int),
                          sqlDbType: SqlDbType.Int,
                          isNullable: false,
                          direction: ParameterDirection.ReturnValue);

            // initialise out parameters which were tagged by OutTransactionAttribute to parameters map
            InitOutParameters();
        }

        private void InitOutParameters()
        {
            foreach (var result in GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.HasAttribute(typeof(OutParameterAttribute))))
            {
                var targetType = result.PropertyType;
                var instance = result.GetCustomAttribute<OutParameterAttribute>();
                SetParameter(paramName: instance.ParamName,
                    paramValue: targetType.IsValueType ? Activator.CreateInstance(targetType) : null,
                    sqlDbType: instance.SqlType,
                    isNullable: false,
                    direction: ParameterDirection.Output);
            }
        }

        protected int ReturnCode(SqlCommand dbCommand)
        {
            if (dbCommand == null)
                throw new ArgumentNullException("dbCommand", "SQL DB command is not defined!");
            SqlParameter returnVal = dbCommand.Parameters["@ReturnVal"];
            return (int)returnVal.Value;
        }


        protected virtual IModelBuilder<TResult> CreateBuilder()
        {
            IModelBuilder<TResult> builder = (IModelBuilder<TResult>)Activator.CreateInstance(Config.ModelBuilderType);

            if (Config.ModelReadyEventSet())
            {
                // append the event handler here
                builder.ModelReady += Config.GetModelReadyCallback<TResult>();
            }

            return builder;
        }


        protected override void ConfigureQuery(CustomQueryConfiguration config)
        {
            base.ConfigureQuery(config);

            config.ModelBuilderType = IsComplexDataModel(typeof(TResult)) ?
                                          typeof(DefaultModelBuilder<TResult>) :
                                          typeof(ScalarValueBuilder<TResult>);
        }

        protected bool IsComplexDataModel(Type modelType)
        {

            // figure out if the current type is complex. 
            // Complex type is a type having properties that make up entity

            // note: StringAdapter type is primitive cause it encapsulates a plain string
            return modelType.IsClass &&
                   !modelType.IsPrimitive &&
                   !typeof(StringAdapter).IsAssignableFrom(modelType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new SpExecResult Execute()
        {
            // execute the store procedure 
            object baseResult = base.Execute();

            // cast result to the SpExecResult
            return (SpExecResult)baseResult;
        }

        ///// <summary>
        ///// Method to execute current stored procedure in asynchronous mode
        ///// </summary>
        ///// <returns></returns>
        //public Task<SpExecResult> ExecuteAsync()
        //{
        //    return Task.Run(() => Execute());
        //}

        /// <summary>
        /// Method to execute current stored procedure in asynchronous mode
        /// </summary>
        /// <returns></returns>
        public async Task<SpExecResult> ExecuteAsync()
        {
            // execute the store procedure 
            object baseResult = await base.ExecuteAsync();

            // cast result to the SpExecResult
            return (SpExecResult)baseResult;
        }
        protected void SetParameter(string paramName,
                                  object paramValue,
                                  SqlDbType sqlDbType,
                                  bool isNullable,
                                  ParameterDirection direction,
                                  int size = 0)
        {
            if (string.IsNullOrEmpty(paramName))
                throw new ArgumentNullException("paramName", "Name of parameter is not defined!");

            QueryParameterConfig config = new QueryParameterConfig
            {
                ParameterName = paramName,
                IsStoredProcedureParameter = true, // TRUE since it's always an SP parameter
                IsNullable = isNullable,
                Size = size,
                Direction = direction,
                DbType = sqlDbType
            };

            // set parameter in the parent class
            SetParameter(paramName, paramValue, config);
        }



        public sealed override CommandType CurrentCommandType
        {
            get
            {
                // type of SQL command is always [Stored Procedure]
                return CommandType.StoredProcedure;
            }
        }

        /// <summary>
        /// Return code set after stored procedure executed
        /// </summary>
        public int OutReturnCode
        {
            get
            {
                return GetParameter<int>("@ReturnVal");
            }
        }
    }
}