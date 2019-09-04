using System;
using System.Data;
using PDFServiceAWS.Helpers;

namespace PDFServiceAWS.DB.Builder
{
    public sealed class ScalarValueBuilder<TScalar> : BaseModelBuilder<TScalar>
    {

        protected override TScalar BuildSingleModel(IDataReader dbDataReader,
            System.Reflection.PropertyInfo[] properties)
        {
            object scalarValue = dbDataReader.GetValue(0);

            if (typeof(TScalar).Equals(typeof(StringAdapter)))
            {
                // in case we're building the StringAdapter model
                scalarValue = new StringAdapter(
                    ((scalarValue != null) && !DBNull.Value.Equals(scalarValue))
                        ? scalarValue.ToString()
                        : (string) null
                );
            }

            return (TScalar) scalarValue;
        }
    }
}