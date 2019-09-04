using System;

namespace PDFServiceAWS.Dto
{
    public class YahrtzeitFieldsDto : LetterFieldsDto
    {
        public YahrtzeitFieldsDto()
        {
        }
        public int RelId { get; set; }
        public int RelFamilyId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Home { get; set; }
        public string Work { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Emergency { get; set; }
        public string Other { get; set; }

        public int DId { get; set; }
        public int DFamilyId { get; set; }
        public string DName { get; set; }
        public string DTitle { get; set; }
        public string DFirstName { get; set; }
        public string DLastName { get; set; }
        public string DHebrewName { get; set; }
        public string DHebrewFather { get; set; }
        public string DHebrewMother { get; set; }
        public string DRelation { get; set; }
        public string DRelationLower => DRelation?.ToLower();
        public string DHisHer { get; set; }
        public string DHisHerLower { get; set; }
        public string DHimHer { get; set; }
        public string DHimHerLower { get; set; }
        public string DHeShe { get; set; }
        public string DHeSheLower { get; set; }
        public string DBenBasLower { get; set; }
        public string DBenBas { get; set; }
        public string DGender { get; set; }
        private DateTime? _yEngDate = null;

        public DateTime? YEngDate
        {
            get { return _yEngDate; }
            set
            {
                _yEngDate = value;
                if (value != null)
                {
                    if (yEngDateAnniv == null) GetyEngDateAnniv();
                    if (_yEngDateHebAnniv == null || string.IsNullOrEmpty(YWeekdayEve)) GetYEngDateHebAnniv();
                }
            }
        }

        public string YHebDate => $"{YHebrewDay} {YHebrewMonth} {YHebrewYear}";
        public int YHebrewDay { get; set; }
        public string YHebrewMonth { get; set; }
        public int? YHebrewYear { get; set; }
        public string DateToday => DateTime.Now.ToShortDateString();
        public string Note { get; set; }
        /// <summary>
        /// hebrew date using current hebrew year (used saved info in table)
        /// </summary>
        public string YHebDateAnniv
        {
            get
            {
                var currHebYer = JewishDate.Now();
                JewishDate origDate;
                DateTime chkDate;
                if (YEngDate != null)
                {
                    origDate = new JewishDate(YEngDate.Value);
                    chkDate = JewishDate.HebrewDateToGregorianDate(currHebYer.Year, origDate.Day, null, origDate.MonthLabel, currHebYer.IsLeap);
                    if (chkDate < DateTime.Now)
                        chkDate = JewishDate.HebrewDateToGregorianDate(currHebYer.Year + 1, origDate.Day, null, origDate.MonthLabel, currHebYer.IsLeap);
                }
                else
                {
                    chkDate = JewishDate.HebrewDateToGregorianDate(currHebYer.Year, YHebrewDay, null, YHebrewMonth, currHebYer.IsLeap);
                    if (chkDate < DateTime.Now)
                        chkDate = JewishDate.HebrewDateToGregorianDate(currHebYer.Year + 1, YHebrewDay, null, YHebrewMonth, currHebYer.IsLeap);
                }

                var resp = new JewishDate(chkDate);
                return resp.ToString();
            }
        }

        public string YWeekdayEve { get; private set; }

        private DateTime? _yEngDateHebAnniv;

        public string YEngDateHebAnniv => _yEngDateHebAnniv != null ? _yEngDateHebAnniv.Value.ToShortDateString() : "";

        public string YEngDateHebAnnivLong =>
            _yEngDateHebAnniv != null ? _yEngDateHebAnniv.Value.ToLongDateString() : "";

        DateTime? yEngDateAnniv;

        public string YEngDateAnniv => yEngDateAnniv != null ? yEngDateAnniv.Value.ToShortDateString() : "";

        public string YEngDateAnnivLong =>
            yEngDateAnniv != null ? yEngDateAnniv.Value.ToLongDateString() : "";

        // public string YWeekdayEve => YEngDateAnniv != null ? "" : YEngDateAnniv.Value.AddDays(-1).DayOfWeek.ToString();
        public string HebrewDateToday => new JewishDate(DateTime.Now).ToString();


        /*Some fields required for csv*/
        public string SortName => $"{LastName}, {FirstName}";
        public string ContactName => $"{Title} {FirstName}";
        public string Salu => FirstName;
        public string FullSalutation => $"{Title} {LastName}";
        public string RelativeName => $"{DTitle} {DFirstName}";
        public string RelativeSortName => $"{DLastName}, {DFirstName}";

        public string HebParents
        {
            get
            {
                var div = "";
                string father = String.Empty;
                string mother = String.Empty;
                if (!string.IsNullOrEmpty(DHebrewMother) && !string.IsNullOrEmpty(DHebrewMother))
                    div = "/";
                if (!string.IsNullOrEmpty(DHebrewMother))
                    mother = $"bas {DHebrewMother}";
                if (!string.IsNullOrEmpty(DHebrewFather))
                    mother = $"ben {DHebrewFather}";
                return $"{father} {div} {mother}";
            }
        }

        void GetyEngDateAnniv()
        {
            if (_yEngDate == null) return;
            var yDate = _yEngDate.Value;
            var yearDiff = DateTime.Now.Year - yDate.Year;
            yEngDateAnniv = new DateTime(yDate.Year, yDate.Month, yDate.Day);
            yEngDateAnniv = yEngDateAnniv.Value.AddYears(yearDiff);
            if (DateTime.Now.Date > yEngDateAnniv.Value.Date)
                yEngDateAnniv = yEngDateAnniv.Value.AddYears(1);
        }

        void GetYEngDateHebAnniv()
        {
            if (_yEngDate == null) return;
            var date = (DateTime)_yEngDate;
            var origHebDate = new JewishDate(date);
            var currHebYer = JewishDate.Now();
            _yEngDateHebAnniv =
                JewishDate.HebrewDateToGregorianDate(year: currHebYer.Year, day: origHebDate.Day, monthLabel: origHebDate.MonthLabel, isLeap: currHebYer.IsLeap);
            if (_yEngDateHebAnniv.Value.Date < DateTime.Now.Date)
                _yEngDateHebAnniv =
                    JewishDate.HebrewDateToGregorianDate(year: currHebYer.Year + 1, day: origHebDate.Day, monthLabel: origHebDate.MonthLabel, isLeap: currHebYer.IsLeap);
            YWeekdayEve = _yEngDateHebAnniv.Value.AddDays(-1).DayOfWeek.ToString();
        }
    }
}