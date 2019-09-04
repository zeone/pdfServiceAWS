using System;
using System.Data;
using System.Reflection;
using PDFServiceAWS.Helpers;

namespace PDFServiceAWS.DB.Builder
{
    public class DefaultModelBuilder<TModel> : BaseModelBuilder<TModel> where TModel : new()
    {


        protected override TModel BuildSingleModel(IDataReader dbDataReader, PropertyInfo[] modelProperties)
        {
            TModel model = new TModel();

            foreach (PropertyInfo modelProperty in modelProperties)
            {
                object modelPropertyValue = null;
                try
                {
                    if (dbDataReader.ColumnExists(modelProperty.Name))
                    {
                        // if the model property was found in the returned
                        // SQL response
                        modelPropertyValue = dbDataReader.ReadColumn(modelProperty.Name);
                        //TODO Currently all work well but any way I'll leve it here
                        // sometimes in database we save data in the different type than in the model
                        // for exampple we in the transaction Amount filed use "Decimal" type, but in the model we expect "String"
                        // unfortunately automatic convertation between values types and reference types not working
                        // so we have to check and convertate it manually
                        //var valType = modelPropertyValue?.GetType();
                        //var propertyType = modelProperty.PropertyType;
                        //if (modelPropertyValue != null && IsNumericType(valType)
                        //    && valType != propertyType && !propertyType.IsEnum)
                        //{
                        //    propertyType = IsNullableType(propertyType) ? Nullable.GetUnderlyingType(propertyType) : propertyType;
                        //    modelPropertyValue = Convert.ChangeType(modelPropertyValue, propertyType);
                        //}
                        modelProperty.SetValue(model, modelPropertyValue);
                    }
                }
                catch (ArgumentException argEx)
                {
                    // cannot set the model property
                    string errorMsg = @"Builder failed to set value on the model property [{0}] of model type [{1}]. 
                                        Value to set: {2}
                                        Type of value to set: {3} ";
                    throw new Exception(string.Format(errorMsg,
                                                      modelProperty.Name,
                                                      typeof(TModel).Name,
                                                      modelPropertyValue ?? "NULL",
                                                      (modelPropertyValue != null) ? (object)modelPropertyValue.GetType() : "<none>"),
                                        argEx);
                }
            }

            return model;
        }
        //public static bool IsNumericType(Type o)
        //{
        //    switch (Type.GetTypeCode(o))
        //    {
        //        case TypeCode.Byte:
        //        case TypeCode.SByte:
        //        case TypeCode.UInt16:
        //        case TypeCode.UInt32:
        //        case TypeCode.UInt64:
        //        case TypeCode.Int16:
        //        case TypeCode.Int32:
        //        case TypeCode.Int64:
        //        case TypeCode.Decimal:
        //        case TypeCode.Double:
        //        case TypeCode.Single:
        //        case TypeCode.DateTime:
        //            return true;
        //        default:
        //            return false;
        //    }
        //}

        //private static bool IsNullableType(Type type)
        //{
        //    return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        //}
    }
}