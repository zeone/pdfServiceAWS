using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Ninject;
using Ninject.Parameters;
using PDFServiceAWS.Enums;
using PDFServiceAWS.Services.Implementation.PdfServiceFormats;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public class PdfServiceGenerator : BaseService, IPdfServiceGenerator
    {
        private string _schema;
        private readonly IAppSettingsProvider _settingsProvider;
        private readonly IPdfTemplateFunctionality _pdfTemplate;
        private readonly ITransactionService _transactionService;
        private readonly ILetterService _letterService;
        private readonly IDepartmentService _departmentService;
        private readonly CustomTemplatesProvider _templatesProvider;
        private readonly IAddressService _addressService;

        public PdfServiceGenerator(IQueryProvider queryProvider, string schema)
            : base(queryProvider)
        {
            _settingsProvider = NinjectBulder.Container.Get<IAppSettingsProvider>(new ConstructorArgument("schema", schema));
            _schema = schema;
            _pdfTemplate = NinjectBulder.Container.Get<IPdfTemplateFunctionality>();
            _transactionService =
                NinjectBulder.Container.Get<ITransactionService>(new ConstructorArgument("schema", schema));
            _letterService = NinjectBulder.Container.Get<ILetterService>(new ConstructorArgument("schema", schema));
            _departmentService = NinjectBulder.Container.Get<IDepartmentService>(new ConstructorArgument("schema", schema));
            _templatesProvider = NinjectBulder.Container.Get<CustomTemplatesProvider>();
            _addressService = NinjectBulder.Container.Get<IAddressService>(new ConstructorArgument("schema", schema));
        }

        public byte[] GeneratePDFLabels(Dictionary<int, EnvelopeAddress> labels)
        {
            Avery5160Format avery5160 = new Avery5160Format();

            PdfDocument pdfDoc = new PdfDocument();
            MakePdfDocumentForLabels(pdfDoc, avery5160, labels);

            return SavePdfDocument(pdfDoc);
        }

        public byte[] GeneratePDFEnvelopes(Dictionary<int, EnvelopeAddress> envelopes)
        {
            PdfDocument pdfDoc = new PdfDocument();
            MakePdfDocumentForEnvelopes(pdfDoc, envelopes);

            return SavePdfDocument(pdfDoc);
        }

        public byte[] GeneratePDFLetters(List<string> letters)
        {
            PdfDocument pdfDoc = new PdfDocument();
            foreach (var letter in letters)
            {
                byte[] letterInBytes = ExportLetterToPdf(letter);
                using (MemoryStream stream = new MemoryStream(letterInBytes))
                {
                    PdfDocument inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.Import);
                    foreach (PdfPage page in inputDocument.Pages)
                    {
                        pdfDoc.AddPage(page);
                    }
                }
            }

            return SavePdfDocument(pdfDoc);
        }

        /// <summary>
        /// Render pdf document
        /// </summary>
        /// <param name="text">Text that should be in document</param>
        /// <param name="transactionID"></param>
        /// <returns>Path to this document</returns>
        public string PdfDoc(string text, int transactionID, string userId)
        {
            //todo change returnun path to byte array
            return string.Empty;
            //var endingFile = userId.Substring(userId.Length - 3);
            //string docName = string.Format("Doc{0}_{1}.pdf", transactionID, endingFile); //string.Format("/{0}.pdf", DateTime.Now);

            //var serverUtil = WorkingContext.HttpContext.Server;
            //string directory = serverUtil.MapPath(string.Format("/PDFs/{0}", _schema));

            //if (!Directory.Exists(directory))
            //{
            //    lock (new object())
            //    {
            //        if (!Directory.Exists(directory))
            //        {
            //            Directory.CreateDirectory(directory);
            //        }
            //    }
            //}

            //string path = Path.Combine(directory, docName);

            //byte[] bytes = ExportLetterToPdf(text);
            //if (bytes.Length == 0)
            //{
            //    return null;
            //}
            //FileStream fs = new FileStream(path, FileMode.OpenOrCreate);

            //fs.Write(bytes, 0, bytes.Length);
            //fs.Close();

            //return path;

        }

        /// <summary>
        /// Render pdf document
        /// </summary>
        /// <param name="text">Text that should be in document</param>
        /// <param name="transactionID"></param>
        /// <returns>Path to this document</returns>
        public string PdfDocWithPath(BatchLetterDto dto, string userId)
        {
            //todo change returnun path to byte array
            return string.Empty;
            //var userId = ExecutingUser.UserId.ToString();

            //var endingFile = userId.Substring(userId.Length - 3);
            //string docName = $"Doc{dto.TransactionId}_{endingFile}.pdf"; //string.Format("/{0}.pdf", DateTime.Now);

            //var serverUtil = WorkingContext.HttpContext.Server;
            //string directory = serverUtil.MapPath($"/PDFs/{ExecutingUser.Schema}");

            //if (!Directory.Exists(directory))
            //{
            //    lock (new object())
            //    {
            //        if (!Directory.Exists(directory))
            //        {
            //            Directory.CreateDirectory(directory);
            //        }
            //    }
            //}

            //string path = Path.Combine(directory, docName);

            //return PdfDocPath(dto, path);

        }

        /// <summary>
        /// Render pdf document for display in a browser
        /// </summary>
        /// <param name="textHtml">Text that should be in document</param>
        /// <returns>Bytes array encoded in Base64</returns>
        public byte[] PdfDoc(object textHtml)
        {
            var pdfByteArray = ExportLetterToPdf(textHtml.ToString());
            return EncodeBase64(pdfByteArray);
        }

        /// <summary>
        /// CreatePdf
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public byte[] PdfDoc(PdfDto dto, bool isBase64Encode = true, string currency = "$")
        {
            LetterDto letter = null;
            DepartmentDto department = null;
            var transaction = _transactionService.GetPaymentById(dto.TransactionId);
            if (dto.LetterId != null) transaction.LetterId = (short)dto.LetterId;
            if (transaction.LetterId != null) letter = _letterService.GetLetterById(transaction.LetterId.Value);
            if (letter == null) throw new Exception("Transaction don't contain letter");
            LetterFieldsDto fields = _letterService.GetLetterFields(dto.TransactionId, currency: currency);
            if (!string.IsNullOrEmpty(letter.PDFLetterText))
                letter.PDFLetterText = _templatesProvider.ReplaceFieldsTemplate(letter.PDFLetterText, fields);
            if (letter.PDFSettingID == null)
                department = _departmentService.GetDepartment(transaction.DepartmentId);

            var addresses = _addressService.GetAddressesByFamilyId(transaction.FamilyId);

            PdfSettingDto setting = PreparePdfSetting(letter, department, addresses, transaction, fields);
            var country = _settingsProvider.GetSetting(Settings.DefaultCountry).Value;
            var response = _pdfTemplate.GetPdfFromTamplate(setting, country, fields);
            if (isBase64Encode) return EncodeBase64(response);
            return response;

        }


        /// <summary>
        /// CreatePdf
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public byte[] PdfDoc(BatchLetterDto dto, bool isBase64Encode = true, string currency = "$")
        {
            LetterDto letter = null;
            DepartmentDto department = null;
            var transaction = _transactionService.GetPaymentById(dto.TransactionId);
            if (dto.LetterId != null) transaction.LetterId = (short)dto.LetterId;
            if (transaction.LetterId != null) letter = _letterService.GetLetterById(transaction.LetterId.Value);
            if (letter == null) throw new Exception("Transaction don't contain letter");
            LetterFieldsDto fields = _letterService.GetLetterFields(dto.TransactionId, currency: currency);
            if (!string.IsNullOrEmpty(letter.PDFLetterText))
                letter.PDFLetterText = _templatesProvider.ReplaceFieldsTemplate(letter.PDFLetterText, fields);
            if (letter.PDFSettingID == null)
                department = _departmentService.GetDepartment(transaction.DepartmentId);

            var addresses = _addressService.GetAddressesByFamilyId(transaction.FamilyId);

            PdfSettingDto setting = PreparePdfSetting(letter, department, addresses, transaction, fields);
            var country = _settingsProvider.GetSetting(Settings.DefaultCountry).Value;
            var response = _pdfTemplate.GetPdfFromTamplate(setting, country, fields);
            if (isBase64Encode) return EncodeBase64(response);
            return response;

        }

        public string PdfDocPath(BatchLetterDto dto, string path, string currency = "$")
        {
            LetterDto letter = null;
            DepartmentDto department = null;
            var transaction = _transactionService.GetPaymentById(dto.TransactionId);
            if (dto.LetterId != null) transaction.LetterId = (short)dto.LetterId;
            if (transaction.LetterId != null) letter = _letterService.GetLetterById(transaction.LetterId.Value);
            if (letter == null) throw new Exception("Transaction don't contain letter");
            LetterFieldsDto fields = _letterService.GetLetterFields(dto.TransactionId, currency: currency);
            if (!string.IsNullOrEmpty(letter.PDFLetterText))
                letter.PDFLetterText = _templatesProvider.ReplaceFieldsTemplate(letter.PDFLetterText, fields);
            if (letter.PDFSettingID == null)
                department = _departmentService.GetDepartment(transaction.DepartmentId);

            var addresses = _addressService.GetAddressesByFamilyId(transaction.FamilyId);

            PdfSettingDto setting = PreparePdfSetting(letter, department, addresses, transaction, fields);
            var country = _settingsProvider.GetSetting(Settings.DefaultCountry).Value;


            var response = _pdfTemplate.GetBatchPdfFromTamplate(setting, country, fields, path);
            return response;

        }

        public byte[] PdfPreview(PdfSettingDto setting, bool isBase64Encode = true, string currency = "$")
        {
            setting.Date = DateTime.Now;
            var trans = _transactionService.GetAnyPayment();
            LetterFieldsDto fields = _letterService.GetLetterFields((int)trans.TransactionId, currency: currency);
            var addresses = _addressService.GetAddressesByFamilyId(trans.FamilyId);
            MapAddressesForPdfSetting(setting, addresses.FirstOrDefault());

            //check if we have alresdy pdf template
            if ((string.IsNullOrEmpty(setting.Template) || string.IsNullOrEmpty(setting.SignatureImage)) && setting.PDFSettingID != null)
            {
                var templ = _letterService.GetPdfSetting((int)setting.PDFSettingID);
                if (string.IsNullOrEmpty(setting.Template)) setting.Template = templ.Template;
                if (string.IsNullOrEmpty(setting.SignatureImage)) setting.SignatureImage = templ.SignatureImage;
            }
            //PdfSettingDto setting = PreparePdfSetting(letter, department, addresses, transaction);
            var country = _settingsProvider.GetSetting(Settings.DefaultCountry).Value;
            var response = _pdfTemplate.GetPdfPreview(setting, country, fields);
            if (isBase64Encode) return EncodeBase64(response);
            return response;

        }
        public byte[] PdfTemplateBatch(List<PdfSettingsBatchDto> settings)
        {
            var country = _settingsProvider.GetSetting(Settings.DefaultCountry).Value;
            return _pdfTemplate.GetBatchPdfFromTemplate(settings, country);
        }

        public byte[] PdfEndYearBatch(List<PdfSettingsBatchDto> settings)
        {
            var country = _settingsProvider.GetSetting(Settings.DefaultCountry).Value;
            return _pdfTemplate.GetBatchEndYearPdfFromTemplate(settings, country);
        }

        public PdfSettingDto PreparePdfSetting(LetterDto letter, DepartmentDto department, List<FamilyAddressDto> addresses, PaymentDto transaction, LetterFieldsDto fields)
        {
            PdfSettingDto setting = letter?.PDFSettingID == null ? department.PdfSettings : letter.PdfSettings;
            if (string.IsNullOrEmpty(letter.PDFLetterText)) throw new Exception("Letter doesn't contain pdf text");
            setting.BodyText = letter.PDFLetterText;


            setting.Date = DateTime.Now;
            MapAddressesForPdfSetting(setting, addresses.FirstOrDefault(r => r.AddressId == transaction.AddressId));
            setting.Label = fields.Label;
            return setting;
        }

        #region private method

        #region Xfonts
        //TODO: get font for each pdf type from user settings
        //private static string _fontLetters = "Tahoma";
        private static string _fontLabels = "Arrial Narrow";
        private static string _fontEnvelopes = "Trebuchet MS";

        private  XFont GetLabelsXFont()
        {
            // Set font encoding to unicode
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            var font = GetSettingFonts(Settings.LabelsFont, _fontLabels);
            return new XFont(font, 0.14f, XFontStyle.Regular, options);
        }

        private  XFont GetEnvelopesXFont()
        {
            // Set font encoding to unicode
            XPdfFontOptions options = new XPdfFontOptions(PdfFontEncoding.Unicode, PdfFontEmbedding.Always);
            string font = GetSettingFonts(Settings.EnvelopesFont, _fontEnvelopes);
            return new XFont(font, 0.14f, XFontStyle.Regular, options);
        }

        private string GetSettingFonts(string fontType, string fontDefault)
        {
            var font = _settingsProvider.GetSetting(fontType);
            if (font == null || string.IsNullOrEmpty(font.Value))
            {
                AppSettingRecord setFont = new AppSettingRecord();
                // todo: maybe it would be better to gather all label/envelopes settings in specific table
                if (fontType == Settings.LabelsFont)
                    setFont.SettingID = 7;
                else if (fontType == Settings.EnvelopesFont)
                    setFont.SettingID = 8;
                else return fontDefault;
                setFont.Setting = fontType;
                setFont.Value = fontDefault;
                setFont.ParentID = null;
                _settingsProvider.CreateSetting(setFont);
                return fontDefault;
            }
            return font.Value;
        }

        #endregion 

        private byte[] EncodeBase64(byte[] bytes)
        {
            string base64 = Convert.ToBase64String(bytes, 0, bytes.Length);
            byte[] array = Encoding.ASCII.GetBytes(base64);
            return array;
        }

        private byte[] SavePdfDocument(PdfDocument pdfDoc)
        {
            byte[] fileContents;
            using (MemoryStream stream = new MemoryStream())
            {
                pdfDoc.Save(stream, true);
                fileContents = stream.ToArray();
            }
            return fileContents;
        }

        private  void MakePdfDocumentForLabels(PdfDocument pdfDoc, LabelFormat labelFormat, Dictionary<int, EnvelopeAddress> labels)
        {
            int pageCount = FindPdfDocPagesCnt(labels.Count, labelFormat.LabelsPerPage);

            int lastLabelIdxOnPage = 0;
            XFont xfont = GetLabelsXFont();
            for (int pageIdx = 0; pageIdx < pageCount; pageIdx++)
            {
                PdfPage pdfPage = CreateNewPage(pdfDoc, true, PageSizeConverter.ToSize(PdfSharp.PageSize.Letter));

                lastLabelIdxOnPage = FillPdfPageWithLabels(pdfPage, labelFormat, xfont, labels, lastLabelIdxOnPage);
            }
        }

        private  void MakePdfDocumentForEnvelopes(PdfDocument pdfDoc, Dictionary<int, EnvelopeAddress> envelopes)
        {
            EnvelopeFormat format = new EnvelopeFormat();
            XFont xfont = GetEnvelopesXFont();
            foreach (var envelope in envelopes.Values)
            {
                PdfPage pdfPage = CreateNewPage(pdfDoc, false);
                SetPageSize(pdfPage, format.Width, format.Height);
                FillPdfPageWithEnvelope(pdfPage, envelope.SenderText, envelope.RecipientText, format, xfont);
            }
        }

        private static void FillPdfPageWithEnvelope(PdfPage pdfPage, string senderAddress, string recipientAddress, EnvelopeFormat format, XFont xfont)
        {
            using (XGraphics graphicsDc = XGraphics.FromPdfPage(pdfPage, XGraphicsUnit.Inch))
            {
                //sender Address Area
                float left = format.SenderCell.Left;
                float top = format.SenderCell.Top;
                float bottom = top + format.SenderCell.Height;
                float right = left + format.SenderCell.Width;
                PdfCell returnCell = new PdfCell()
                {
                    Left = left,
                    Top = top,
                    Width = right - left,
                    Height = bottom - top,
                    TopMargin = format.SenderCell.TopMargin,
                    LeftMargin = format.SenderCell.LeftMargin
                };
                returnCell.XFont = xfont;

                if (returnCell.ShowMarkupLines)
                    PaintPdfCellLines(graphicsDc, left, right, top, bottom);
                PaintPdfCellText(graphicsDc, senderAddress, returnCell);

                //Recipient Address Area
                left = format.RecipientCell.Left;
                //top = format.RecipientCell.Top;
                top = 2;
                bottom = top + format.RecipientCell.Height;
                right = left + format.RecipientCell.Width;
                PdfCell recipientCell = new PdfCell()
                {
                    //Left = left,
                    Left = 0,
                    Top = top,
                    Width = right - left,
                    Height = bottom - top,
                    TopMargin = format.RecipientCell.TopMargin,
                    //LeftMargin = format.RecipientCell.LeftMargin
                    LeftMargin = 4
                };
                recipientCell.XFont = xfont;
                if (recipientCell.ShowMarkupLines)
                    PaintPdfCellLines(graphicsDc, left, right, top, bottom);
                PaintPdfCellText(graphicsDc, recipientAddress, recipientCell);
            }
        }

        private static void PaintPdfCellLines(XGraphics graphicsDc, float left, float right, float top, float bottom)
        {
            //XPen blackPen = new XPen(XColor.FromArgb(0, 0, 0), 0.009);
            XPen greyPen = new XPen(XColor.FromArgb(192, 192, 192), 0.009);
            //left
            graphicsDc.DrawLine(greyPen, left, bottom, left, top);
            //top
            graphicsDc.DrawLine(greyPen, left, top, right, top);
            //right
            graphicsDc.DrawLine(greyPen, right, top, right, bottom);
            // bottom
            graphicsDc.DrawLine(greyPen, left, bottom, right, bottom);
        }

        private static void PaintPdfCellText(XGraphics graphicsDc, string text, PdfCell cell)
        {
            XRect rc = new XRect(cell.Left + cell.LeftMargin, cell.Top + cell.TopMargin, cell.Width - cell.LeftMargin, cell.Height);

            XStringFormat xfmt = new XStringFormat();
            xfmt.Alignment = XStringAlignment.Near;
            XTextFormatter textFmt = new XTextFormatter(graphicsDc);
            textFmt.LayoutRectangle = rc;
            textFmt.Alignment = XParagraphAlignment.Left;
            textFmt.Font = cell.XFont;
            XSize textSz = graphicsDc.MeasureString(text, cell.XFont, xfmt);

            if (textSz.Width > rc.Width
                        || textSz.Height > rc.Height
                        || text.Contains("\r")
                        || text.Contains("\n"))
            {
                try
                {
                    textFmt.DrawString(text ?? string.Empty,
                                        cell.XFont,
                                        XBrushes.Black,
                                        rc,
                                        xfmt);
                }
                catch
                {
                    try
                    {
                        graphicsDc.DrawString(text ?? string.Empty,
                                            cell.XFont,
                                            XBrushes.Black,
                                            rc,
                                            xfmt);
                    }
                    catch { }
                }
            }
            else
            {
                graphicsDc.DrawString(text ?? string.Empty,
                                cell.XFont,
                                XBrushes.Black,
                                rc,
                                xfmt);
            }
        }

        private static PdfPage CreateNewPage(PdfDocument pdfDoc, bool portraitOrient, XSize? xsize = null)
        {
            PdfPage pdfPage = pdfDoc.AddPage();
            pdfPage.Orientation = portraitOrient
                ? PdfSharp.PageOrientation.Portrait
                : PdfSharp.PageOrientation.Landscape;

            if (xsize != null)
            {
                SetPageSize(pdfPage, xsize.Value.Width, xsize.Value.Height);
            }

            return pdfPage;
        }

        private static void SetPageSize(PdfPage pdfPage, double width, double height)
        {
            pdfPage.Width = width;
            pdfPage.Height = height;
        }

        private static int FillPdfPageWithLabels(PdfPage pdfPage, LabelFormat labelFormat, XFont xfont, Dictionary<int, EnvelopeAddress> labels, int lastLabelIdx)
        {
            string cellText;

            using (XGraphics graphicsDc = XGraphics.FromPdfPage(pdfPage, XGraphicsUnit.Inch))
            {
                float left = labelFormat.LabelCell.Left;
                float top = labelFormat.LabelCell.Top;
                float bottom;
                float right;
                float cellWidth = labelFormat.LabelCell.Width;
                float cellHeight = labelFormat.LabelCell.Height;
                float columnSpacing = labelFormat.ColumnSpacing;
                int lablelIdx = lastLabelIdx;

                for (int j = 0; j < labelFormat.CellsCntPerVertical; j++)
                {
                    if (lablelIdx > labels.Count)
                        break;

                    bottom = top + cellHeight;

                    for (int k = 0; k < labelFormat.CellsCntPerHorizontal; k++)
                    {
                        EnvelopeAddress envelope;
                        if (!labels.TryGetValue(lablelIdx++, out envelope))
                            continue;
                        cellText = envelope.RecipientText;

                        right = left + cellWidth;
                        if (labelFormat.LabelCell.ShowMarkupLines)
                            PaintPdfCellLines(graphicsDc, left, right, top, bottom);
                        float rightMargin = labelFormat.LabelCell.LeftMargin;
                        PdfCell newCell = new PdfCell()
                        {
                            Left = left,
                            Top = top,
                            Width = right - left - rightMargin,
                            Height = bottom - top,
                            LeftMargin = labelFormat.LabelCell.LeftMargin,
                            TopMargin = labelFormat.LabelCell.TopMargin
                        };
                        newCell.XFont = xfont;

                        PaintPdfCellText(graphicsDc, cellText, newCell);

                        left = right + columnSpacing;
                    }
                    top = bottom;
                    left = labelFormat.LabelCell.Left;
                }

                return lablelIdx;
            }
        }

        private byte[] ExportLetterToPdf(string html)
        {
            //recomend to use Pechkin.Synhrozine for generating pdf from html
            //recomend to use Pechkin.Synhrozine for generating pdf from html
            // byte[] bytes = new SimplePechkin(new GlobalConfig()).Convert(html);
            //IPechkin pechkin = new SynchronizedPechkin(new GlobalConfig());
            //byte[] bytes = pechkin.Convert(html);

            byte[] bytes;
            using (MemoryStream ms = new MemoryStream())
            {

                var config = new PdfGenerateConfig();
                config.PageSize = PageSize.Letter;
                // config.PageSize = PageSize.A4;
                //todo: load margins from letter user settings for pdf
                config.MarginLeft = 20;
                config.MarginRight = 20;
                config.MarginTop = 25;
                config.MarginBottom = 25;
                using (var pdf = PdfGenerator.GeneratePdf(html, config))
                {
                    if (pdf.PageCount == 0)
                    {
                        pdf.Close();
                        bytes = new byte[0];
                        return bytes;
                    }
                    pdf.Save(ms, false);
                    bytes = ms.ToArray();
                }
            }
            return bytes;
        }


        private static int FindPdfDocPagesCnt(int itemsCount, int itemsPerPage)
        {
            int pageCount = itemsCount / itemsPerPage;

            if (itemsCount % itemsPerPage > 0)
                pageCount += 1;

            return pageCount;
        }


        void MapAddressesForPdfSetting(PdfSettingDto settings, FamilyAddressDto address)
        {
            if (address == null) return;
            settings.City = address.City;
            settings.Address1 = address.Address;
            settings.Country = address.Country;
            settings.Zip = address.Zip;
            settings.State = address.State;
            //  settings.Label = address.Label;
            settings.Address2 = address.Address2;
            settings.AddressId = address.AddressId;
            settings.CompanyName = address.Company;
        }
        #endregion
    }
}