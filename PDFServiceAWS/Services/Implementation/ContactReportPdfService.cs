using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using Ninject;
using Ninject.Parameters;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Enums;
using PDFServiceAWS.Services.Implementation.PdfServiceFormats;
using PDFServiceAWS.Helpers;
using BorderStyle = MigraDoc.DocumentObjectModel.BorderStyle;
using Orientation = MigraDoc.DocumentObjectModel.Orientation;
using TabAlignment = MigraDoc.DocumentObjectModel.TabAlignment;

namespace PDFServiceAWS.Services.Implementation
{
    public class ContactReportPdfService : ReportPdfService, IContactReportPdfService
    {
        private readonly IPdfServiceGenerator _pdfService;
        private static int colsRows;

        public ContactReportPdfService(string schema)
        {
            _pdfService = NinjectBulder.Container.Get<IPdfServiceGenerator>(new ConstructorArgument("schema", schema));
        }

        public byte[] CreateDocument(object docObj)
        {
            PdfDocumentDto doc = (PdfDocumentDto) docObj;
            var reportDto = doc.ReportDto;
            var contacts = doc.Contacts;
            CountryInSettings = reportDto.Country;
            _translator = TranslateHelper.GetTranslation;
            _document = new Document { Info = { Title = reportDto.Name } };
            var cosCount = GetColCount(reportDto.Criteria);

            DefineStyles();
            if (reportDto.AdditionalPreferences.ExportType == "table")
            {
                colsRows = 0;
                CreateTablePage(reportDto.Criteria, cosCount);
                if (reportDto.Criteria.Columns.Any(r => (int)r.Column >= 9 && (int)r.Column <= 13 && r.IsChecked) &&
                    !string.Equals(reportDto.Criteria.GroupBy, "None", StringComparison.InvariantCultureIgnoreCase))
                    FillGrouped(cosCount, GroupBy(contacts, reportDto.Criteria), reportDto.Criteria);
                else
                    FillContent(contacts, reportDto.Criteria);
                AddBottom(GetTranslation("contact_report_summary"), GetTranslation("contact_number_addresses"), colsRows);
            }
            if (reportDto.AdditionalPreferences.ExportType == "labels")
            {
                CreateLabelPage();
                FillLabelRows(reportDto.Criteria, contacts);
            }
            if (reportDto.AdditionalPreferences.ExportType == "envelopes")
            {
                return FillEnvelops(contacts, reportDto.AdditionalPreferences.CustomAddress, reportDto.Criteria);
            }
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);
            pdfRenderer.Document = _document;
            pdfRenderer.RenderDocument();
            using (MemoryStream ms = new MemoryStream())
            {
                pdfRenderer.Save(ms, false);
                byte[] buffer = new byte[ms.Length];
                ms.Seek(0, SeekOrigin.Begin);
                ms.Flush();
                ms.Read(buffer, 0, (int)ms.Length);
                ms.Close();
                return buffer;
            }
        }
        #region Envelops
        byte[] FillEnvelops(List<ContactReportResultDto> contacts, string senderAddr, FilterContactReport filter)
        {
            // outputDocument = new PdfDocument { Info = { Title = filter.Name } };
            var labels = new Dictionary<int, EnvelopeAddress>();
            var conts = contacts.Where(e => e.Addresses != null && e.Addresses.Any()).ToList();
            for (int i = 0; i < conts.Count(); i++)
            {
                var contact = conts[i];
                foreach (ContactReportAddress addr in contact.Addresses)
                {


                    if (filter.ReportType == TransFilterType.Family)
                        labels.Add(i, AddEnvelopAddress(senderAddr, addr, filter.ReportType));
                    if (filter.ReportType == TransFilterType.Member)
                        CreateEnvelopsForMembers(ref labels, senderAddr, contact, addr, filter);
                }
            }
            return _pdfService.GeneratePDFEnvelopes(labels);

        }

        void CreateEnvelopsForMembers(ref Dictionary<int, EnvelopeAddress> labels, string senderAddr, ContactReportResultDto contact, ContactReportAddress addr, FilterContactReport filter)
        {

            var index = labels.Count;
            var members = contact.Members.ToList();
            for (int i = 0; i < members.Count; i++)
            {
                labels.Add(index + i, AddEnvelopAddress(senderAddr, addr, filter.ReportType, members[i].Gender));
            }
        }


        EnvelopeAddress AddEnvelopAddress(string customAddress, ContactReportAddress addr, TransFilterType addrType, string gender = null)
        {

            string senderAddress = FormAddressStr(
                GetAddrLabel(addr, addrType, gender),
                addr.CompanyName,
                addr.Address,
                addr.Address2,
                addr.City,
                addr.State,
                addr.Zip,
                addr.Country,
                CountryInSettings);
            return new EnvelopeAddress(0, customAddress, senderAddress);
        }

        private string GetAddrLabel(ContactReportAddress addr, TransFilterType addrType, string gender)
        {
            if (addrType == TransFilterType.Family) return addr.Label;
            switch (gender)
            {
                case "Male":
                    return addr.HisLabel;
                case "Female":
                    return addr.HerLabel;
            }
            return addr.Label;
        }

        #endregion

        #region Label

        void CreateLabelPage()
        {
            // Each MigraDoc document needs at least one section.
            _section = _document.AddSection();
            //_section.PageSetup = _document.DefaultPageSetup.Clone();
            var pageSize = _document.DefaultPageSetup.Clone();
            pageSize.PageFormat = PageFormat.Letter;
            pageSize.Orientation = Orientation.Portrait;
            pageSize.LeftMargin = new Unit { Inch = 0.19 };
            pageSize.RightMargin = new Unit { Inch = 0.19 };
            pageSize.TopMargin = new Unit { Inch = 0.5 };
            pageSize.BottomMargin = new Unit { Inch = 0.5 };

            _section.PageSetup = pageSize;
            _section.PageSetup.PageWidth = new Unit { Inch = 8.5 };
            _section.PageSetup.PageHeight = new Unit { Inch = 11 };

            // Create the item table
            _table = _section.AddTable();
            _table.Style = "TableLabel";

            _table.Borders.Color = Color.Empty;
            _table.Borders.Width = 0;
            //_table.Borders.Left.Width = 0.5;
            //_table.Borders.Right.Width = 0.5;
            _table.Rows.LeftIndent = 0;
            _table.Rows.HeightRule = RowHeightRule.Exactly;
            _table.Rows.Height = new Unit { Inch = 1 };
            CreateLabelColumns();
            // _table.SetEdge(0, 0, 3, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

        }

        void CreateLabelColumns()
        {
            Unit mainSize = new Unit { Inch = 2.63 };
            Unit innSize = new Unit { Inch = 0.12 };
            for (int i = 0; i < 5; i++)
            {
                Column column = _table.AddColumn();

                column.Format.Alignment = ParagraphAlignment.Center;
                column.Width = (i % 2 != 0) ? innSize : mainSize;
            }
        }

        void FillLabelRows(FilterContactReport filter, List<ContactReportResultDto> contacts)
        {
            int countCols = 0;
            Row row = _table.AddRow();
            foreach (ContactReportResultDto contact in contacts.Where(a => a.Addresses != null && a.Addresses.Any()))
            {
                if (filter.ReportType == TransFilterType.Family)
                {
                    foreach (ContactReportAddress addr in contact.Addresses)
                    {
                        countCols++;
                        if ((countCols % 2 == 0) && countCols != 0) countCols++;
                        if (countCols > 5)
                        {
                            countCols = 1;
                            row = _table.AddRow();
                        }
                        FillRow(row, countCols - 1, GetShortName(
                                string.Format("{0}\n{1}", contact.FamilyLabel ?? contact.FamilyName, GetFullAddressLine(addr, filter, true)), 132),
                            false, ParagraphAlignment.Left,
                            VerticalAlignment.Top, marginLeft: new Unit { Millimeter = 2 });
                    }
                }
                if (filter.ReportType == TransFilterType.Member)
                {
                    foreach (ContactReportAddress addr in contact.Addresses)
                    {
                        foreach (ContactReportMambers member in contact.Members)
                        {
                            countCols++;
                            if ((countCols % 2 == 0) && countCols != 0) countCols++;
                            if (countCols > 5)
                            {
                                countCols = 1;
                                row = _table.AddRow();
                            }
                            var memberName = (filter.SkipTitles ? "" : member.Title) + member.FirstName + " " + member.LastName;
                            FillRow(row, countCols - 1, GetShortName(
                                    string.Format("{0}\n{1}", memberName, GetFullAddressLine(addr, filter, true)), 132),
                                false, ParagraphAlignment.Left,
                                VerticalAlignment.Top, marginLeft: new Unit { Millimeter = 2 });
                        }
                    }
                }
            }
        }


        #endregion

        #region TableReport

        #region Initialize PDF
        int CreateColumns(FilterContactReport filter, int colsCount, Unit? sectionWidth = null)
        {
            Unit? colWidth = null;
            if (sectionWidth != null)
            {
                colWidth = sectionWidth / colsCount;
            }
            List<ReportColumn> checkedCols = new List<ReportColumn>();
            Column column = _table.AddColumn();
            //first name col
            column.Format.Alignment = ParagraphAlignment.Center;
            if (colWidth != null) column.Width = (Unit)colWidth;
            //create cols obj
            checkedCols = filter.Columns.Where(r => r.IsChecked).ToList();
            //company
            if (checkedCols.Any(e => e.Column == ReportColumns.Company))
            {
                column = _table.AddColumn();
                if (colWidth != null) column.Width = (Unit)colWidth;
                column.Format.Alignment = ParagraphAlignment.Right;
            }
            //Address
            if (checkedCols.Any(e => (int)e.Column >= 9 && (int)e.Column <= 13))
            {
                //col for address
                if (checkedCols.Any(r => r.Column == ReportColumns.Address && r.IsChecked))
                {
                    column = _table.AddColumn();
                    if (colWidth != null) column.Width = (Unit)colWidth;
                    column.Format.Alignment = ParagraphAlignment.Right;
                }
                //col for city,state,zip, country
                if (checkedCols.Any(e => (int)e.Column >= 10 && (int)e.Column <= 13 && e.IsChecked))
                {
                    column = _table.AddColumn();
                    if (colWidth != null) column.Width = (Unit)colWidth;
                    column.Format.Alignment = ParagraphAlignment.Right;
                }
            }
            //Contact data
            foreach (ReportColumn col in checkedCols.Where(e => (int)e.Column <= 8).OrderBy(e => e.Column))
            {
                column = _table.AddColumn();
                if (colWidth != null) column.Width = (Unit)colWidth;
                column.Format.Alignment = ParagraphAlignment.Right;
            }



            //fill col obj
            var startedIsnex = 1;
            Row row = _table.AddRow();
            //Name
            FillRow(row, ParagraphAlignment.Center, true, _tableHeaderBackground, 0, GetTranslation("new_report_Name"), true, ParagraphAlignment.Left, VerticalAlignment.Bottom, true, _tableHeaderFont, fontSize: 11);
            //Company
            if (checkedCols.Any(e => e.Column == ReportColumns.Company))
            {
                startedIsnex++;
                var col = checkedCols.First(e => e.Column == ReportColumns.Company);
                FillRow(row, ParagraphAlignment.Center, true, _tableHeaderBackground, 1,
                   GetTranslation(col.Name), true, ParagraphAlignment.Center, VerticalAlignment.Bottom, true, _tableHeaderFont, fontSize: 11);

            }
            //Address
            if (checkedCols.Any(e => (int)e.Column >= 9 && (int)e.Column <= 13))
            {

                var colName = "";
                var colPosition = checkedCols.Any(r => r.Column == ReportColumns.Company) ? 2 : 1;
                if (checkedCols.Any(e => e.Column == ReportColumns.Address && e.IsChecked))
                {

                    if (checkedCols.Any(e => e.Column == ReportColumns.Address && e.IsChecked))
                    {
                        colName += GetTranslation(checkedCols.First(e => e.Column == ReportColumns.Address).Name);
                        startedIsnex++;
                        FillRow(row, ParagraphAlignment.Center, true, _tableHeaderBackground, colPosition,
                            colName, true, ParagraphAlignment.Center, VerticalAlignment.Bottom, true, _tableHeaderFont,
                            fontSize: 11);
                        colPosition++;
                    }
                }
                colName = "";
                if (checkedCols.Any(e => (int)e.Column >= 10 && (int)e.Column <= 13 && e.IsChecked))
                {
                    if (checkedCols.Any(e => e.Column == ReportColumns.City && e.IsChecked))
                        colName +=
                            string.Format("{0}",
                                GetTranslation(checkedCols.First(e => e.Column == ReportColumns.City).Name));
                    if (checkedCols.Any(e => e.Column == ReportColumns.State && e.IsChecked))
                        colName +=
                            string.Format("/{0}",
                                GetTranslation(checkedCols.First(e => e.Column == ReportColumns.State).Name));
                    if (checkedCols.Any(e => e.Column == ReportColumns.Zip && e.IsChecked))
                        colName +=
                            string.Format("/{0}",
                                GetTranslation(checkedCols.First(e => e.Column == ReportColumns.Zip).Name));
                    if (checkedCols.Any(e => e.Column == ReportColumns.Country && e.IsChecked))
                        colName +=
                            string.Format("/{0}",
                                GetTranslation(checkedCols.First(e => e.Column == ReportColumns.Country).Name));
                    startedIsnex++;
                    FillRow(row, ParagraphAlignment.Center, true, _tableHeaderBackground, colPosition,
                        colName, true, ParagraphAlignment.Center, VerticalAlignment.Bottom, true, _tableHeaderFont,
                        fontSize: 11);
                }
            }
            //Contact data
            checkedCols = checkedCols.Where(e => (int)e.Column <= 8).OrderBy(e => e.Column).ToList();

            for (var i = 0; i < checkedCols.Count; i++)
            {
                var index = startedIsnex + i;
                ReportColumn col = checkedCols[i];
                FillRow(row, ParagraphAlignment.Center, true, _tableHeaderBackground, index,
                    GetTranslation(col.Name), true, ParagraphAlignment.Center, VerticalAlignment.Bottom, true, _tableHeaderFont, fontSize: 11);

            }

            var colCount = row.Cells.Count;


            return colCount;
        }

        int CreateTablePage(FilterContactReport filter, int colCount)
        {
            // Each MigraDoc document needs at least one section.
            _section = _document.AddSection();
            //_section.PageSetup = _document.DefaultPageSetup.Clone();
            var pageSize = _document.DefaultPageSetup.Clone();
            pageSize.Orientation = colCount < 5 ? Orientation.Portrait : Orientation.Landscape;
            pageSize.PageFormat = PageFormat.A4;
            pageSize.LeftMargin = new Unit { Centimeter = 1.5 };
            pageSize.RightMargin = new Unit { Centimeter = 1.5 };
            _section.PageSetup = pageSize;
            //fir page numbers
            pageSize.OddAndEvenPagesHeaderFooter = true;
            pageSize.StartingNumber = 1;

            //Paragraph paragraph = _section.Headers.Primary.AddParagraph();
            //paragraph.AddText(filter.Name);
            //paragraph.Format.Font.Size = 9;
            //paragraph.Format.Alignment = ParagraphAlignment.Center;
            //Header
            Paragraph paragrapgEven = _section.Headers.EvenPage.AddParagraph();
            paragrapgEven.AddText(GetShortName(filter.Name, 270));
            paragrapgEven.Format.Font.Size = 9;
            paragrapgEven.Format.LeftIndent = new Unit { Millimeter = 10 };
            paragrapgEven.Format.RightIndent = new Unit { Millimeter = 10 };
            paragrapgEven.Format.Alignment = ParagraphAlignment.Center;

            Paragraph paragraph = _section.Headers.Primary.AddParagraph();
            paragraph.AddText(GetShortName(filter.Name, 270));
            paragraph.Format.Font.Size = 9;
            paragraph.Format.LeftIndent = new Unit { Millimeter = 10 };
            paragraph.Format.RightIndent = new Unit { Millimeter = 10 };
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            // Create the item table
            _table = _section.AddTable();
            _table.Style = "Table";

            _table.Borders.Color = _tableBorder;
            _table.Borders.Width = 0.25;
            _table.Borders.Left.Width = 0.5;
            _table.Borders.Right.Width = 0.5;
            _table.Rows.LeftIndent = 0;
            _table.Rows.HeightRule = RowHeightRule.AtLeast;
            _table.Rows.Height = 12;
            Unit curPageWidth = new Unit { Centimeter = pageSize.Orientation == Orientation.Landscape ? 29.7 : 21 };
            Unit? pageWidth = curPageWidth - _section.PageSetup.LeftMargin - _section.PageSetup.RightMargin;

            colCount = CreateColumns(filter, colCount, pageWidth);
            _table.SetEdge(0, 0, colCount, 1, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);

            //page num
            var style = _document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop(colCount < 5 ? "9cm" : "13.3cm", TabAlignment.Center);
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;
            style.Font.Color = _fontColor;
            // Create a paragraph with centered page number. See definition of style "Footer".
            Paragraph footerParagraf = new Paragraph();
            footerParagraf.Style = StyleNames.Footer;
            footerParagraf.AddTab();
            footerParagraf.AddPageField();

            // Add paragraph to footer for odd pages.
            _section.Footers.Primary.Add(footerParagraf);
            // Add clone of paragraph to footer for odd pages. Cloning is necessary because an object must
            // not belong to more than one other object. If you forget cloning an exception is thrown.
            _section.Footers.EvenPage.Add(footerParagraf.Clone());

            return colCount;
        }

        void AddBottom(string name, string descriptionName, int count)
        {
            Paragraph totalText = _section.AddParagraph();
            totalText.AddText(string.Format("{0} \n ", name) +
                              string.Format("{0} {1}", descriptionName, count));
            totalText.Format.Font.Size = 9;
            totalText.Format.Alignment = ParagraphAlignment.Left;
            totalText.Format.LeftIndent = -20;
        }
        #endregion

        #region FillContent
        void FillGrouped(int colsCount, List<GroupedContactReport> grp, FilterContactReport filter)
        {
            foreach (GroupedContactReport grouped in grp)
            {
                Row rowHead = _table.AddRow();
                FillRow(rowHead, 0, grouped.Name, true, ParagraphAlignment.Left, VerticalAlignment.Center);
                rowHead.Cells[0].MergeRight = colsCount - 1;
                FillContent(grouped.Contacts, filter);
            }
        }
        void FillContent(List<ContactReportResultDto> contacts, FilterContactReport filter)
        {
            foreach (ContactReportResultDto contact in contacts)
            {
                if (filter.ReportType == TransFilterType.Family)
                {
                    if (filter.Columns.Any(r => (int)r.Column >= 9 && (int)r.Column <= 13 && r.IsChecked))
                        FillWithAddresses(filter, contact);
                    else
                    {
                        FillContRow(contact, filter, "", null);
                    }
                }
                if (filter.ReportType == TransFilterType.Member)
                {
                    if (filter.Columns.Any(r => (int)r.Column >= 9 && (int)r.Column <= 13 && r.IsChecked))
                        FillWithAddresses(filter, contact);
                    else
                    {
                        foreach (ContactReportMambers member in contact.Members)
                        {
                            FillContRow(member, filter, "", null);
                        }

                    }
                }
            }
        }

        private void FillWithAddresses(FilterContactReport filter, ContactReportResultDto contact)
        {

            if (filter.ReportType == TransFilterType.Family)
            {
                if (contact.Addresses.Any())
                {
                    foreach (ContactReportAddress address in contact.Addresses)
                    {
                        FillContRow(contact, filter, address.CompanyName, address);
                    }
                }
                else
                {
                    FillContRow(contact, filter, "", null);
                }
            }
            if (filter.ReportType == TransFilterType.Member)
            {

                foreach (ContactReportMambers member in contact.Members)
                {
                    if (contact.Addresses.Any())
                    {
                        foreach (ContactReportAddress address in contact.Addresses)
                        {
                            FillContRow(member, filter, address.CompanyName, address);
                        }
                    }
                    else
                    {
                        FillContRow(member, filter, "", null);
                    }
                }

            }


        }

        void FillContRow(ContactReportMambers member, FilterContactReport filter, string companyName, ContactReportAddress address)
        {
            colsRows++;
            Row row = _table.AddRow();
            var startIndex = 1;
            //fam name
            FillRow(row, 0,
                (filter.SkipTitles ? "" : member.Title) + member.FirstName + " " + member.LastName,
                false, ParagraphAlignment.Left,
                VerticalAlignment.Center);
            //company
            if (filter.Columns.Any(r => r.Column == ReportColumns.Company && r.IsChecked))
            {
                startIndex++;
                FillRow(row, 1,
                    string.IsNullOrEmpty(companyName) ? "" : companyName,
                    false, ParagraphAlignment.Left,
                    VerticalAlignment.Center);
            }
            // addresses
            if (filter.Columns.Any(e => e.Column == ReportColumns.Address && e.IsChecked))
            {
                FillRow(row, startIndex, address != null ? GetAddressCol(address) : "",
                    false, ParagraphAlignment.Left,
                    VerticalAlignment.Center);
                startIndex++;
            }
            if (filter.Columns.Any(e => (int)e.Column >= 10 && (int)e.Column <= 13 && e.IsChecked))
            {
                FillRow(row, startIndex, address != null ? GetShortAddressLine(address, filter) : "",
                    false, ParagraphAlignment.Left,
                    VerticalAlignment.Center);
                startIndex++;
            }
            FillContactsForMember(row, startIndex, member, filter);
        }

        void FillContRow(ContactReportResultDto contact, FilterContactReport filter, string companyName, ContactReportAddress address)
        {
            colsRows++;
            Row row = _table.AddRow();

            var startIndex = 1;
            //fam name
            FillRow(row, 0, contact.FamilyName,
                false, ParagraphAlignment.Left,
                VerticalAlignment.Center);
            //company
            if (filter.Columns.Any(r => r.Column == ReportColumns.Company && r.IsChecked))
            {
                startIndex++;
                FillRow(row, 1,
                    string.IsNullOrEmpty(companyName) ? "" : companyName,
                    false, ParagraphAlignment.Left,
                    VerticalAlignment.Center);
            }
            // addresses
            if (filter.Columns.Any(e => e.Column == ReportColumns.Address && e.IsChecked))
            {
                FillRow(row, startIndex, address != null ? GetAddressCol(address) : "",
                    false, ParagraphAlignment.Left,
                    VerticalAlignment.Center);
                startIndex++;
            }
            if (filter.Columns.Any(e => (int)e.Column >= 10 && (int)e.Column <= 13 && e.IsChecked))
            {
                FillRow(row, startIndex, address != null ? GetShortAddressLine(address, filter) : "",
                    false, ParagraphAlignment.Left,
                    VerticalAlignment.Center);
                startIndex++;
            }
            //contacts
            var cols = filter.Columns.Where(e => (int)e.Column >= 1 && (int)e.Column <= 8 && e.IsChecked)
                .OrderBy(q => q.Column)
                .ToList();
            for (int i = 0; i < cols.Count; i++)
            {
                var index = startIndex + i;
                var col = cols[i];
                FillRow(row, index,
                    GetContactsForFamily(contact.Members.ToList(), filter, (int)col.Column), false,
                    ParagraphAlignment.Left,
                    VerticalAlignment.Center);
            }
        }

        #endregion

        #region Grouping 
        List<GroupedContactReport> GroupBy(List<ContactReportResultDto> contacts, FilterContactReport filter)
        {
            if (filter.GroupBy == "City")
            {
                return GroupByCity(contacts);
            }

            if (filter.GroupBy == "State")
            {
                return GroupByState(contacts);
            }
            if (filter.GroupBy == "Zip")
            {
                return GroupByZip(contacts);
            }
            if (filter.GroupBy == "Country")
            {
                return GroupByCountry(contacts);
            }
            return null;
        }

        private List<GroupedContactReport> GroupByCountry(List<ContactReportResultDto> contacts)
        {
            var countries = contacts.SelectMany(e => e.Addresses.Select(r => r.Country)).Distinct().ToList();
            var resp = new List<GroupedContactReport>();
            foreach (string cont in countries)
            {
                var grp = new GroupedContactReport();
                grp.Name = string.IsNullOrEmpty(cont) ? "No country" : cont;
                grp.Contacts = contacts
                    .Where(e => e.Addresses.Any(
                        w => string.Equals(w.Country, cont, StringComparison.InvariantCultureIgnoreCase))).Select(t =>
                        {
                            return new ContactReportResultDto()
                            {
                                Addresses = null,
                                Members = t.Members.Select(r => (ContactReportMambers)r.Clone()).ToList(),
                                FamilyID = t.FamilyID,
                                FamilyName = t.FamilyName,
                                FamilySalutation = t.FamilySalutation,
                                HisSalutation = t.HisSalutation,
                                HerSalutation = t.HerSalutation,
                                FamilyLabel = t.FamilyLabel,
                                HisLabel = t.HisLabel,
                                HerLabel = t.HerLabel
                            };
                        })
                    .ToList();

                // choose only addreses with current city
                foreach (ContactReportResultDto contact in grp.Contacts)
                {
                    contact.Addresses = contacts.Where(t => t.FamilyID == contact.FamilyID).SelectMany(r => r.Addresses
                            .Where(e => string.Equals(e.Country, cont, StringComparison.InvariantCultureIgnoreCase)))
                        .ToList();
                }
                resp.Add(grp);
            }
            return resp;
        }

        private List<GroupedContactReport> GroupByZip(List<ContactReportResultDto> contacts)
        {
            var zips = contacts.SelectMany(e => e.Addresses.Select(r => r.Zip)).Distinct().ToList();
            var resp = new List<GroupedContactReport>();
            foreach (string zip in zips)
            {
                var grp = new GroupedContactReport();
                grp.Name = string.IsNullOrEmpty(zip) ? "No zip" : zip;
                grp.Contacts = contacts
                    .Where(e => e.Addresses.Any(
                        w => string.Equals(w.Zip, zip, StringComparison.InvariantCultureIgnoreCase))).Select(t =>
                        {
                            return new ContactReportResultDto()
                            {
                                Addresses = null,
                                Members = t.Members.Select(r => (ContactReportMambers)r.Clone()).ToList(),
                                FamilyID = t.FamilyID,
                                FamilyName = t.FamilyName,
                                FamilySalutation = t.FamilySalutation,
                                HisSalutation = t.HisSalutation,
                                HerSalutation = t.HerSalutation,
                                FamilyLabel = t.FamilyLabel,
                                HisLabel = t.HisLabel,
                                HerLabel = t.HerLabel
                            };
                        })
                    .ToList();

                // choose only addreses with current city
                foreach (ContactReportResultDto contact in grp.Contacts)
                {
                    contact.Addresses = contacts.Where(t => t.FamilyID == contact.FamilyID).SelectMany(r => r.Addresses
                            .Where(e => string.Equals(e.Zip, zip, StringComparison.InvariantCultureIgnoreCase)))
                        .ToList();
                }
                resp.Add(grp);
            }
            return resp;
        }

        private List<GroupedContactReport> GroupByState(List<ContactReportResultDto> contacts)
        {
            var states = contacts.SelectMany(e => e.Addresses.Select(r => r.State)).Distinct().ToList();
            var resp = new List<GroupedContactReport>();
            foreach (string state in states)
            {
                var grp = new GroupedContactReport();
                grp.Name = string.IsNullOrEmpty(state) ? "No state" : state;
                grp.Contacts = contacts
                    .Where(e => e.Addresses.Any(
                        w => string.Equals(w.State, state, StringComparison.InvariantCultureIgnoreCase))).Select(t =>
                        {
                            return new ContactReportResultDto()
                            {
                                Addresses = null,
                                Members = t.Members.Select(r => (ContactReportMambers)r.Clone()).ToList(),
                                FamilyID = t.FamilyID,
                                FamilyName = t.FamilyName,
                                FamilySalutation = t.FamilySalutation,
                                HisSalutation = t.HisSalutation,
                                HerSalutation = t.HerSalutation,
                                FamilyLabel = t.FamilyLabel,
                                HisLabel = t.HisLabel,
                                HerLabel = t.HerLabel
                            };
                        })
                    .ToList();

                // choose only addreses with current city
                foreach (ContactReportResultDto contact in grp.Contacts)
                {
                    contact.Addresses = contacts.Where(t => t.FamilyID == contact.FamilyID).SelectMany(r => r.Addresses
                            .Where(e => string.Equals(e.State, state, StringComparison.InvariantCultureIgnoreCase)))
                        .ToList();
                }
                resp.Add(grp);
            }
            return resp;
        }

        List<GroupedContactReport> GroupByCity(List<ContactReportResultDto> contacts)
        {
            var cities = contacts.SelectMany(e => e.Addresses.Select(r => r.City)).Distinct().ToList();
            var resp = new List<GroupedContactReport>();
            foreach (string city in cities)
            {
                var grp = new GroupedContactReport();
                grp.Name = string.IsNullOrEmpty(city) ? "No city" : city; ;
                grp.Contacts = contacts
                    .Where(e => e.Addresses.Any(
                        w => string.Equals(w.City, city, StringComparison.InvariantCultureIgnoreCase))).Select(t =>
                        {
                            return new ContactReportResultDto()
                            {
                                Addresses = null,
                                Members = t.Members.Select(r => (ContactReportMambers)r.Clone()).ToList(),
                                FamilyID = t.FamilyID,
                                FamilyName = t.FamilyName,
                                FamilySalutation = t.FamilySalutation,
                                HisSalutation = t.HisSalutation,
                                HerSalutation = t.HerSalutation,
                                FamilyLabel = t.FamilyLabel,
                                HisLabel = t.HisLabel,
                                HerLabel = t.HerLabel
                            };
                        })
                    .ToList();

                // choose only addreses with current city
                foreach (ContactReportResultDto contact in grp.Contacts)
                {
                    contact.Addresses = contacts.Where(t => t.FamilyID == contact.FamilyID).SelectMany(r => r.Addresses
                            .Where(e => string.Equals(e.City, city, StringComparison.InvariantCultureIgnoreCase)))
                        .ToList();
                }
                resp.Add(grp);
            }
            return resp;
        }
        #endregion
        #endregion

        #region Additional methods
        int GetColCount(FilterContactReport filter)
        {
            int resp = 1;
            //contact info cols
            resp += filter.Columns.Count(r => (int)r.Column >= 1 && (int)r.Column <= 8 && r.IsChecked);
            //address cols
            if (filter.Columns.Any(r => r.Column == ReportColumns.Address && r.IsChecked))
                resp++;
            if (filter.Columns.Any(r => (int)r.Column >= 10 && (int)r.Column <= 13 && r.IsChecked))
                resp++;
            if (filter.Columns.Any(r => r.Column == ReportColumns.Company && r.IsChecked))
                resp++;
            return resp;
        }

        /// <summary>
        /// return string with full address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        string GetFullAddressLine(ContactReportAddress address, FilterContactReport filter, bool newLineForAddres = false)
        {
            var response = "";
            if (filter.Columns.Any(e => e.Column == ReportColumns.Company) && !string.IsNullOrEmpty(address.CompanyName))
                response += address.CompanyName + "\n";
            if (filter.Columns.Any(e => e.Column == ReportColumns.Address))
                response += string.IsNullOrEmpty(address.Address) && string.IsNullOrEmpty(address.Address2) ? "" :
                    address.Address + (!string.IsNullOrEmpty(address.Address2) ? $", {address.Address2}"
                        : "") + "\n";
            if (filter.Columns.Any(e => e.Column == ReportColumns.City && e.IsChecked))
                response += $"{address.City}, ";
            if (filter.Columns.Any(e => e.Column == ReportColumns.State && e.IsChecked))
                response += $"{address.State} ";
            if (filter.Columns.Any(e => e.Column == ReportColumns.Zip && e.IsChecked))
                response += $"{address.Zip} ";
            if (filter.Columns.Any(e => e.Column == ReportColumns.Country && e.IsChecked) &&
                (string.IsNullOrEmpty(CountryInSettings) || !string.Equals(address.Country, CountryInSettings, StringComparison.InvariantCultureIgnoreCase)))
                response += $", {address.Country}";
            return response;
        }

        /// <summary>
        /// Return addres line without address
        /// </summary>
        /// <param name="address"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        static string GetShortAddressLine(ContactReportAddress address, FilterContactReport filter)
        {
            var response = "";

            if (filter.Columns.Any(e => e.Column == ReportColumns.City && e.IsChecked))
                response += $" {address.City},";
            if (filter.Columns.Any(e => e.Column == ReportColumns.State && e.IsChecked))
                response += $" {address.State}";
            if (filter.Columns.Any(e => e.Column == ReportColumns.Zip && e.IsChecked))
                response += $" {address.Zip}";
            if (filter.Columns.Any(e => e.Column == ReportColumns.Country && e.IsChecked))
                response += $", {address.Country}";
            return response;
        }

        static string GetAddressCol(ContactReportAddress address)
        {
            var addressResult = "";
            if (!string.IsNullOrEmpty(address.Address)) addressResult = address.Address;
            if (!string.IsNullOrEmpty(address.Address2))
            {
                if (!string.IsNullOrEmpty(address.Address)) addressResult += "\n";
                addressResult += address.Address2;
            }

            return addressResult;
        }

        string GetContactsForFamily(List<ContactReportMambers> members,
           FilterContactReport filter, int colId)
        {
            return members.Where(e => e.TypeID != 3)
                .Aggregate("", (current, member) => current + (GetContactStringFromList(member.Contacts.Where(r => r.PhoneTypeID == colId).ToList(), filter, member.TypeID) + "\n"));
        }

        void FillContactsForMember(Row row, int startIndex, ContactReportMambers member, FilterContactReport filter)
        {
            var cols = filter.Columns.Where(e => (int)e.Column >= 1 && (int)e.Column <= 8 && e.IsChecked).OrderBy(w => w.Column).ToList();

            for (int i = 0; i < cols.Count(); i++)
            {
                var index = startIndex + i;
                var col = cols[i];
                FillRow(row, index, GetContactStringFromList(member.Contacts.Where(r => r.PhoneTypeID == (int)col.Column).ToList(), filter, member.TypeID)
                    , false, ParagraphAlignment.Left,
                    VerticalAlignment.Center);

            }

        }

        string GetContactStringFromList(List<ContactReportContactInfo> contacts, FilterContactReport filter, int memberType)
        {
            return contacts.Aggregate("", (current, cont) => current + (GetContactLine(cont, filter, memberType) + "\n"));
        }
        string GetContactLine(ContactReportContactInfo contact, FilterContactReport filter, int memberType)
        {
            var prefix = "";
            if (memberType == 1) prefix = "His: ";
            if (memberType == 2) prefix = "Her: ";
            string isNc = contact.NoCall != null && (bool)contact.NoCall && filter.ShowNC ? "(NC)" : "";
            return $"{prefix}{isNc} {contact.PhoneNumber}";


        }

        #endregion

        List<ReportColumn> GetAddressColumnList()
        {
            return new List<ReportColumn>()
            {
                new ReportColumn()
                {
                    Column = ReportColumns.Company,
                    IsChecked = true
                },
                new ReportColumn()
                {
                    Column = ReportColumns.Address,
                    IsChecked = true
                },
                new ReportColumn()
                {
                    Column = ReportColumns.City,
                    IsChecked = true
                },
                new ReportColumn()
                {
                    Column = ReportColumns.State,
                    IsChecked = true
                },
                new ReportColumn()
                {
                    Column = ReportColumns.Zip,
                    IsChecked = true
                },
                new ReportColumn()
                {
                    Column = ReportColumns.Country,
                    IsChecked = true
                }

            };
        }
        public string FormAddressStr(string name,
            string companyName,
            string address,
            string address2,
            string city,
            string state,
            string zip,
            string country,
            string compCountry)
        {
            StringBuilder addressStr = new StringBuilder();
            addressStr.AppendLine(name);
            if (!string.IsNullOrEmpty(companyName))
                addressStr.AppendLine(companyName);
            addressStr.AppendLine($"{address} ");
            if (!string.IsNullOrEmpty(address2))
                addressStr.AppendLine($"{address2} ");

            //addressStr.Append(string.Format("{0}, {1}, {2}, {3}", city, state, zip, country));
            // addressStr.Append(country);
            if (!string.IsNullOrEmpty(city)) addressStr.Append($"{city}, ");
            if (!string.IsNullOrEmpty(state)) addressStr.Append($"{state} ");
            if (!string.IsNullOrEmpty(zip)) addressStr.Append($"{zip}");
            if (!string.IsNullOrEmpty(country) &&
                (string.IsNullOrEmpty(compCountry) || !string.Equals(country, compCountry, StringComparison.InvariantCultureIgnoreCase)))
                addressStr.Append($", {country}");
            return addressStr.ToString();
        }
    }
}