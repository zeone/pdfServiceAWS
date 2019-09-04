using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using Ninject;
using Ninject.Parameters;
using PDFServiceAWS;
using PDFServiceAWS.Enums;
using PDFServiceAWS.DB.SP;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Helpers;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public class LetterService : BaseService, ILetterService
    {
        private readonly IAppSettingsProvider _settingsProvider;
        private readonly IXMLService _xmlService;
        private readonly IHebrewDateService _hebrewDateService;
        string _schema;
        public LetterService(IQueryProvider queryProvider,string schema) : base(queryProvider)
        {
            _hebrewDateService = NinjectBulder.Container.Get<IHebrewDateService>(new ConstructorArgument("schema", schema));
            _settingsProvider = NinjectBulder.Container.Get<IAppSettingsProvider>(new ConstructorArgument("schema", schema));
            _schema = schema;
            _xmlService = new XMLService();
        }

        public PdfSettingDto GetPdfSetting(int id)
        {
            return GetPdfSettings(id).FirstOrDefault();
        }

        public LetterDto GetLetterById(short id)
        {
            var selectLetterById = QueryProvider.CreateQuery<SelectLetterByIdStoredProcedure>(_schema);
            selectLetterById.LetterId = id;
            var result = selectLetterById.Execute();
            if (result.HasNoDataRows) return null;
            var resp = result.FirstRow<LetterDto>();
            if (resp.PDFSettingID != null)
                resp.PdfSettings = GetPdfSetting(resp.PDFSettingID.Value);

            return resp;
        }

        public LetterFieldsDto GetLetterFields(int transactionId, int?[] subCategoryIds = null, bool includeSubCats = true, string currency="$")
        {
            AppSettingRecord defClasses = _settingsProvider.GetSetting(Settings.DefaultClasses);

            var select = QueryProvider.CreateQuery<SelectFieldsForLetterStoredProcedure>(_schema);
            select.TransactionId = transactionId;
            select.ClassesOn = Convert.ToBoolean(defClasses.Value);
            select.SubCategoryIds = subCategoryIds;
            select.SubCatInclude = includeSubCats;

            LetterFieldsDto fields = select.Execute().FirstRow<LetterFieldsDto>();
            ParseXMLFields(ref fields);

            MakeFieldsInRegionalStandarts(ref fields, currency);

            return fields;
        }

        List<PdfSettingDto> GetPdfSettings(int? id = null)
        {
            var selectLettersSp = QueryProvider.CreateQuery<SelectPDFSettingsStoredProcedure>(_schema);
            selectLettersSp.PDFSettingID = id;
            var result = selectLettersSp.Execute();
            return !result.HasNoDataRows ? result.ResultToArray<PdfSettingDto>().ToList() : new List<PdfSettingDto>();
        }

        private void ParseXMLFields(ref LetterFieldsDto fields)
        {
            if (!string.IsNullOrEmpty(fields.Subcategory))
                fields.Subcategory = SplitDetails(_xmlService.GetSubcategory(fields.Subcategory));
            if (!string.IsNullOrEmpty(fields.Category))
                fields.Category = SplitDetails(_xmlService.GetCategory(fields.Category));
            if (!string.IsNullOrEmpty(fields.Subclass))
                fields.Subclass = SplitDetails(_xmlService.GetSubclass(fields.Subclass));
            if (!string.IsNullOrEmpty(fields.Class))
                fields.Class = SplitDetails(_xmlService.GetClass(fields.Class));
        }

        private string SplitDetails(XmlNodeList items)
        {
            var strBuilder = new StringBuilder();
            for (int i = 0; i < items.Count; i++)
                strBuilder.Append(string.Format(items[i].InnerText + " / "));
            var result = strBuilder.ToString().Trim();
            return result.Remove(result.Length - 1).Trim();
        }

        private void MakeFieldsInRegionalStandarts(ref LetterFieldsDto fields, string currency)
        {
            var timeHelper = new TimeHelper(_schema);
            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
            DateTime orgDate;
            DateTime todayOrgDate = timeHelper.TryConvertLocalTimeToOrgTime(out orgDate) ? orgDate.Date : DateTime.Today.Date;
            JewishDate jevishDate = _hebrewDateService.ConvertToHebrewDate(todayOrgDate.Day, todayOrgDate.Month, todayOrgDate.Year, false);
            fields.TodayHebrewDate = jevishDate.GetDate();

            fields.DonationDate = fields.DonationDateTime.ToString(culture.DateTimeFormat.ShortDatePattern);
            //  fields.DonationLongDate = fields.DonationDateTime.ToString(culture.DateTimeFormat.LongDatePattern);
            fields.DonationLongDate = string.Equals(culture.Name, "en-US", StringComparison.InvariantCultureIgnoreCase) ? fields.DonationDateTime.ToString("MMMM d, yyyy")
                : fields.DonationDateTime.ToString(culture.DateTimeFormat.LongDatePattern);

            fields.TodayDate = todayOrgDate.ToString(culture.DateTimeFormat.ShortDatePattern);
            // fields.TodayLongDate = todayOrgDate.ToString(culture.DateTimeFormat.LongDatePattern);
            fields.TodayLongDate = string.Equals(culture.Name, "en-US", StringComparison.InvariantCultureIgnoreCase) ? todayOrgDate.ToString("MMMM d, yyyy")
                : todayOrgDate.ToString(culture.DateTimeFormat.LongDatePattern);
            // fields.DonationShortDate = fields.DonationDateTime.ToString(culture.DateTimeFormat.ShortDatePattern);

            //current in cms we use only dollars so this currency hardcoded
            fields.Amount = $"{currency}{fields.AmountDec}";
        }

    }
}