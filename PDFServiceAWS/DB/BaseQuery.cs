using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace PDFServiceAWS.DB
{
    public abstract class BaseQuery
    {
        private const int DeadLockErrorCode = 1205;

        protected readonly PerSchemaSqlDbContext DbContext;
        protected readonly IQueryProvider Provider;
        private int _execTimes;
        private bool _lastExecFailed;
        private object _cachedResult;
        private bool _isExecuted;




        protected BaseQuery(IQueryProvider provider, PerSchemaSqlDbContext dbContext)
        {
            if (provider == null)
                throw new ArgumentNullException("provider", "Query provider is not defined for query");
            if (dbContext == null)
                throw new ArgumentNullException("dbContext", "DB context is not defined for query");

            DbContext = dbContext;
            Provider = provider;
        }

        /// <summary>
        /// Count of executions this query was run
        /// </summary>
        public int ExecutedTimes
        {
            get
            {
                return _execTimes;
            }
            private set
            {
                _execTimes = value;
            }
        }


        /// <summary>
        /// Current Schema (database catalog) the query is running at
        /// </summary>
        public virtual string Schema
        {
            get
            {
                return DbContext.Schema;
            }
            set
            {
                // empty 
            }
        }


        /// <summary>
        /// Last execution failed?
        /// </summary>
        public bool LastExecutionFailed
        {
            get
            {
                return _lastExecFailed;
            }
            private set
            {
                _lastExecFailed = value;
            }
        }



        /// <summary>
        /// Property used to hold result after query executed
        /// </summary>
        public object CachedResult
        {
            get
            {
                return _cachedResult;
            }
            private set
            {
                _cachedResult = value;
            }
        }

        /// <summary>
        /// Property that defines status of the query. 
        /// </summary>
        protected bool IsExecuted
        {
            get
            {
                return _isExecuted;
            }
            private set
            {
                _isExecuted = value;
            }
        }



        /// <summary>
        /// Execute the query base entry point
        /// </summary>
        /// <returns></returns>
        public object Execute()
        {
            // reset tracking properties
            CachedResult = null;
            IsExecuted = false;
            LastExecutionFailed = false;

            try
            {

                // delegate execution of the query to derived queries
                object execResult = ExecuteInternal();
                CachedResult = execResult;
                IsExecuted = true;


                // increase times the query was executed
                ExecutedTimes++;
                return execResult;
            }
            catch (SqlException sqlEx)
            {
                LastExecutionFailed = true;
                // DEADLOCK issue, still need fixes
                if (sqlEx.Number == DeadLockErrorCode)
                {
                    // handle the SQL Server deadlock issue here
                    return HandleDeadLock();
                }

                throw;
            }
            catch (Exception)
            {
                LastExecutionFailed = true;
                throw;
            }
        }

        /// <summary>
        /// Execute the query base entry point async
        /// </summary>
        /// <returns></returns>
        public async Task<object> ExecuteAsync()
        {
            // reset tracking properties
            CachedResult = null;
            IsExecuted = false;
            LastExecutionFailed = false;

            try
            {

                // delegate execution of the query to derived queries
                object execResult = await ExecuteInternalAsync();
                CachedResult = execResult;
                IsExecuted = true;
                
                // increase times the query was executed
                ExecutedTimes++;
                return execResult;
            }
            catch (SqlException sqlEx)
            {
                LastExecutionFailed = true;
                // DEADLOCK issue, still need fixes
                if (sqlEx.Number == DeadLockErrorCode)
                {
                    // handle the SQL Server deadlock issue here
                    return HandleDeadLock();
                }

                throw;
            }
            catch (Exception)
            {
                LastExecutionFailed = true;
                throw;
            }
        }


        private object HandleDeadLock()
        {
            const int maxRetries = 10;

            int reTry = 0;
            object result = null;

            while (reTry < maxRetries)
            {
                try
                {
                    result = ExecuteInternal();
                    CachedResult = result;
                    LastExecutionFailed = false;
                    return result;
                }
                catch (SqlException sqlEx)
                {
                    if (sqlEx.Number == DeadLockErrorCode)
                    {
                        /* wait a second + extra time each loop step */
                        Thread.Sleep(1000 + (50 * reTry));
                        reTry++;
                    }
                    else
                    {

                        throw;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Executes the query against the database
        /// </summary>
        /// <returns></returns>
        protected abstract object ExecuteInternal();

        protected abstract Task<object> ExecuteInternalAsync();
    }
}