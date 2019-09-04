using System;

namespace PDFServiceAWS.Helpers
{
    /// <summary>
    /// Set of utilities for arrays handling
    /// </summary>
    public static class ArrayUtils
    {
        public static T[] Empty<T>()
        {
            // return a new zero-length array
            return new T[0];
        }


        public static bool IsEmpty(Array array)
        {
            // reverse conditions 
            return !IsNotEmpty(array);
        }

        /// <summary>
        /// Check out if the array has any elements
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsNotEmpty(Array array)
        {
            //
            return (array != null) && (array.Length > 0);
        }
    }
}