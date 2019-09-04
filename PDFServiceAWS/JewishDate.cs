using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Globalization;

namespace PDFService
{
    public class JewishDate
    {

        private int _day, _month, _year;
        private bool _isLeap;
        private string _monthLabel;

        private static CultureInfo CreateJewishCulture()
        {
            CultureInfo culture = new CultureInfo("he") { DateTimeFormat = { Calendar = new HebrewCalendar() } };
            return culture;
        }

        public static string GetHebrewMonth(int month, bool isLeapYear)
        {
            if ((month < 1) || (month > 13))
                throw new ArgumentException("Hebrew month must be of range [1..13]!", "month");

            #region === SWITCH months ===
            switch (month)
            {
                case 1:
                    return "Tishrei";
                case 2:
                    return "Cheshvan";
                case 3:
                    return "Kislev";
                case 4:
                    return "Tevet";
                case 5:
                    return "Shevat";
                case 6:
                    return !isLeapYear ? "Adar" : "Adar I";
                case 7:
                    return !isLeapYear ? "Nissan" : "Adar II";
                case 8:
                    return !isLeapYear ? "Iyar" : "Nissan";
                case 9:
                    return !isLeapYear ? "Sivan" : "Iyar";
                case 10:
                    return !isLeapYear ? "Tamuz" : "Sivan";
                case 11:
                    return !isLeapYear ? "Av" : "Tamuz";
                case 12:
                    return !isLeapYear ? "Elul" : "Av";
                case 13:
                    return "Elul";
                default:
                    return null;
            }
            #endregion
        }
        //use for customizate date
        public JewishDate(int day, int month, int year, string monthLabel, bool isLeap)
        {
            _day = day;
            _month = month;
            _year = year;
            _monthLabel = monthLabel;
            _isLeap = isLeap;
        }

        public static JewishDate Now()
        {
            return new JewishDate(DateTime.Today);
        }

        public static DateTime HebrewDateToGregorianDate(JewishDate hebrDate)
        {
            return HebrewDateToGregorianDate(hebrDate.Year, hebrDate.Day, hebrDate.Month, hebrDate.MonthLabel, hebrDate.IsLeap);
        }

        public static DateTime HebrewDateToGregorianDate(int year, int day, int? month = null, string monthLabel = null, bool? isLeap = null)
        {
            var jewishCulture = CreateJewishCulture();
            HebrewCalendar calendar = (HebrewCalendar)jewishCulture.DateTimeFormat.Calendar;
            if (isLeap == null)
                isLeap = calendar.IsLeapYear(year);
            if (month == null)
                month = GetHebrewMonthNumber(monthLabel, (bool)isLeap);

            return calendar.ToDateTime(year, month.Value, day, 0, 0, 0, 0);
        }
        public JewishDate(DateTime date)
        {
            var jewishCulture = CreateJewishCulture();
            HebrewCalendar calendar = (HebrewCalendar)jewishCulture.DateTimeFormat.Calendar;

            // convert gregorian date to jewish date
            _month = calendar.GetMonth(date);
            _year = calendar.GetYear(date);
            _day = calendar.GetDayOfMonth(date);
            _monthLabel = GetHebrewMonth(_month, calendar.IsLeapYear(_year));
            _isLeap = calendar.IsLeapYear(_year);
        }

        public string MonthLabel => _monthLabel;

        public int Month => _month;

        public int Year => _year;

        public int Day => _day;

        public bool IsLeap => _isLeap;
        public string GetDate()
        {
            return $"{_day} {_monthLabel} {_year}";
        }

        public override string ToString()
        {
            return $"{Day} {MonthLabel}, {Year}";
        }


        public static int GetHebrewMonthNumber(string monthLabel, bool isLeapYear, bool uknownYear = false)
        {
            if (monthLabel == "Tishrei") return 1;
            if (monthLabel == "Cheshvan") return 2;
            if (monthLabel == "Kislev") return 3;
            if (monthLabel == "Tevet" || monthLabel == "Teves") return 4;
            if (monthLabel == "Shevat") return 5;
            if (uknownYear)
            {
                if (monthLabel == "Adar" || monthLabel == "Adar I" || monthLabel == "Adar Alef") return 6;
                if (monthLabel == "Nissan" || monthLabel == "Nisan"
                                           || monthLabel == "Adar \u2161" || monthLabel == "Adar II") return 7;
            }
            if (monthLabel == "Adar" || monthLabel == "Adar I" || monthLabel == "Adar Alef") return 6;
            if (monthLabel == "Adar \u2161" || monthLabel == "Adar II") return !isLeapYear ? 6 : 7;
            if (monthLabel == "Nissan" || monthLabel == "Nisan") return !isLeapYear ? 7 : 8;
            if (monthLabel == "Iyar") return !isLeapYear ? 8 : 9;
            if (monthLabel == "Sivan") return !isLeapYear ? 9 : 10;
            if (monthLabel == "Tamuz") return !isLeapYear ? 10 : 11;
            if (monthLabel == "Av") return !isLeapYear ? 11 : 12;
            if (monthLabel == "Elul") return !isLeapYear ? 12 : 13;
            throw new ArgumentException("Not valid month label", "month");
        }
    }
}