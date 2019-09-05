using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Enums;
using PDFServiceAWS.Helpers;
using PdfSharp.Pdf;

namespace PDFServiceAWS.Services.Implementation
{
    public abstract class ReportPdfService
    {
        protected IEnumerable<PaymentMethodDto> _paymentMethods;
        protected IEnumerable<SolicitorDto> _solicitors;
        protected IEnumerable<MailingDto> _mailings;
        protected IEnumerable<DepartmentDto> _departments;
        protected IEnumerable<CategoryDto> _categoryTree;
        protected Func<string, string> _translator;
        protected readonly Color _tableBorder = new Color(210, 210, 210);
        protected readonly Color _tableGray = new Color(242, 242, 242);
        protected readonly Color _tableHeaderBackground = new Color(26, 189, 156);
        protected readonly Color _tableHeaderFont = new Color(255, 255, 255);
        protected readonly Color _fontColor = new Color(102, 102, 102);
        protected string CountryInSettings;
        /// <summary>
        /// The MigraDoc document that represents the invoice.
        /// </summary>
        protected Document _document;
        protected PdfDocument outputDocument;
        /// <summary>
        /// The table of the MigraDoc document that contains the invoice items.
        /// </summary>
        protected Table _table;

        protected Section _section;

        public void InitializeCollections(Func<string, string> transFunc, IEnumerable<PaymentMethodDto> paymentMethods,
            IEnumerable<SolicitorDto> solicitors, IEnumerable<MailingDto> mailings,
            IEnumerable<DepartmentDto> departments, IEnumerable<CategoryDto> categoryTree)
        {
            _translator = transFunc;
            _paymentMethods = paymentMethods;
            _solicitors = solicitors;
            _mailings = mailings;
            _departments = departments;
            _categoryTree = categoryTree;
        }

        readonly List<ColumnsPositions> _colPos = new List<ColumnsPositions>();


        protected void DefineStyles()
        {
            // Get the predefined style Normal.
            Style style = _document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = _document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            //style = _document.Styles[StyleNames.Footer];
            //style.ParagraphFormat.AddTabStop("1cm", TabAlignment.Center);
            //style.Font.Name = "Verdana";
            //style.Font.Name = "Times New Roman";
            //style.Font.Size = 9;
            //style.Font.Color = _fontColor;

            // Create a new style called Table based on style Normal
            style = _document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;
            style.Font.Color = _fontColor;

            style = _document.Styles.AddStyle("TableLabel", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 11;
            style.Font.Color = Color.Empty;

            // Create a new style called Reference based on style Normal
            style = _document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        protected void AddEmptyRow()
        {
            Row row = _table.AddRow();
            row.HeightRule = RowHeightRule.Exactly;
            row.Height = 5;
            row.Borders.Visible = false;
        }
        /// <summary>
        /// Mostly use for main rows
        /// </summary>
        /// <param name="row"></param>
        /// <param name="rowParAligment"></param>
        /// <param name="rowBold"></param>
        /// <param name="rowBackgroundColor"></param>
        /// <param name="cellIndex"></param>
        /// <param name="colText"></param>
        /// <param name="colBold"></param>
        /// <param name="colParAligment"></param>
        /// <param name="colVerAlignment"></param>
        /// <param name="rowHeadingFormat"></param>
        /// <param name="fontColor"></param>
        /// <param name="rowHeight"></param>
        /// <param name="fontSize"></param>
        protected void FillRow(Row row, ParagraphAlignment rowParAligment, bool rowBold, Color rowBackgroundColor, int cellIndex, string colText,
            bool colBold, ParagraphAlignment colParAligment, VerticalAlignment colVerAlignment, bool rowHeadingFormat = false, Color? fontColor = null, Unit? rowHeight = null, int? fontSize = null)
        {
            row.HeadingFormat = rowHeadingFormat;
            row.Format.Alignment = rowParAligment;
            row.Format.Font.Bold = rowBold;
            if (rowHeight != null) row.Height = (float)rowHeight;
            // if (fontColor != null) row.Format.Font.Color = (Color)fontColor;
            row.Shading.Color = rowBackgroundColor;
            row.Cells[cellIndex].AddParagraph(colText);
            row.Cells[cellIndex].Format.Font.Bold = colBold;
            if (fontSize != null) row.Cells[cellIndex].Format.Font.Size = (int)fontSize;
            row.Cells[cellIndex].Format.Alignment = colParAligment;
            row.Cells[cellIndex].VerticalAlignment = colVerAlignment;
            if (fontColor != null) row.Cells[cellIndex].Format.Font.Color = (Color)fontColor;
        }
        /// <summary>
        /// Used for generals rows
        /// </summary>
        /// <param name="row"></param>
        /// <param name="cellIndex"></param>
        /// <param name="colText"></param>
        /// <param name="colBold"></param>
        /// <param name="colParAligment"></param>
        /// <param name="colVerAlignment"></param>
        /// <param name="fontColor"></param>
        /// <param name="marginLeft"></param>
        /// <param name="fontSize"></param>
        protected void FillRow(Row row, int cellIndex, string colText, bool colBold, ParagraphAlignment colParAligment, VerticalAlignment colVerAlignment,
            Color? fontColor = null, float? marginLeft = null, int? fontSize = null)
        {

            if (fontSize != null) row.Cells[cellIndex].Format.Font.Size = (int)fontSize;
            if (fontColor != null) row.Cells[cellIndex].Format.Font.Color = (Color)fontColor;
            if (marginLeft != null) row.Cells[cellIndex].Format.LeftIndent = (float)marginLeft;
            row.Cells[cellIndex].AddParagraph(string.IsNullOrEmpty(colText) ? "" : colText);
            row.Cells[cellIndex].Format.Font.Bold = colBold;
            row.Cells[cellIndex].Format.Alignment = colParAligment;
            row.Cells[cellIndex].VerticalAlignment = colVerAlignment;
        }

        protected string GetMethodName(int? methodId)
        {
            return NameHelper.GetMethodName(methodId, _paymentMethods.ToList());
        }

        protected string GetSolicitorName(int? solID)
        {
            var result = NameHelper.GetSolicitorName(solID, _solicitors.ToList());

            return string.IsNullOrEmpty(result) ? GetTranslation("report_no_solicitors") : result;
        }


        protected string GetMailingName(int? mailId)
        {
            var result = NameHelper.GetMailingName(mailId, _mailings.ToList());
            return string.IsNullOrEmpty(result) ? GetTranslation("report_no_mailingDesc") : result;
        }

        protected string GetDepartmentName(int? depId)
        {
            var result = NameHelper.GetDepartmentName(depId, _departments.ToList());
            return string.IsNullOrEmpty(result) ? GetTranslation("report_no_department") : result;
        }

        protected string GetCategoryName(int? catId)
        {
            var result = NameHelper.GetCategoryName(catId, _categoryTree.ToList());
            return string.IsNullOrEmpty(result) ? GetTranslation("report_no_category") : result;
        }
        protected string GetSubCategoryName(int? subcatId)
        {
            var result = NameHelper.GetSubCategoryName(subcatId, _categoryTree.ToList());
            return string.IsNullOrEmpty(result) ? GetTranslation("report_no_subcategory") : result;
        }

        protected string GetCatSubcatName(List<TransactionDetailReportList> trDetails)
        {
            if (trDetails.Count == 1)
                return
                    string.Format("{0}/ {1}", GetCategoryName(trDetails.First().CategoryID),
                        GetSubCategoryName(trDetails.First().SubcategoryID));
            var subCatId = trDetails.First().SubcategoryID;
            return
                string.Format("{0}/ {1}", GetCategoryName(trDetails.First().CategoryID),
                    trDetails.All(e => e.SubcategoryID == subCatId)
                        ? GetSubCategoryName(trDetails.First().SubcategoryID)
                        : GetTranslation("report_subcat_multi"));
        }

        protected string GetTranslation(string toTranslate)
        {
            if (_translator == null) throw new ArgumentNullException("Translation delegate is required");
            return _translator.Invoke(toTranslate);
        }

        protected bool TryGetColIndexFromList(TransactioReportColumns col, out int index)
        {
            index = -1;
            var result = _colPos.FirstOrDefault(r => r.Column == col);
            if (result == null)
                return false;
            index = result.ColPosition;
            return true;
        }
        protected void AddColToPositionList(TransactioReportColumns col, int index)
        {
            _colPos.Add(new ColumnsPositions
            {
                Column = col,
                ColPosition = index
            });
        }

        //used for mapping col position in table
        protected class ColumnsPositions
        {
            public TransactioReportColumns Column { get; set; }
            public int ColPosition { get; set; }
        }

        protected string GetShortName(string name, int maxSize)
        {
            if (name.Length > maxSize) name = name.Substring(0, Math.Min(name.Length, maxSize)) + "...";
            return name;
        }
    }
}