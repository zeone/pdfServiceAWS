using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFServiceAWS.Services.Implementation
{
    public class HebrewDateService : IHebrewDateService
    {
        public JewishDate ConvertToHebrewDate(int day, int month, int year, bool afterDark)
        {
            DateTime dateTime = new DateTime(year, month, day);
            if (afterDark)
                dateTime = dateTime.AddDays(1.0);

            return new JewishDate(dateTime);
        }
    }
}