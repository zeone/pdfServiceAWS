using System;

namespace PDFServiceAWS.DB
{
    public interface IQueryProvider
    {
        /// <summary>
        /// Creates a Query object specified for a particular Schema (database catalog)
        /// </summary>
        /// <typeparam name="TQuery">Type of Query object to create defined explicitly</typeparam>
        /// <param name="schema">Schama (DB catalog) for which a Query object should be created</param>
        /// <returns></returns>
        TQuery CreateQuery<TQuery>(string schema) where TQuery : BaseQuery;

        /// <summary>
        /// Creates a Query object specified for Shared Schema
        /// </summary>
        /// <typeparam name="TQuery">Type of Query object to create defined explicitly</typeparam>
        /// <returns>Return created query object</returns>
        // TQuery CreateQueryToSharedSchema<TQuery>() where TQuery : BaseQuery;

        /// <summary>
        /// Creates a Query object of the specified type for a particular Schema (database catalog)
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="schema"></param>
        /// <returns></returns>
        BaseQuery CreateQuery(Type queryType, string schema);
    }

}
