using System;
using System.Data;
using System.Linq;
using System.Reflection;
using PDFServiceAWS.Helpers;

namespace PDFServiceAWS.DB.Builder
{
    public abstract class BaseModelBuilder<TModel> : IModelBuilder<TModel>
    {

        protected PropertyInfo[] ModelProperties { get; private set; }

        public BaseModelBuilder()
        {
            ModelProperties = typeof(TModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite && p.CanRead)
                .ToArray();
        }

        public virtual TModel BuildDefaultModel()
        {
            return typeof(TModel).IsValueType ? default(TModel) : Activator.CreateInstance<TModel>();
        }

        /// <summary>
        /// Build a single model instance from the DB reader using a set of model properties to bind to.
        /// </summary>
        /// <param name="dbDataReader"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        protected abstract TModel BuildSingleModel(IDataReader dbDataReader, PropertyInfo[] properties);



        public TModel Build(IDataReader dbDataReader)
        {
            // build the model using the PropertyInfo[] list
            TModel constructedModel = BuildSingleModel(dbDataReader, ModelProperties);

            if ((constructedModel != null) && (ModelReady != null))
            {
                // raise event
                // handle the newly constructed model
                ModelReady(constructedModel);
            }

            return constructedModel;
        }

        public TModel BuildScalar(IDataReader dbDataReader)
        {
            // read the first column from the DB reader
            return (TModel)dbDataReader.ReadColumn(0);
        }

        /// <summary>
        /// Event that is fired after the model is materialized
        /// </summary>
        public event ModelMaterializedEventHandler<TModel> ModelReady;
    }
}