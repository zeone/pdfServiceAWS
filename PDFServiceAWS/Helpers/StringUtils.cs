using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PDFServiceAWS.Helpers
{
    public static class StringUtils
    {
        public static string EmptyStringToNull(string s)
        {
            if (s == null)
                return ReflectionUtils.Null<string>();

            // empty string to NULL
            if (s == string.Empty)
                return ReflectionUtils.Null<string>();

            return s;
        }

        public static string ToMoneyString(this decimal str)
        {
            return str.ToString("C", CultureInfo.CreateSpecificCulture("en-US"));
        }

        public static bool EqualUpperStrings(string s1, string s2)
        {
            bool? basicCompareResult;

            BasicStringsComparison(s1, s2, out basicCompareResult);

            if (basicCompareResult != null)
            {

                return basicCompareResult.Value;
            }

            // make both string upper
            string upperS1 = s1.ToUpper();
            string upperS2 = s2.ToUpper();

            return string.Equals(upperS1, upperS2);
        }

        private static void BasicStringsComparison(string s1, string s2, out bool? result)
        {
            // default result is NULL,
            // since the conditions below 
            // might not be met
            result = default(bool?);

            if ((s1 == null) && (s2 == null))
            {
                // if both strings are NULL,
                // they're considered equal
                result = true;
                return;
            }

            if ((s1 == null) || (s2 == null))
            {
                // if only one string is NULL and the 
                // other is not, then they're not equal
                result = false;
                return;
            }

            if ((s1 == String.Empty) && (s2 == string.Empty))
            {
                // if both strings are empty strings,
                // then they're considered equal
                result = true;
                return;
            }

            if (s1.Length != s2.Length)
            {
                // if lengths of two strings differ,
                // then the strings are definitely not equal
                result = false;
            }
        }

        public static bool EqualLoweredStrings(string s1, string s2)
        {
            bool? basicCompareResult;

            // compare by basic cases
            BasicStringsComparison(s1, s2, out basicCompareResult);

            if (basicCompareResult != null)
            {

                return basicCompareResult.Value;
            }

            // lower the strings 
            string loweredS1 = s1.ToLower();
            string loweredS2 = s2.ToLower();

            return string.Equals(loweredS1, loweredS2);
        }

        public static IEnumerable<string> TrimAll(this IEnumerable<string> strings)
        {
            // ReSharper disable once PossibleMultipleEnumeration
            if (!strings.Any())
                // ReSharper disable once PossibleMultipleEnumeration
                yield return string.Empty;

            foreach (string s in strings)
            {
                string trimmed = s.Trim();
                yield return s;
            }
        }


        public static string[] ConvertToStrings(this StringAdapter[] saList)
        {
            if (!saList.Any())
                return ArrayUtils.Empty<string>();

            string[] strings = saList.Select(sa => sa.Value)
                                     .ToArray();
            return strings;
        }

        public static bool IsBase64(this string str)
        {
            try
            {
                byte[] data = Convert.FromBase64String(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static string CleaupHtmlSpecificSymbols(this string orig)
        {
            if (string.IsNullOrEmpty(orig)) return orig;
            orig = orig.Replace("&nbsp;", " ");
            orig = orig.Replace("&nbsp", " ");
            orig = orig.Replace("nbsp;", " ");
            orig = orig.Replace("nbsp", " ");
            orig = orig.Replace("&amp;", "&");
            return orig;
        }

        public static StringBuilder TrimEnd(this StringBuilder sb)
        {
            if (sb == null || sb.Length == 0) return sb;

            int i = sb.Length - 1;
            for (; i >= 0; i--)
                if (!char.IsWhiteSpace(sb[i]))
                    break;

            if (i < sb.Length - 1)
                sb.Length = i + 1;

            return sb;
        }
    }
}