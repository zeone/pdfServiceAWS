using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.Rendering;
using PDFServiceAWS.Helpers;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.AcroForms;
using PdfSharp.Pdf.Annotations;
using PdfSharp.Pdf.IO;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public class PdfTemplateFunctionality : BaseService, IPdfTemplateFunctionality
    {
        protected readonly Color _tableBorder = new Color(210, 210, 210);
        protected readonly Color _tableGray = new Color(255, 255, 255, 0);
        protected readonly Color _tableHeaderBackground = new Color(255, 255, 255, 0);
        protected readonly Color _tableHeaderFont = new Color(0, 0, 0);
        protected readonly Color _fontColor = new Color(0, 0, 0);


        public PdfTemplateFunctionality( IQueryProvider queryProvider) : base( queryProvider)
        {
        }

        public byte[] GetBatchPdfFromTemplate(List<PdfSettingsBatchDto> pdfSettings, string countryInSettings)
        {
            PdfDocument document = new PdfDocument();
            int pdfsettingsId = 0;
            //create pdf first
            foreach (PdfSettingsBatchDto set in pdfSettings)
            {
                pdfsettingsId++;
                var impDoc = ReadOrCreateDocument(set.Settings, PdfDocumentOpenMode.Modify);
                var tmpDoc = PrepareDocumentForBatch(impDoc, set, countryInSettings);
                RenameAnnotationFields(tmpDoc, set, countryInSettings, pdfsettingsId);
                tmpDoc = ReopenDocumentWithoutSaving(tmpDoc, PdfDocumentOpenMode.Import);
                impDoc.Close();
                foreach (PdfPage page in tmpDoc.Pages)
                {
                    document.AddPage(page);
                }
                // tmpDoc.Close();
            }
            //  FieldsTest(document, "Amount");
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                ms.Close();
                document.Close();
                // File.Delete(path);
                return buffer;
            }
        }

        public byte[] GetBatchEndYearPdfFromTemplate(List<PdfSettingsBatchDto> pdfSettings, string countryInSettings)
        {
            PdfDocument document = new PdfDocument();
            int pdfsettingsId = 0;
            var groupedSettings = pdfSettings.GroupBy(r => r.Fields.FamilyID).OrderBy(r => r.First().Fields.Name);
            foreach (IGrouping<int, PdfSettingsBatchDto> groupedSetting in groupedSettings)
            {
                var groupedByAddress = groupedSetting.GroupBy(r => r.Settings.AddressId);
                foreach (IGrouping<int, PdfSettingsBatchDto> addrGrp in groupedByAddress)
                {
                    var baseSettingsForFamily = addrGrp.First();
                    var settings = baseSettingsForFamily.Settings;
                    var country = baseSettingsForFamily.CountryInSettings;
                    var baseFields = baseSettingsForFamily.Fields;
                    var totalAmount = addrGrp.Where(r => r.Fields.TransType == 1).Sum(r => r.Fields.AmountDec);
                    var transactionList = GroupTransactionsByFields(addrGrp);
                    pdfsettingsId++;
                    var impDoc = ReadOrCreateDocument(settings, PdfDocumentOpenMode.Modify);
                    var tmpDoc = PrepareDocumentForEndYearBatch(impDoc, settings, country, baseFields, totalAmount, transactionList);
                    RenameAnnotationFields(tmpDoc, baseSettingsForFamily, countryInSettings, pdfsettingsId);
                    tmpDoc = ReopenDocumentWithoutSaving(tmpDoc, PdfDocumentOpenMode.Import);
                    impDoc.Close();
                    foreach (PdfPage page in tmpDoc.Pages)
                    {
                        document.AddPage(page);
                    }
                }

            }
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                ms.Close();
                document.Close();
                return buffer;
            }
        }




        /// <summary>
        /// used for batch
        /// if not do this, all fields in pdf document will have the same values
        /// </summary>
        /// <param name="document"></param>
        void RenameAnnotationFields(PdfDocument document, PdfSettingsBatchDto set, string country, int unicNum)
        {
            for (int i = 0; i < document.Pages.Count; i++)
            {
                PdfAnnotations anotations = document.Pages[i].Annotations;
                for (int j = 0; j < anotations.Count; j++)
                {
                    PdfAnnotation obj;
                    if (!string.IsNullOrEmpty(GetValueByName(anotations[j].Title, set.Fields, set.Settings, country)))
                    {
                        obj = anotations[j];
                        obj.Title = $"{anotations[j].Title}_{unicNum}";
                    }

                }
            }
        }
        /// <summary>
        /// Create Single page
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="countryInSettings"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public byte[] GetPdfFromTamplate(PdfSettingDto settings, string countryInSettings, LetterFieldsDto fields)
        {


            PdfDocument document = PrepareDocument(ReadOrCreateDocument(settings, PdfDocumentOpenMode.Modify), settings, countryInSettings, fields);


            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                ms.Close();
                return buffer;
            }
        }

        /// <summary>
        /// Save File end return path(use for batch letter
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="countryInSettings"></param>
        /// <param name="fields"></param>
        /// <param name="path">path for saving</param>
        /// <returns></returns>
        public string GetBatchPdfFromTamplate(PdfSettingDto settings, string countryInSettings, LetterFieldsDto fields, string path)
        {

            PdfDocument document = PrepareDocument(ReadOrCreateDocument(settings, PdfDocumentOpenMode.Modify), settings, countryInSettings, fields);

            document.Save(path);
            document.Dispose();
            return path;

        }
        public byte[] GetPdfPreview(PdfSettingDto settings, string countryInSettings, LetterFieldsDto fields)
        {


            PdfDocument document = PrepareDocument(CreateDocumentForPreview(settings, PdfDocumentOpenMode.Modify), settings, countryInSettings, fields, isBase64: true);


            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                ms.Close();
                return buffer;
            }
        }
        PdfDocument PrepareDocumentForBatch(PdfDocument document, PdfSettingsBatchDto set, string countryInSettings)
        {
            var settings = set.Settings;
            Document doc = new Document();
            PdfPage page = document.Pages[0];

            Section sec = doc.AddSection();


            sec.PageSetup.LeftMargin = Unit.FromInch(settings.LeftMargin);
            sec.PageSetup.RightMargin = Unit.FromInch(settings.RightMargin);
            sec.PageSetup.TopMargin = Unit.FromInch(settings.TopMargin);
            sec.PageSetup.BottomMargin = Unit.FromInch(settings.BottomMargin);

            //date
            if (settings.ShowDate)
                AddDate(sec, settings);

            //address 
            if (settings.ShowAddress)
                AddAddress(sec, settings, set.CountryInSettings);

            // body
            AddBody(sec, settings);

            //signature
            if (settings.ShowSingnature)
                AddSignature(sec, settings);

            //fields
            if (settings.HasReceiptStub)
                FillFields(document, set.Fields, settings, countryInSettings);
            DocumentRenderer docRenderer = new DocumentRenderer(doc);
            docRenderer.PrepareDocument();
            int pages = docRenderer.FormattedDocument.PageCount;
            for (int j = 1; j <= pages; ++j)
            {
                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                {
                    // HACK²
                    gfx.MUH = PdfFontEncoding.Unicode;


                    docRenderer.RenderPage(gfx, j);
                }
            }
            return document;
        }
        PdfDocument PrepareDocument(PdfDocument document, PdfSettingDto settings, string countryInSettings, LetterFieldsDto fields, int pageNum = 0, bool isBase64 = false)
        {


            PdfPage page = document.Pages[pageNum];
            XGraphics gfx = XGraphics.FromPdfPage(page);
            // HACK²
            gfx.MUH = PdfFontEncoding.Unicode;
            Document doc = new Document();
            Section sec = doc.AddSection();
            DocumentRenderer docRenderer = new DocumentRenderer(doc);

            sec.PageSetup.LeftMargin = Unit.FromInch(settings.LeftMargin);
            sec.PageSetup.RightMargin = Unit.FromInch(settings.RightMargin);
            sec.PageSetup.TopMargin = Unit.FromInch(settings.TopMargin);
            sec.PageSetup.BottomMargin = Unit.FromInch(settings.BottomMargin);

            //date
            if (settings.ShowDate)
                AddDate(sec, settings);

            //address 
            if (settings.ShowAddress)
                AddAddress(sec, settings, countryInSettings);

            // body
            AddBody(sec, settings);

            //signature
            if (settings.ShowSingnature)
                AddSignature(sec, settings, isBase64);

            //fields
            if (settings.HasReceiptStub)
                FillFields(document, fields, settings, countryInSettings);
            docRenderer.PrepareDocument();
            int pageCount = docRenderer.FormattedDocument.PageCount;
            for (int idx = 0; idx < pageCount; idx++)
            {


                docRenderer.RenderPage(gfx, idx + 1, PageRenderOptions.All);
            }

            gfx.Dispose();
            return document;
        }

        private PdfDocument PrepareDocumentForEndYearBatch(PdfDocument document, PdfSettingDto settings, string countryInSettings, LetterFieldsDto fields, decimal totalAmount, List<EndYearTransactions> transactionList)
        {
            Document doc = new Document();
            //  PdfPage page = document.Pages[0];

            Section sec = doc.AddSection();

            sec.PageSetup.LeftMargin = Unit.FromInch(settings.LeftMargin);
            sec.PageSetup.RightMargin = Unit.FromInch(settings.RightMargin);
            sec.PageSetup.TopMargin = Unit.FromInch(settings.TopMargin);
            sec.PageSetup.BottomMargin = Unit.FromInch(settings.BottomMargin);

            //date
            if (settings.ShowDate)
                AddDate(sec, settings);

            //address 
            if (settings.ShowAddress)
                AddAddress(sec, settings, countryInSettings);

            // body
            AddEndYearBody(sec, settings, transactionList, totalAmount);

            //signature
            //if (settings.ShowSingnature)
            //    AddSignature(sec, settings);

            //fields
            //if (settings.HasReceiptStub)
            //    FillFields(document, fields, settings, countryInSettings, true);
            ConvertMigraDocToPdfSharp(ref document, doc);
            return document;
        }


        private void AddSignature(Section sec, PdfSettingDto settings, bool isBase64 = false)
        {
            //todo implement sending signatures to service

            return;
            AddEmptyLineForSpacing(sec, Unit.FromInch(settings.ClosingAdj));
            var para = sec.AddParagraph();

            FillDefaultParagraphSettings(para, settings);
            para.Format.LineSpacingRule = LineSpacingRule.Exactly;
            para.Format.LineSpacing = Unit.FromInch(settings.LineSpacing);
            para.Format.LeftIndent = Unit.FromInch(settings.SignatureIndent);
            if (!string.IsNullOrEmpty(settings.Closing)) para.AddText(settings.Closing);
            if (!string.IsNullOrEmpty(settings.SignatureImage) && settings.SignatureImage.IsBase64())
            {
                Image image = sec.AddImage("base64:" + settings.SignatureImage);
                image.LockAspectRatio = true;
                if (settings.SignatureWidth != null)
                    image.Width = Unit.FromInch(settings.SignatureWidth.Value);
                if (settings.SignatureHeight != null)
                    image.Width = Unit.FromInch(settings.SignatureHeight.Value);
                image.Left = Unit.FromInch(settings.SignatureIndent);
            }
            else
            {
                ////var serverUtil = WorkingContext.HttpContext.Server;
                ////var directory = serverUtil.MapPath($"/PDFs/{ExecutingUser.Schema}/Templates/Signatures");
                //if (Directory.Exists(directory) && !string.IsNullOrEmpty(settings.SignatureImage))
                //{
                //    string path = Path.Combine(directory, settings.SignatureImage);
                //    if (File.Exists(path))
                //    {
                //        Image image = sec.AddImage(path);
                //        image.LockAspectRatio = true;
                //        if (settings.SignatureWidth != null)
                //            image.Width = Unit.FromInch(settings.SignatureWidth.Value);
                //        if (settings.SignatureHeight != null)
                //            image.Width = Unit.FromInch(settings.SignatureHeight.Value);
                //        image.Left = Unit.FromInch(settings.SignatureIndent);
                //    }
                //}
            }

            //add sign name
            if (!string.IsNullOrEmpty(settings.SignatureName))
                AddSignatureName(sec, settings);
            //add sign title
            if (!string.IsNullOrEmpty(settings.SignatureTitle))
                AddSignatureTitle(sec, settings);
            //add singnature PS
            if (!string.IsNullOrEmpty(settings.PS))
            {
                AddEmptyLineForSpacing(sec, Unit.FromInch(settings.BlockSpacing));
                AddSignaturePS(sec, settings);
            }

        }

        void AddSignatureName(Section sec, PdfSettingDto settings)
        {
            if (string.IsNullOrEmpty(settings.SignatureName)) return;
            AddEmptyLineForSpacing(sec, Unit.FromInch(settings.SignatureNameAdj));
            var para = sec.AddParagraph();

            FillDefaultParagraphSettings(para, settings);
            para.Format.LineSpacingRule = LineSpacingRule.Exactly;
            para.Format.LineSpacing = Unit.FromInch(settings.LineSpacing);
            para.Format.LeftIndent = Unit.FromInch(settings.SignatureIndent);
            para.AddText(settings.SignatureName);
        }

        void AddSignatureTitle(Section sec, PdfSettingDto settings)
        {
            if (string.IsNullOrEmpty(settings.SignatureTitle)) return;
            var para = sec.AddParagraph();
            FillDefaultParagraphSettings(para, settings);
            para.Format.LineSpacingRule = LineSpacingRule.Exactly;
            para.Format.LineSpacing = Unit.FromInch(settings.LineSpacing);
            para.Format.LeftIndent = Unit.FromInch(settings.SignatureIndent);
            para.AddText(settings.SignatureTitle);
        }
        void AddSignaturePS(Section sec, PdfSettingDto settings)
        {
            if (string.IsNullOrEmpty(settings.PS)) return;
            var para = sec.AddParagraph();
            FillDefaultParagraphSettings(para, settings);
            para.Format.LineSpacingRule = LineSpacingRule.Exactly;
            para.Format.LineSpacing = Unit.FromInch(settings.LineSpacing);
            para.Format.LeftIndent = Unit.FromInch(settings.SignatureIndent);
            para.AddText(settings.PS);
        }

        private void AddBody(Section sec, PdfSettingDto settings)
        {
            if (string.IsNullOrEmpty(settings.BodyText)) return;
            var para = sec.AddParagraph();

            FillDefaultParagraphSettings(para, settings);
            para.Format.LineSpacingRule = LineSpacingRule.Exactly;
            para.Format.LineSpacing = Unit.FromInch(settings.LineSpacing);
            para.Format.FirstLineIndent = Unit.FromInch(settings.FirstLineIntent);

            para.AddText(settings.BodyText.CleaupHtmlSpecificSymbols());
        }

        #region endYear body
        private void AddEndYearBody(Section sec, PdfSettingDto settings, List<EndYearTransactions> transactionList, decimal totalAmount)
        {
            if (string.IsNullOrEmpty(settings.BodyText)) return;
            var para = sec.AddParagraph();

            var bodyText = settings.BodyText;
            if (bodyText.Contains("<<TotalAmount>>"))
            {

                bodyText = bodyText.Replace("<<TotalAmount>>", totalAmount.ToMoneyString());
            }

            var splittedBody = bodyText.Split(new string[] { "<<TransactionList>>" }, StringSplitOptions.None);
            var firstPart = splittedBody.Length > 0 ? splittedBody[0] : "";
            var lastPart = splittedBody.Length > 1 ? splittedBody[1] : "";

            //add the first part
            if (!string.IsNullOrEmpty(firstPart))
            {
                FillDefaultParagraphSettings(para, settings);
                para.Format.LineSpacingRule = LineSpacingRule.Exactly;
                para.Format.LineSpacing = Unit.FromInch(settings.LineSpacing);
                para.Format.FirstLineIndent = Unit.FromInch(settings.FirstLineIntent);
                para.AddText(firstPart.CleaupHtmlSpecificSymbols());
            }
            // add table with transaction list
            AddTransactionList(sec, transactionList);

            //add second part of text
            if (!string.IsNullOrEmpty(lastPart))
            {
                para = sec.AddParagraph();
                FillDefaultParagraphSettings(para, settings);
                para.Format.LineSpacingRule = LineSpacingRule.Exactly;
                para.Format.LineSpacing = Unit.FromInch(settings.LineSpacing);
                para.Format.FirstLineIndent = Unit.FromInch(settings.FirstLineIntent);
                para.AddText(lastPart.CleaupHtmlSpecificSymbols());
            }
        }

        private void AddTransactionList(Section sec, List<EndYearTransactions> transactionList)
        {
            var table = sec.AddTable();

            //date
            Column column = table.AddColumn();
            column.Width = Unit.FromCentimeter(2);
            column.Format.Alignment = ParagraphAlignment.Left;
            //method
            column = table.AddColumn();
            column.Width = Unit.FromCentimeter(2.5);
            column.Format.Alignment = ParagraphAlignment.Left;
            //number
            column = table.AddColumn();
            column.Width = Unit.FromCentimeter(1.5);
            column.Format.Alignment = ParagraphAlignment.Left;
            //Amount
            column = table.AddColumn();
            column.Width = Unit.FromCentimeter(2);
            column.Format.Alignment = ParagraphAlignment.Left;
            //Reciipt#
            column = table.AddColumn();
            column.Width = Unit.FromCentimeter(2);
            column.RightPadding = Unit.FromCentimeter(0.5);
            column.Format.Alignment = ParagraphAlignment.Left;
            //purpose(cat/subcat)
            column = table.AddColumn();
            column.Width = Unit.FromCentimeter(5.5);
            column.Format.Alignment = ParagraphAlignment.Left;

            Row row = table.AddRow();
            FillRow(row, ParagraphAlignment.Left, false, null, 0, "Date", true, ParagraphAlignment.Left, VerticalAlignment.Center, true, _tableHeaderFont, fontSize: 8, addBottomLine: true);
            FillRow(row, ParagraphAlignment.Left, false, null, 1, "Method", true, ParagraphAlignment.Left, VerticalAlignment.Center, true, _tableHeaderFont, fontSize: 8, addBottomLine: true);
            FillRow(row, ParagraphAlignment.Left, false, null, 2, "Number", true, ParagraphAlignment.Left, VerticalAlignment.Center, true, _tableHeaderFont, fontSize: 8, addBottomLine: true);
            FillRow(row, ParagraphAlignment.Left, false, null, 3, "Amount", true, ParagraphAlignment.Right, VerticalAlignment.Center, true, _tableHeaderFont, fontSize: 8, addBottomLine: true);
            FillRow(row, ParagraphAlignment.Left, false, null, 4, "Receipt #", true, ParagraphAlignment.Right, VerticalAlignment.Center, true, _tableHeaderFont, fontSize: 8, addBottomLine: true);
            FillRow(row, ParagraphAlignment.Left, false, null, 5, "Purpose", true, ParagraphAlignment.Left, VerticalAlignment.Center, true, _tableHeaderFont, fontSize: 8, addBottomLine: true);

            int transIndexer = 0;
            int transCount = transactionList.Count;
            foreach (EndYearTransactions tr in transactionList.OrderBy(d => d.DonationDateTime))
            {
                transIndexer++;
                row = table.AddRow();
                FillRow(row, ParagraphAlignment.Left, false, null, 0, tr.Date ?? "", false, ParagraphAlignment.Left, VerticalAlignment.Center, true, _fontColor, fontSize: 8, addBottomLine: transIndexer == transCount);
                FillRow(row, ParagraphAlignment.Left, false, null, 1, tr.Method ?? "", false, ParagraphAlignment.Left, VerticalAlignment.Center, true, _fontColor, fontSize: 8, addBottomLine: transIndexer == transCount);
                FillRow(row, ParagraphAlignment.Left, false, null, 2, tr.Number ?? "", false, ParagraphAlignment.Left, VerticalAlignment.Center, true, _fontColor, fontSize: 8, addBottomLine: transIndexer == transCount);
                FillRow(row, ParagraphAlignment.Left, false, null, 3, tr.Amount ?? "", false, ParagraphAlignment.Right, VerticalAlignment.Center, true, _fontColor, fontSize: 8, addBottomLine: transIndexer == transCount);
                FillRow(row, ParagraphAlignment.Left, false, null, 4, tr.Receipt.ToString(), false, ParagraphAlignment.Right, VerticalAlignment.Center, true, _fontColor, fontSize: 8, addBottomLine: transIndexer == transCount);
                FillRow(row, ParagraphAlignment.Left, false, null, 5, tr.Purpose ?? "", false, ParagraphAlignment.Left, VerticalAlignment.Center, true, _fontColor, fontSize: 8, addBottomLine: transIndexer == transCount);
            }
        }
        protected void FillRow(Row row, ParagraphAlignment rowParAligment, bool rowBold, Color? rowBackgroundColor, int cellIndex, string colText,
            bool colBold, ParagraphAlignment colParAligment, VerticalAlignment colVerAlignment, bool rowHeadingFormat = false, Color? fontColor = null,
            Unit? rowHeight = null, int? fontSize = null, bool addBottomLine = false)
        {
            row.HeadingFormat = rowHeadingFormat;
            row.Format.Alignment = rowParAligment;
            row.Format.Font.Bold = rowBold;
            if (rowHeight != null) row.Height = (float)rowHeight;
            // if (fontColor != null) row.Format.Font.Color = (Color)fontColor;
            if (rowBackgroundColor != null) row.Shading.Color = (Color)rowBackgroundColor;
            row.Cells[cellIndex].AddParagraph(colText);
            row.Cells[cellIndex].Format.Font.Bold = colBold;
            if (fontSize != null) row.Cells[cellIndex].Format.Font.Size = (int)fontSize;
            row.Cells[cellIndex].Format.Alignment = colParAligment;
            row.Cells[cellIndex].VerticalAlignment = colVerAlignment;
            if (addBottomLine) row.Cells[cellIndex].Borders.Bottom.Color = _fontColor;
            //row.Cells[cellIndex].Borders.Left.Color = _fontColor;
            //row.Cells[cellIndex].Borders.Right.Color = _fontColor;
            if (fontColor != null) row.Cells[cellIndex].Format.Font.Color = (Color)fontColor;
        }

        #endregion

        private void AddAddress(Section sec, PdfSettingDto settings, string countryInSettings)
        {
            Paragraph para = sec.AddParagraph();
            para = sec.AddParagraph();
            FillDefaultParagraphSettings(para, settings);
            para.Format.Alignment = settings.AddressAligment;
            para.Format.LeftIndent = Unit.FromInch(settings.AddressIndent);
            para.AddText(GetAddress(settings, countryInSettings));
            AddEmptyLineForSpacing(sec, Unit.FromInch(settings.BlockSpacing));
        }

        private void AddDate(Section sec, PdfSettingDto settings)
        {
            Paragraph para = sec.AddParagraph();
            FillDefaultParagraphSettings(para, settings);
            para.Format.Alignment = settings.DateAligment;
            para.Format.LeftIndent = Unit.FromInch(settings.DateIndent);

            para.AddText(settings.UseLongDateFormat ? settings.Date.Date.ToString("MMMM d, yyyy") : settings.Date.Date.ToShortDateString());
            if (settings.ShowHebrewDate) para.AddText(Environment.NewLine + new JewishDate(settings.Date.Date).ToString());
            AddEmptyLineForSpacing(sec, Unit.FromInch(settings.BlockSpacing));
        }


        private string GetAddress(PdfSettingDto settings, string countryInSettings)
        {
            var addressLine = new StringBuilder();
            if (!string.IsNullOrEmpty(settings.Label))
                addressLine.Append($"{settings.Label}\n");
            if (!string.IsNullOrEmpty(settings.CompanyName))
                addressLine.Append($"{settings.CompanyName}\n");
            addressLine.Append($"{settings.Address1}\n");
            if (!string.IsNullOrEmpty(settings.Address2))
                addressLine.Append($"{settings.Address2}\n");

            addressLine.Append(GetCSZByTemplate(settings.CSZ, settings.City, settings.State, settings.Zip));

            if (string.IsNullOrEmpty(countryInSettings) || !string.Equals(settings.Country, countryInSettings, StringComparison.InvariantCultureIgnoreCase))
                addressLine.Append($" \n{settings.Country}\n");
            return addressLine.ToString();
        }

        private string GetAddress(LetterFieldsDto fields, string countryInSettings, string cszTemplate)
        {
            var addressLine = new StringBuilder();
            addressLine.Append($"{fields.Label}\n");
            if (!string.IsNullOrEmpty(fields.CompanyName))
                addressLine.Append($"{fields.CompanyName}\n");
            addressLine.Append($"{fields.Address}\n");
            if (!string.IsNullOrEmpty(fields.Address2))
                addressLine.Append($"{fields.Address2}\n");

            addressLine.Append(GetCSZByTemplate(cszTemplate, fields.City, fields.State, fields.Zip));



            if (string.IsNullOrEmpty(countryInSettings) || !string.Equals(fields.Country, countryInSettings, StringComparison.InvariantCultureIgnoreCase))
                addressLine.Append($"\n{fields.Country}\n");
            return addressLine.ToString();
        }

        string GetCSZByTemplate(string template, string city, string state, string zip)
        {
            var templateChars = CleanCSZFromUselessCommas(template, city, state, zip);

            var addressLine = new StringBuilder();
            foreach (char c in templateChars)
            {
                switch (c)
                {
                    case ',':
                        addressLine.TrimEnd().Append(", ");
                        break;
                    case 'C':
                        if (!string.IsNullOrEmpty(city)) addressLine.Append($"{city} ");
                        break;
                    case 'S':
                        if (!string.IsNullOrEmpty(state)) addressLine.Append($"{state} ");
                        break;
                    case 'Z':
                        if (!string.IsNullOrEmpty(zip)) addressLine.Append($"{zip} ");
                        break;
                }
            }


            return addressLine.ToString();
        }

        Char[] CleanCSZFromUselessCommas(string template, string city, string state, string zip)
        {
            var templateChars = template.ToUpper().ToCharArray();
            StringBuilder newTemplate = new StringBuilder();

            var maxValue = templateChars.Length;
            for (int i = 0; i < templateChars.Length; i++)
            {
                switch (templateChars[i])
                {
                    case 'C':
                        if (!string.IsNullOrEmpty(city))
                        {
                            newTemplate.Append('C');
                            if ((i + 1) < maxValue && templateChars[i + 1] == ',') newTemplate.Append(',');
                        }
                        break;
                    case 'S':
                        if (!string.IsNullOrEmpty(state))
                        {
                            newTemplate.Append('S');
                            if ((i + 1) < maxValue && templateChars[i + 1] == ',') newTemplate.Append(',');
                        }
                        break;
                    case 'Z':
                        if (!string.IsNullOrEmpty(zip))
                        {
                            newTemplate.Append('Z');
                            if ((i + 1) < maxValue && templateChars[i + 1] == ',') newTemplate.Append(',');
                        }
                        break;

                }

            }

            return newTemplate.ToString().ToCharArray();
        }

        void AddEmptyLineForSpacing(Section sec, Unit lineIntend)
        {
            Paragraph para = sec.AddParagraph();
            para.Format.LineSpacingRule = LineSpacingRule.Exactly;
            para.Format.LineSpacing = lineIntend;
        }

        void FillDefaultParagraphSettings(Paragraph par, PdfSettingDto settings)
        {
            par.Format.Alignment = settings.Aligment;
            par.Format.Font.Name = settings.FontName;
            par.Format.Font.Size = settings.FontSize;
            par.Format.Font.Color = Colors.Black;
        }


        void FillFields(PdfDocument document, LetterFieldsDto fields, PdfSettingDto settings, string countryInSettings, bool isEndYear = false)
        {
            PdfAcroForm form = document.AcroForm;

            if (form == null || fields == null) return;
            var acroFields = form.Fields;
            var names = acroFields.DescendantNames;
            if (!names.Any()) names = acroFields.Names;


            foreach (string name in names)
            {
                var t = form.Fields[name];
                var text = GetValueByName(name, fields, settings, countryInSettings);

                t.Value = new PdfString(text);
                ((PdfTextField)t).BackColor = XColor.FromKnownColor(XKnownColor.White);
                t.ReadOnly = true;


            }
            //Ensure the new values are displayed
            if (document.AcroForm.Elements.ContainsKey("/NeedAppearances") == false)
                document.AcroForm.Elements.Add("/NeedAppearances", new PdfBoolean(true));
            else
                document.AcroForm.Elements["/NeedAppearances"] = new PdfBoolean(true);
        }


        string GetValueByName(string name, LetterFieldsDto fields, PdfSettingDto settings, string countryInSettings)
        {
            switch (name)
            {
                case "Name":
                    return fields.Name;
                case "CompanyName":
                    return fields.CompanyName;
                case "Address":
                    return fields.Address;
                case "Address2":
                    return fields.Address2;
                case "City":
                    return fields.City;
                case "State":
                    return fields.State;
                case "Zip":
                    return fields.Zip;
                case "Country":
                    return fields.Country;
                case "NameAddress":
                    return GetAddress(fields, countryInSettings, settings.CSZ);
                case "CityStateZip":
                    return GetCSZByTemplate(settings.CSZ, fields.City, fields.State, fields.Zip);
                case "Amount":
                    return fields.Amount;
                case "GoodsAmount":
                    //todo implement
                    return "";
                case "GoodsDescription":
                    //todo implement
                    return "";
                case "TaxDeductibleAmount":
                    //todo implement
                    return "";
                case "TextAmount":
                    //todo implement
                    return "";
                case "HonorMemory":
                    return fields.HonorMemory;
                case "DonationDate":
                    return fields.DonationDate;
                case "DonationLongDate":
                    return fields.DonationLongDate;
                case "Method":
                    return fields.PaymentMethod;
                case "CheckNumber":
                    return fields.CheckNo;
                case "ReceiptNumber":
                    return fields.ReceiptNo.ToString();
                case "Category":
                    return fields.Category;
                case "Subcategory":
                    return fields.Subcategory;
                case "Purpose":
                    return fields.Subcategory;
                case "Solicitor":
                    return fields.Solicitor;
                case "Organization":
                    return fields.Department;
                case "TransactionNote":
                    //todo implement
                    return "";
                case "HebrewDate":
                    return fields.TodayHebrewDate;
                case "TodayDate":
                    return fields.TodayDate;
                case "TodayLongDate":
                    return fields.TodayLongDate;
            }

            return "";
        }
        PdfDocument ReadOrCreateDocument(PdfSettingDto settings, PdfDocumentOpenMode mode)
        {
            //todo implement sending template
            return new PdfDocument();
            string directory = "";
            PdfDocument document;
            if (string.IsNullOrEmpty(settings.Template))
            {
                // check if user didn't setup manual page size, will setup it like A4 format
                if (settings.PageHeight == null) settings.PageHeight = 11;
                if (settings.PageWidth == null) settings.PageWidth = 8.5;
                document = new PdfDocument();
                var initPage = document.AddPage();
                initPage.Height = XUnit.FromInch(settings.PageHeight.Value);
                initPage.Width = XUnit.FromInch(settings.PageWidth.Value);
                //document should be imported so I have to reopen it
                using (MemoryStream ms = new MemoryStream())
                {
                    document.Save(ms, false);
                    document = PdfReader.Open(ms, mode);
                }
            }
            else
            {
                //var serverUtil = WorkingContext.HttpContext.Server;
                //directory = serverUtil.MapPath($"/PDFs/{ExecutingUser.Schema}/Templates");
                //if (!Directory.Exists(directory)) throw new DirectoryNotFoundException($"{directory} not found");
                //string path = Path.Combine(directory, settings.Template);
                //document = PdfReader.Open(path, mode);
            }

            return document;
        }

        PdfDocument CreateDocumentForPreview(PdfSettingDto settings, PdfDocumentOpenMode mode)
        {
            //todo implement sending template
            return new PdfDocument();
            string directory = "";
            PdfDocument document;
            if (string.IsNullOrEmpty(settings.Template))
            {
                // check if user didn't setup manual page size, will setup it like A4 format
                if (settings.PageHeight == null) settings.PageHeight = 11.7;
                if (settings.PageWidth == null) settings.PageWidth = 8.3;
                document = new PdfDocument();
                var initPage = document.AddPage();
                initPage.Height = XUnit.FromInch(settings.PageHeight.Value);
                initPage.Width = XUnit.FromInch(settings.PageWidth.Value);
                //document should be imported so I have to reopen it
                using (MemoryStream ms = new MemoryStream())
                {
                    document.Save(ms, false);
                    document = PdfReader.Open(ms, mode);
                }
            }
            else
            {
                if (settings.Template.IsBase64())
                {
                    var pdfArr = Convert.FromBase64String(settings.Template);
                    MemoryStream pdfStream = new MemoryStream(pdfArr, 0, pdfArr.Length);
                    document = PdfReader.Open(pdfStream, mode);
                }
                else
                {
                    //var serverUtil = WorkingContext.HttpContext.Server;
                    //directory = serverUtil.MapPath($"/PDFs/{ExecutingUser.Schema}/Templates");
                    //if (!Directory.Exists(directory)) throw new DirectoryNotFoundException($"{directory} not found");
                    //string path = Path.Combine(directory, settings.Template);
                    //document = PdfReader.Open(path, mode);
                }
            }

            return document;
        }

        PdfDocument ReopenDocumentWithoutSaving(PdfDocument document, PdfDocumentOpenMode mode)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms, false);
                return PdfReader.Open(ms, mode);
            }
        }


        private List<EndYearTransactions> GroupTransactionsByFields(IGrouping<int, PdfSettingsBatchDto> groupedSetting)
        {
            return groupedSetting.Select(r => new EndYearTransactions
            {
                Amount = r.Fields.Amount,
                Date = r.Fields.DonationDate,
                DonationDateTime = r.Fields.DonationDateTime,
                Method = r.Fields.PaymentMethod,
                Number = r.Fields.CheckNo,
                Purpose = $"{r.Fields.Category}: {r.Fields.Subcategory}",
                Receipt = r.Fields.ReceiptNo
            }).OrderBy(t => t.Date).ToList();
        }

        private void ConvertMigraDocToPdfSharp(ref PdfDocument document, Document migraDocument)
        {
            var pdfRenderer = new DocumentRenderer(migraDocument);

            pdfRenderer.PrepareDocument();

            int pages = pdfRenderer.FormattedDocument.PageCount;
            var firstPage = (PdfPage)document.Pages[0].Clone();
            for (int i = 1; i <= pages; ++i)
            {
                PdfPage page = i == 1 ? document.Pages[0] : document.AddPage((PdfPage)firstPage.Clone());

                using (XGraphics gfx = XGraphics.FromPdfPage(page))
                {
                    // HACK²
                    gfx.MUH = PdfFontEncoding.Unicode;
                    // gfx.MFEH = PdfFontEmbedding.Default;

                    pdfRenderer.RenderPage(gfx, i, PageRenderOptions.All);
                }
            }
        }
    }
}