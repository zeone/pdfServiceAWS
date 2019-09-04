using System;

namespace PDFServiceAWS.DB
{
    /// <summary>
    /// Signature of the callback that is run after the model is 
    /// materialized from the executed Query object
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <param name="model"></param>
    public delegate void ModelMaterializedEventHandler<in TModel>(TModel model);

    /// <summary>
    /// Callback function that is fired after the model is populated
    /// </summary>
    /// <param name="model"></param>
    public delegate void ModelmaterializedEventHandler(object model);

    public sealed class CustomQueryConfiguration
    {

        private Delegate _modelReadyCallback;

        /// <summary>
        /// Usee default handling of query parameters
        /// </summary>
        public bool UseDefaultParameterHandling { get; set; }

        /// <summary>
        /// Boolean value that defines what exactly result 
        /// is returned from the executed query: single record or multiple records
        /// </summary>
        public bool QueryReturnsSingleRecord { get; set; }

        /// <summary>
        /// Type of a model builder uses to construct models
        /// by reading results from the SqlDataReader
        /// </summary>
        public Type ModelBuilderType { get; set; }

        /// <summary>
        /// Set the callback
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="callback"></param>
        public void SetOnModelReadyCallback<TModel>(ModelMaterializedEventHandler<TModel> callback)
        {

            _modelReadyCallback = callback;
        }

        /// <summary>
        /// Get the callback function
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public ModelMaterializedEventHandler<TModel> GetModelReadyCallback<TModel>()
        {
            if (_modelReadyCallback == null)
                return null;

            return (ModelMaterializedEventHandler<TModel>)_modelReadyCallback;
        }


        /// <summary>
        /// Check if the event handler is set to the [ModelReady] event
        /// </summary>
        /// <returns></returns>
        public bool ModelReadyEventSet()
        {
            // if NULL
            return (_modelReadyCallback != null);
        }
    }
}