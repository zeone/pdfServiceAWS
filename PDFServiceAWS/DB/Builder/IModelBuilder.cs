using System.Data;

namespace PDFServiceAWS.DB.Builder
{
    /// <summary>
    /// Interfacce used to build models from DB data readers
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IModelBuilder<out TModel>
    {
        /// <summary>
        /// Build a model from the data reader
        /// </summary>
        /// <param name="dbDataReader"></param>
        /// <returns></returns>
        TModel Build(IDataReader dbDataReader);

        /// <summary>
        /// Build a model with default state
        /// </summary>
        /// <returns></returns>
        TModel BuildDefaultModel();
        /// <summary>
        /// Event that is fired after the model has been built
        /// </summary>
        event ModelMaterializedEventHandler<TModel> ModelReady;
    }
}
