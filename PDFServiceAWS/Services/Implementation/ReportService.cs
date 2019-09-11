using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Xml;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Ninject;
using Ninject.Parameters;
using PDFServiceAWS.Enums;
using PDFServiceAWS.DB.SP;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Helpers;
//using Microsoft.Extensions.DependencyInjection;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public class ReportService : BaseService, IReportService
    {
        private string _schema;

        IEnumerable<PaymentMethodDto> _paymentMethods;
        IEnumerable<SolicitorDto> _solicitors;
        IEnumerable<MailingDto> _mailings;
        IEnumerable<DepartmentDto> _departments;
        IEnumerable<CategoryDto> _categoryTree;


        public readonly IMapper mapper;
        private readonly IXMLService _xmlService;
        private readonly IReportGroupingService _groupingService;
        private IPaymentMethodService _paymentService;
        private ISolicitorService _solicitorService;
        private IMailingService _mailingService;
        private IDepartmentService _departmentService;
        private IGroupingService _catService;
        private readonly ITransactionReportPdfService _transPdfService;
        private readonly IContactReportPdfService _contactPdfService;

        public ReportService(IQueryProvider queryProvider, string schema) : base(queryProvider)
        {
            _schema = schema;
            _groupingService = NinjectBulder.Container.Get<IReportGroupingService>(new ConstructorArgument("schema", schema));
            _transPdfService = NinjectBulder.Container.Get<ITransactionReportPdfService>(new ConstructorArgument("schema", schema));
            _contactPdfService = NinjectBulder.Container.Get<IContactReportPdfService>(new ConstructorArgument("schema", schema));
            _xmlService = new XMLService();
            mapper = new MapperConfiguration(e =>
            {
                e.CreateMap<FilterTransactionReport, FilterContactReport>();
                e.CreateMap<FilterTransactionReport, SelectTransactionsReportStoredProcedure>()
                    .ConstructUsing(
                        r => QueryProvider.CreateQuery<SelectTransactionsReportStoredProcedure>(schema))
                    .ForMember(t => t.MinAmount, r => r.MapFrom(w => w.MinSum))
                .ForMember(y => y.TransactionStartDate, t => t.MapFrom(w => w.TransactionStartDate == null ? w.TransactionStartDate : ((DateTime)w.TransactionStartDate).Date))
                .ForMember(y => y.TransactionEndDate, t => t.MapFrom(w => w.TransactionEndDate == null ? w.TransactionEndDate : ((DateTime)w.TransactionEndDate).Date))
                .ForMember(y => y.DateDueFrom, t => t.MapFrom(w => w.DateDueFrom == null ? w.DateDueFrom : ((DateTime)w.DateDueFrom).Date))
                .ForMember(y => y.DateDueTo, t => t.MapFrom(w => w.DateDueTo == null ? w.DateDueTo : ((DateTime)w.DateDueTo).Date));
                e.CreateMap<FilterContactReport, SelectContactReportStoredProcedure>()
                    .ForMember(q => q.TransactionSort, g => g.MapFrom(t => (int)t.TransactionSort))
                    .ForMember(s => s.ReportType, x => x.MapFrom(p => (int)p.ReportType))
                    .ConstructUsing(
                        x => QueryProvider.CreateQuery<SelectContactReportStoredProcedure>(schema));
            }).CreateMapper();
        }
        void GetFamiliesIds(FilterTransactionReport filter, bool useAddress = true)
        {
            var contactsCriterias = mapper.Map<FilterTransactionReport, FilterContactReport>(filter);
            contactsCriterias.TransSubCatIDs = null;
            contactsCriterias.MaxSum = null;
            contactsCriterias.MinSum = null;
            contactsCriterias.TransactionStartDate = null;
            contactsCriterias.TransactionEndDate = null;
            if (!useAddress)
            {
                contactsCriterias.Country = string.Empty;
                contactsCriterias.City = string.Empty;
                contactsCriterias.State = string.Empty;
                contactsCriterias.Zip = null;
            }
            List<int> familiesIds =
                FilterContactReport(contactsCriterias)
                    .GroupBy(r => r.FamilyID).Select(w => w.Key).ToList();
            if (filter.FamilyIds == null) filter.FamilyIds = new List<int>();
            if (familiesIds.Any())
            {
                //if we have some families by other filters we have to use it like "AND'
                //when FamilyOtherCriteria is true. it mean that we will select transaction with families lie "OR"
                filter.FamilyOtherCriteria = false;
                filter.FamilyIds.AddRange(familiesIds);
            }
        }


        public byte[] GetTransactionPdf(object filterObj)
        {
            FilterTransactionReport filter = (FilterTransactionReport)filterObj;
            if (filter.HasFamilyCriterias)
                GetFamiliesIds(filter);
            var transDto = FilterTransactionReport(filter);
            FillCollections(filter, ((TransFilterView)filter.view == TransFilterView.Total));
            _groupingService.InitializeCollections(Startup.GetTranslation, _paymentMethods, _solicitors, _mailings, _departments, _categoryTree);
            TransactionGrouped grouped = !string.Equals(filter.totalOnlyBy, "totalOnly", StringComparison.InvariantCultureIgnoreCase)
                ? _groupingService.TransactionTotalOnlyBy(transDto, filter) : _groupingService.TotalBy(transDto, filter);
            _transPdfService.InitializeCollections(Startup.GetTranslation, _paymentMethods, _solicitors, _mailings, _departments, _categoryTree);
            return _transPdfService.CreateDocument(new PdfDocumentDto { Filter = filter, Grouped = grouped, CountTrans = transDto.Transactions.Count });
        }

        public byte[] GetContactPdf(object reportDtoObj)
        {
            ReportDto reportDto = (ReportDto)reportDtoObj;
            var contactDto = FilterContactReport(reportDto.Criteria).OrderBy(d => d.FamilyName).ToList();
            if (reportDto.Criteria.Columns != null && reportDto.Criteria.Columns.Any(r => r.Filter != ReportColumnsFilter.All))
                contactDto = FilterContactReportByColumns(reportDto.Criteria, contactDto.ToList()).ToList();
            if (reportDto.AdditionalPreferences.ExportType != "labels" &&
                reportDto.AdditionalPreferences.ExportType != "envelopes")
                reportDto.Country = string.Empty;

            return _contactPdfService.CreateDocument(new PdfDocumentDto { ReportDto = reportDto, Contacts = contactDto.ToList() });
        }

        /// <summary>
        /// Use for contact report
        /// </summary>
        /// <param name="filter">object with params for filtering</param>
        /// <returns>List of filtered contacts</returns>
        public IList<ContactReportResultDto> FilterContactReport(FilterContactReport filter)
        {
            var queryMember = mapper.Map<FilterContactReport, SelectContactReportStoredProcedure>(filter);
            var resultq = queryMember.Execute();
            IList<ContactReportResultDto> resp = new List<ContactReportResultDto>();
            return !resultq.HasNoDataRows ? ConvertResultToContactReport(resultq.ResultToArray<ContactReportDto>().ToList()) : resp;

        }

        /// <summary>
        /// Use for transaction report
        /// </summary>
        /// <param name="filter">object with params for filtering</param>
        /// <returns>Return object with filtered transactions and families</returns>
        public TransactionReportResultDto FilterTransactionReport(FilterTransactionReport filter)
        {
            //hardcoded to false, in case if we need use families in other criteria like OR just remove this string 
            filter.FamilyOtherCriteria = false;

            var queryMember = mapper.Map<FilterTransactionReport, SelectTransactionsReportStoredProcedure>(filter);
            var resultqr = queryMember.Execute();
            var result = new TransactionReportResultDto();
            if (!resultqr.HasNoDataRows) ConvertResultToTransactionReport(result, resultqr.ResultToArray<TransactionsReportDto>().ToList(), filter.ReportType, filter);
            return result;
        }

        #region private methods

        /// <summary>
        /// Convert response from database and returl list of ContactReportResultDto
        /// </summary>
        /// <param name="rep">List objects from DB</param>
        /// <returns></returns>
        IList<ContactReportResultDto> ConvertResultToContactReport(IList<ContactReportDto> rep)
        {
            var fIds = rep.Select(e => e.FamilyID).Distinct();
            var result = new List<ContactReportResultDto>();
            foreach (int fId in fIds)
            {
                var famRows = rep.Where(r => r.FamilyID == fId);
                var famiyInfo = famRows.FirstOrDefault();
                result.Add(new ContactReportResultDto()
                {
                    FamilyID = famiyInfo.FamilyID,
                    FamilyName = famiyInfo.FamilyName,
                    FamilySalutation = famiyInfo.FamilySalutation,
                    HerSalutation = famiyInfo.HerSalutation,
                    HisSalutation = famiyInfo.HisSalutation,
                    FamilyLabel = famiyInfo.FamilyLabel,
                    HisLabel = famiyInfo.HisLabel,
                    HerLabel = famiyInfo.HerLabel,
                    Addresses = famRows.GroupBy(e => e.AddressID).Select(r => r.FirstOrDefault()).Select(s => new ContactReportAddress()
                    {
                        City = s.City,
                        Address = s.Address,
                        State = s.State,
                        Zip = s.Zip,
                        Country = s.Country,
                        CompanyName = s.CompanyName,
                        AddressID = s.AddressID,
                        AddrCurrent = s.AddrCurrent,
                        AddrNoMail = s.AddrNoMail,
                        AddrPrimary = s.AddrPrimary,
                        Address2 = s.Address2,
                        AddressTypeID = s.AddressTypeID,
                        SalSetting = s.SalSetting,
                        Label = s.FamilyLabel,
                        HisLabel = s.HisLabel,
                        HerLabel = s.HerLabel
                    }).ToList(),
                    Members = famRows.GroupBy(m => m.MemberID).Select(e => e.First()).Select(e => new ContactReportMambers()
                    {
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        MemberID = e.MemberID,
                        Title = e.Title,
                        TypeID = e.TypeID,
                        HebrewFather = e.HebrewFather,
                        HebrewMother = e.HebrewMother,
                        HebrewName = e.HebrewName,
                        Gender = e.Gender,
                        Contacts = famRows.Where(t => t.MemberID == e.MemberID).Select(w => new ContactReportContactInfo()
                        {
                            IsPrimary = w.IsPrimary,
                            NoCall = w.NoCall,
                            PhoneNumber = w.PhoneNumber,
                            PhoneTypeID = w.PhoneTypeID
                        }).GroupBy(r => r.PhoneNumber).Select(c => c.First()).ToList()

                    }).GroupBy(r => r.MemberID).Select(c => c.First()).ToList()
                });
            }
            return result;
        }

        /// <summary>
        /// Convert respose from db and convert to transaction report
        /// </summary>
        /// <param name="result">ref obj result</param>
        /// <param name="response">response from db</param>
        /// <param name="reportType">Report type: 1 - payment, 2 - bill</param>
        /// <param name="filter"></param>
        void ConvertResultToTransactionReport(TransactionReportResultDto result, IList<TransactionsReportDto> response, TransFilterType reportType, FilterTransactionReport filter)
        {
            List<TransactionsReportList> unassignedpayments;
            List<int> categoriesIds;
            var familiesIds = response.Select(r => r.FamilyID).Distinct().ToList();
            result.Families = FillFamilies(familiesIds, response);
            result.Transactions = FillTransactions(response, reportType, out unassignedpayments, out categoriesIds, filter).ToList();
            result.UnassignedPayments = unassignedpayments;
            result.CategoriesIDs = categoriesIds;
            if (filter.ExcludeBillsWithCards != null && (bool)filter.ExcludeBillsWithCards)
                result.Transactions = ExcludeAutoBills(result.Transactions);
        }

        /// <summary>
        /// Cuild list for families
        /// </summary>
        /// <param name="familiesIds">list of families ids</param>
        /// <param name="response">Response from database</param>
        /// <returns>List of families</returns>
        List<TransactionReportFamily> FillFamilies(List<int> familiesIds, IList<TransactionsReportDto> response)
        {
            var result = new List<TransactionReportFamily>();
            foreach (int familyId in familiesIds)
            {
                var famRows = response.Where(r => r.FamilyID == familyId).ToList();
                var firstFamRow = famRows.First();
                var family = new TransactionReportFamily
                {
                    FamilyID = firstFamRow.FamilyID,
                    Family = firstFamRow.Family,
                    AddressID = firstFamRow.AddressID,
                    FamilySalutation = firstFamRow.FamilySalutation,
                    HisSalutation = firstFamRow.HisSalutation,
                    HerSalutation = firstFamRow.HerSalutation,
                    FamilyLabel = firstFamRow.FamilyLabel,
                    HisLabel = firstFamRow.HisLabel,
                    HerLabel = firstFamRow.HerLabel,
                    Contacts = FillContacts(famRows),
                    Addresses = FillAddress(famRows)
                };
                family.Company = family.Addresses.Any(e => string.IsNullOrEmpty(e.CompanyName))
                    ? family.Addresses.FirstOrDefault(e => string.IsNullOrEmpty(e.CompanyName)).CompanyName
                    : string.Empty;
                result.Add(family);
            }
            return result;
        }

        /// <summary>
        /// Build contacts list for family
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        IList<ContactReportContactInfo> FillContacts(IList<TransactionsReportDto> rows)
        {
            var contacts = new List<ContactReportContactInfo>();
            foreach (TransactionsReportDto row in rows)
            {
                if (contacts.Any(e => e.IsPrimary == row.IsPrimary && e.NoCall == row.NoCall
                                                                   && string.Equals(e.PhoneNumber, row.PhoneNumber) && e.PhoneTypeID == row.PhoneTypeID && e.MemberType == row.MemberType)) continue;
                contacts.Add(new ContactReportContactInfo
                {
                    PhoneTypeID = row.PhoneTypeID,
                    PhoneNumber = row.PhoneNumber,
                    NoCall = row.NoCall,
                    IsPrimary = row.IsPrimary,
                    MemberType = row.MemberType
                });
            }
            return contacts;
        }

        /// <summary>
        /// Build address list for family
        /// </summary>
        /// <param name="rows">Row from database sorted obly for one faily</param>
        /// <returns></returns>
        IList<ContactReportAddress> FillAddress(IList<TransactionsReportDto> rows)
        {
            var addresses = new List<ContactReportAddress>();
            addresses.AddRange(rows.GroupBy(r => r.AddressID).Select(e => e.First()).Select(w => new ContactReportAddress
            {
                State = w.State,
                Zip = w.Zip,
                Address = w.Address,
                AddressID = w.AddressID,
                AddrCurrent = w.AddrCurrent,
                Country = w.Country,
                City = w.City,
                CompanyName = w.CompanyName,
                AddrNoMail = w.AddrNoMail,
                AddressTypeID = w.AddressTypeID,
                AddrPrimary = w.AddrPrimary,
                Address2 = w.Address2,
                Salutation = w.FamilySalutation,
                Label = w.FamilyLabel
            }).Distinct().ToList());
            return addresses;
        }


        /// <summary>
        /// Get rows from database and build correct structure for it
        /// </summary>
        /// <param name="response">Response from database</param>
        /// <param name="reportType">0 - bill 1 - payment, 2 - payment/bills </param>
        /// <param name="unassignedPayments">out params in case if reportType 2 we create onother one list for payments which not assigned to bills</param>
        /// <param name="categoriesIds">Out param for contain categories list</param>
        /// <returns>Return correct list of object for transactions</returns>
        List<TransactionsReportList> FillTransactions(IList<TransactionsReportDto> response, TransFilterType reportType,
            out List<TransactionsReportList> unassignedPayments, out List<int> categoriesIds, FilterTransactionReport filter)
        {
            bool usePreview = false;
            var previewCountRows = 0;
            if (filter.CountRows != null)
            {
                usePreview = true;
                previewCountRows =
                    Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ReportFeContRows"]);
            }
            unassignedPayments = new List<TransactionsReportList>();
            categoriesIds = new List<int>();
            var sorted = new List<TransactionsReportList>();
            // complette list of reports
            sorted.AddRange(response.GroupBy(e => e.TransactionID).Select(t => t.First()).Select(q =>
            {
                var details = PrepareTransactionsDetails(q.BillDetails, q.PaymentDetails, filter);
                return new TransactionsReportList
                {
                    Amount = filter.TransSubCatIDs == null || !filter.TransSubCatIDs.Any() ? q.Amount : details.Sum(r => r.Amount),
                    BillID = q.BillID,
                    TransType = q.TransType,
                    FamilyID = q.FamilyID,
                    AddressID = q.AddressID,
                    AuthTransactionId = q.AuthTransactionId,
                    AuthNumber = q.AuthNumber,
                    TransactionID = q.TransactionID,
                    AmountDue = q.AmountDue,
                    CheckNo = q.CheckNo,
                    Date = q.Date,
                    DepartmentID = q.DepartmentID,

                    Details = details,
                    HonorMemory = q.HonorMemory,
                    InvoiceNo = q.InvoiceNo,
                    IsReceipt = q.IsReceipt,
                    LetterID = q.LetterID,
                    MailingID = q.MailingID,
                    Note = q.Note,
                    PayID = q.PayID,
                    PaymentMethodID = q.PaymentMethodID,
                    ReceiptNo = q.ReceiptNo,
                    ReceiptSent = q.ReceiptSent,
                    SolicitorID = q.SolicitorID
                };
            }));

            CalculeteAmountDueForTransactionDetails(sorted);
            //fill categoriesIds
            foreach (List<TransactionDetailReportList> details in sorted.Select(e => e.Details))
            {
                foreach (TransactionDetailReportList det in details)
                {
                    if (det.CategoryID != null && !categoriesIds.Contains(det.CategoryID.Value)) categoriesIds.Add(det.CategoryID.Value);
                }
            }
            // if we have only payment or bill type so we no need to make some addition 
            if (reportType != TransFilterType.Bill) return sorted;

            //create list of payments without wich unasignet to bill
            foreach (TransactionsReportList payment in sorted.Where(r => r.TransType == 1))
            {
                if (payment.Details.All(e => e.BillID != null)) continue;
                unassignedPayments.Add(payment);
                var tmpPayment = unassignedPayments.First(e => e.TransactionID == payment.TransactionID);
                if (tmpPayment.Details.Any(w => w.BillID != null))
                {
                    tmpPayment.Details = payment.Details.Where(r => r.BillID == null).ToList();
                }
            }
            var result = new List<TransactionsReportList>();
            List<int> allowedFamList = usePreview
                ? GetAllowedFamiliesIds(sorted.Where(r => r.TransType == 2), previewCountRows)
                    : sorted.Select(e => e.FamilyID).Distinct().ToList();
            // add to bils detail payment detail
            foreach (TransactionsReportList report in sorted.Where(r => allowedFamList.Any(s => s == r.FamilyID) && r.TransType == 2))
            {
                result.Add(new TransactionsReportList
                {
                    TransactionID = report.TransactionID,
                    FamilyID = report.FamilyID,
                    BillID = report.BillID,
                    AddressID = report.AddressID,
                    InvoiceNo = report.InvoiceNo,
                    Date = report.Date,
                    Amount = report.Amount,
                    AmountDue = report.AmountDue,
                    CheckNo = report.CheckNo,
                    TransType = report.TransType,
                    PayID = report.PayID,
                    AuthNumber = report.AuthNumber,
                    PaymentMethodID = report.PaymentMethodID,
                    IsReceipt = report.IsReceipt,
                    ReceiptNo = report.ReceiptNo,
                    ReceiptSent = report.ReceiptSent,
                    LetterID = report.LetterID,
                    SolicitorID = report.SolicitorID,
                    DepartmentID = report.DepartmentID,
                    MailingID = report.MailingID,
                    HonorMemory = report.HonorMemory,
                    Note = report.Note,
                    AuthTransactionId = report.AuthTransactionId,
                    Details = report.Details

                });
                foreach (List<TransactionDetailReportList> detailReportLists in sorted.Where(p => p.TransType == 1).Select(r => r.Details))
                {
                    if (detailReportLists.Any(e => e.BillID == report.BillID))
                        result.First(e => e.TransactionID == report.TransactionID).Details.AddRange(detailReportLists.Where(e => e.BillID == report.BillID));
                    // report.Details.AddRange(detailReportLists.Where(e => e.BillID == report.BillID));
                }
            }

            //return only bills which contains also payment details
            return result;
        }

        /// <summary>
        /// Parse transaction details from XML
        /// </summary>
        /// <param name="billDetails">Bill details XML string</param>
        /// <param name="paymentDetails">Payment details XML string</param>
        /// <returns></returns>
        private List<TransactionDetailReportList> PrepareTransactionsDetails(string billDetails, string paymentDetails, FilterTransactionReport filter)
        {
            //parse dateils from string
            List<TransactionDetailReportList> details;
            details = string.IsNullOrEmpty(billDetails) && string.IsNullOrEmpty(paymentDetails)
               ? new List<TransactionDetailReportList>()
               : ParseTransactionDetails(string.IsNullOrEmpty(billDetails) ? paymentDetails : billDetails
                       , string.IsNullOrEmpty(billDetails) ? "Payment" : "Bill")
                   .ToList();
            if (!details.Any()) return details;
            if (filter.TransSubCatIDs != null && filter.TransSubCatIDs.Any())
                details = filter.ExByTransSubCat == null || (filter.ExByTransSubCat != null && (bool)!filter.ExByTransSubCat) ?
                    details.Where(r => filter.TransSubCatIDs.Any(t => t == r.SubcategoryID)).ToList() :
                    details.Where(r => filter.TransSubCatIDs.All(t => t != r.SubcategoryID)).ToList();
            details = details.GroupBy(q => new { q.CategoryID, q.SubcategoryID }).Select(w => new TransactionDetailReportList
            {
                Amount = w.Sum(r => r.Amount),
                //in case if haven't any payment for bill
                AmountDue = w.Sum(r => r.Amount),
                TransType = w.First().TransType,
                ClassID = w.First().ClassID,
                SubclassID = w.First().SubclassID,
                SubcategoryID = w.First().SubcategoryID,
                CategoryID = w.First().CategoryID,
                BillID = w.First().BillID,
                CardCharged = w.First().CardCharged,
                Category = w.First().Category,
                Class = w.First().Category,
                DateDue = w.First().DateDue,
                Note = w.First().Note,
                Quantity = w.First().Quantity,
                Subcategory = w.First().Subcategory,
                Subclass = w.First().Subclass,
                TransactionDetailID = w.First().TransactionDetailID,
                UnitPrice = w.First().UnitPrice
            }).ToList();


            return details;
        }

        /// <summary>
        /// Convert xml string with transactions detail to object
        /// </summary>
        /// <param name="xmlString">xml string for payment or bill</param>
        /// <param name="nodeName">could be "Payment" or "Bill" </param>
        /// <returns>List of transaction detail objects</returns>
        IList<TransactionDetailReportList> ParseTransactionDetails(string xmlString, string nodeName)
        {

            var details = new List<TransactionDetailReportList>();
            var nodes = _xmlService.GetXMLElements(nodeName, xmlString);
            string transDetId;
            string billId;
            string catId;
            string subCatId;
            string amount;
            string category;
            string subcategory;
            string datedue;
            string quantity;
            string unitPrice;
            string cardCharged;
            string classId;
            string subclassId;
            var culturePointer = CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator;

            foreach (XmlElement node in nodes)
            {
                transDetId = CheckXmlValue(_xmlService.GetTransactionDetailsIds(node.OuterXml));
                billId = CheckXmlValue(_xmlService.GetBillIDs(node.OuterXml));
                catId = CheckXmlValue(_xmlService.GetCategoryID(node.OuterXml));
                classId = CheckXmlValue(_xmlService.GetClassId(node.OuterXml));
                subclassId = CheckXmlValue(_xmlService.GetSubclassId(node.OuterXml));
                subCatId = CheckXmlValue(_xmlService.GetSubcategoryID(node.OuterXml));
                amount = CheckXmlValue(_xmlService.GetAmount(node.OuterXml));
                category = CheckXmlValue(_xmlService.GetCategory(node.OuterXml));
                subcategory = CheckXmlValue(_xmlService.GetSubcategory(node.OuterXml));
                datedue = CheckXmlValue(_xmlService.GetDateDue(node.OuterXml));
                quantity = CheckXmlValue(_xmlService.GetQuantity(node.OuterXml));
                unitPrice = CheckXmlValue(_xmlService.GetUnitPrice(node.OuterXml));
                cardCharged = CheckXmlValue(_xmlService.GetCardCharged(node.OuterXml));
                var det = new TransactionDetailReportList();
                det.TransactionDetailID = string.IsNullOrEmpty(transDetId) ? default(int?) : Convert.ToInt32(transDetId);
                det.BillID = string.IsNullOrEmpty(billId) ? default(int?) : Convert.ToInt32(billId);
                det.CategoryID = string.IsNullOrEmpty(catId) ? default(int?) : Convert.ToInt32(catId);
                det.SubcategoryID = string.IsNullOrEmpty(subCatId) ? default(int?) : Convert.ToInt32(subCatId);
                det.Amount = string.IsNullOrEmpty(amount) ? default(decimal) : decimal.Parse(amount.Replace(",", culturePointer), CultureInfo.InvariantCulture);
                det.Category = category;
                det.Subcategory = subcategory;
                det.ClassID = string.IsNullOrEmpty(classId) ? default(int?) : Convert.ToInt32(classId);
                det.SubclassID = string.IsNullOrEmpty(subclassId) ? default(int?) : Convert.ToInt32(subclassId);
                det.TransType = string.Equals(nodeName, "Payment") ? 1 : 2;
                if (nodeName == "Bill")
                {
                    det.DateDue = string.IsNullOrEmpty(datedue) ? default(DateTime?) : DateTime.Parse(datedue);
                    det.Quantity = string.IsNullOrEmpty(quantity) ? default(int?) : Convert.ToInt32(quantity);
                    det.UnitPrice = string.IsNullOrEmpty(unitPrice) ? default(decimal?) : decimal.Parse(unitPrice.Replace(",", culturePointer), CultureInfo.InvariantCulture);
                    det.CardCharged = string.IsNullOrEmpty(cardCharged) ? default(bool?) : string.Equals(cardCharged, "1");
                }
                details.Add(det);

            }
            return details;
        }

        string CheckXmlValue(XmlNodeList node)
        {
            if (node.Count > 0)
                return node[0].InnerText;
            return string.Empty;
        }

        /// <summary>
        /// Calculate and fill amount due for details onsode transaction
        /// </summary>
        /// <param name="list"></param>
        private void CalculeteAmountDueForTransactionDetails(List<TransactionsReportList> list)
        {
            //firs grouped by family
            foreach (var trans in list.GroupBy(r => r.FamilyID).Select(e => e.Select(t => t)).ToList())
            {
                if (!trans.Any(e => e.TransType == 1) || !trans.Any(e => e.TransType == 2)) continue;
                //get all transaction grouped by bill id
                var transBils = trans.Where(r => r.TransType == 2 && r.BillID != null)
                    .GroupBy(t => t.BillID)
                    .Select(e => e.Select(q => q)).ToList();
                foreach (var trBill in transBils)
                {
                    int? billId = trBill.First().BillID;
                    if (billId == null || trans.Where(w => w.TransType == 1).Any(r => r.Details.All(e => e.BillID != billId))) continue;
                    foreach (TransactionsReportList tr in trBill)
                    {
                        foreach (TransactionDetailReportList detail in tr.Details)
                        {
                            var payment = trans.Where(e => e.TransType == 1 &&
                                                          e.Details.Any(
                                                              w => w.TransType == 1 &&
                                                              w.BillID == detail.BillID &&
                                                                   w.CategoryID == detail.CategoryID &&
                                                                   w.SubcategoryID == detail.SubcategoryID))
                                .Select(q => q.Details.FirstOrDefault(t => t.TransType == 1 && t.CategoryID == detail.CategoryID &&
                                t.SubcategoryID == detail.SubcategoryID)).ToList();
                            if (!payment.Any()) continue;
                            detail.AmountDue = detail.Amount + payment.First().Amount;
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Will return only families which will get full transaction list in preview
        /// </summary>
        /// <param name="transactions"></param>
        /// <returns></returns>
        private List<int> GetAllowedFamiliesIds(IEnumerable<TransactionsReportList> transactions, int previewCountRows)
        {
            var leftCount = previewCountRows;
            var families = transactions.GroupBy(e => e.FamilyID);
            List<int> resp = new List<int>();
            foreach (var family in families)
            {
                var count = family.Count();
                if (count <= previewCountRows && count <= leftCount)
                {
                    resp.Add(family.Key);
                    leftCount -= count;
                }
            }

            return resp;
        }

        /// <summary>
        /// if bill has a card and date(date due) are larger than current date, this transaction will be skip
        /// </summary>
        /// <param name="resultTransactions"></param>
        /// <returns></returns>
        private List<TransactionsReportList> ExcludeAutoBills(List<TransactionsReportList> resultTransactions)
        {
            var res = new List<TransactionsReportList>();
            foreach (var trans in resultTransactions)
            {
                if (trans.TransType == 2 && trans.PayID != null && trans.Date > DateTime.Now &&
                    trans.Details.Any(e => e.TransType == 2 && e.DateDue > DateTime.Now)) continue;
                res.Add(trans);
            }
            return res;
        }

        /// <summary>
        /// Check seected columns and fill collection if need
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="loadAll">Use for filling all collections< moustli useed for csv</param>
        private void FillCollections(FilterTransactionReport filter, bool loadAll = false)
        {
            if (filter.Columns.Any(e => e.TransactionColumn == TransactioReportColumns.Method && e.IsChecked) || loadAll
                || string.Equals(filter.subtotalBy, "Method", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(filter.GroupBy, "Method", StringComparison.InvariantCultureIgnoreCase))
            {
                if (_paymentService == null) _paymentService = NinjectBulder.Container.Get<IPaymentMethodService>(new ConstructorArgument("schema", _schema));
                _paymentMethods = _paymentService.GetAllPaymentMethods();
            }
            if (filter.Columns.Any(e => e.TransactionColumn == TransactioReportColumns.Solicitor && e.IsChecked) || loadAll
                || string.Equals(filter.subtotalBy, "Solicitor", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(filter.GroupBy, "Solicitor", StringComparison.InvariantCultureIgnoreCase))
            {
                if (_solicitorService == null) _solicitorService = NinjectBulder.Container.Get<ISolicitorService>(new ConstructorArgument("schema", _schema));
                _solicitors = _solicitorService.GetAllSolicitors();
            }
            if (filter.Columns.Any(e => e.TransactionColumn == TransactioReportColumns.Mailing && e.IsChecked) || loadAll
                || string.Equals(filter.subtotalBy, "Mailing", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(filter.GroupBy, "Mailing", StringComparison.InvariantCultureIgnoreCase))
            {
                if (_mailingService == null) _mailingService = NinjectBulder.Container.Get<IMailingService>(new ConstructorArgument("schema", _schema));
                _mailings = _mailingService.GetAllMailings();
            }
            if (filter.Columns.Any(e => e.TransactionColumn == TransactioReportColumns.Department && e.IsChecked) || loadAll
                || string.Equals(filter.subtotalBy, "Depatrment", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(filter.GroupBy, "Depatrment", StringComparison.InvariantCultureIgnoreCase))
            {
                if (_departmentService == null) _departmentService = NinjectBulder.Container.Get<IDepartmentService>(new ConstructorArgument("schema", _schema));
                _departments = _departmentService.GetAllDepartments();
            }
            if (filter.Columns.Any(e => e.TransactionColumn == TransactioReportColumns.CatSubcat && e.IsChecked) || loadAll
                || string.Equals(filter.subtotalBy, "Subcategory", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(filter.GroupBy, "Subcategory", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(filter.subtotalBy, "Category", StringComparison.InvariantCultureIgnoreCase)
                || string.Equals(filter.GroupBy, "Category", StringComparison.InvariantCultureIgnoreCase))
            {
                if (_catService == null) _catService = NinjectBulder.Container.Get<IGroupingService>(new ConstructorArgument("schema", _schema));
                _categoryTree = _catService.GetCategoryTree();
            }
        }

        public IList<ContactReportResultDto> FilterContactReportByColumns(FilterContactReport filters, List<ContactReportResultDto> data)
        {

            var result = new List<ContactReportResultDto>();
            foreach (ContactReportResultDto fam in data)
            {
                var newContact = new ContactReportResultDto
                {
                    FamilyID = fam.FamilyID,
                    FamilyName = fam.FamilyName,
                    Addresses = fam.Addresses,
                    Members = new List<ContactReportMambers>()
                };
                //Filter by address settings
                if (filters.Columns.Any(r => r.Column == ReportColumns.Address))
                {
                    var filter = filters.Columns.First(e => e.Column == ReportColumns.Address);
                    if (filter.Filter == ReportColumnsFilter.With)
                        if (fam.Addresses.Any(e => string.IsNullOrEmpty(e.Address))) continue;
                    if (filter.Filter == ReportColumnsFilter.Without)
                        if (fam.Addresses.All(e => string.IsNullOrEmpty(e.Address))) continue;
                }
                if (filters.Columns.Any(r => r.Column == ReportColumns.City))
                {
                    var filter = filters.Columns.First(e => e.Column == ReportColumns.City);
                    if (filter.Filter == ReportColumnsFilter.With)
                        if (fam.Addresses.Any(e => string.IsNullOrEmpty(e.City))) continue;
                    if (filter.Filter == ReportColumnsFilter.Without)
                        if (fam.Addresses.All(e => string.IsNullOrEmpty(e.City))) continue;
                }
                if (filters.Columns.Any(r => r.Column == ReportColumns.State))
                {
                    var filter = filters.Columns.First(e => e.Column == ReportColumns.State);
                    if (filter.Filter == ReportColumnsFilter.With)
                        if (fam.Addresses.Any(e => string.IsNullOrEmpty(e.State))) continue;
                    if (filter.Filter == ReportColumnsFilter.Without)
                        if (fam.Addresses.All(e => string.IsNullOrEmpty(e.State))) continue;
                }
                if (filters.Columns.Any(r => r.Column == ReportColumns.Zip))
                {
                    var filter = filters.Columns.First(e => e.Column == ReportColumns.Zip);
                    if (filter.Filter == ReportColumnsFilter.With)
                        if (fam.Addresses.Any(e => string.IsNullOrEmpty(e.Zip))) continue;
                    if (filter.Filter == ReportColumnsFilter.Without)
                        if (fam.Addresses.All(e => string.IsNullOrEmpty(e.Zip))) continue;
                }

                foreach (ContactReportMambers member in fam.Members)
                {
                    bool passedFilters = true;
                    foreach (ReportColumn filter in filters.Columns)
                    {
                        if (!passedFilters) break;
                        switch (filter.Column)
                        {
                            case ReportColumns.HomePhone:
                                if (filter.Filter == ReportColumnsFilter.With)
                                    if (!member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.HomePhone
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                if (filter.Filter == ReportColumnsFilter.Without)
                                    if (member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.HomePhone
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                break;
                            case ReportColumns.WorkPhone:
                                if (filter.Filter == ReportColumnsFilter.With)
                                    if (!member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.WorkPhone
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                if (filter.Filter == ReportColumnsFilter.Without)
                                    if (member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.WorkPhone
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                break;
                            case ReportColumns.MobilePhone:
                                if (filter.Filter == ReportColumnsFilter.With)
                                    if (!member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.MobilePhone
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                if (filter.Filter == ReportColumnsFilter.Without)
                                    if (member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.MobilePhone
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                break;
                            case ReportColumns.OtherPhone:
                                if (filter.Filter == ReportColumnsFilter.With)
                                    if (!member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.OtherPhone
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                if (filter.Filter == ReportColumnsFilter.Without)
                                    if (member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.OtherPhone
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                break;
                            case ReportColumns.Pager:
                                if (filter.Filter == ReportColumnsFilter.With)
                                    if (!member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.Pager
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                if (filter.Filter == ReportColumnsFilter.Without)
                                    if (member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.Pager
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                break;
                            case ReportColumns.EmergencyPhone:
                                if (filter.Filter == ReportColumnsFilter.With)
                                    if (!member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.EmergencyPhone
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                if (filter.Filter == ReportColumnsFilter.Without)
                                    if (member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.EmergencyPhone
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                break;
                            case ReportColumns.Fax:
                                if (filter.Filter == ReportColumnsFilter.With)
                                    if (!member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.Fax
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                if (filter.Filter == ReportColumnsFilter.Without)
                                    if (member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.Fax
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                break;
                            case ReportColumns.Email:
                                if (filter.Filter == ReportColumnsFilter.With)
                                    if (!member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.Email
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                if (filter.Filter == ReportColumnsFilter.Without)
                                    if (member.Contacts.Any(e => e.PhoneTypeID == (int)ContactTypes.Email
                                    && !string.IsNullOrEmpty(e.PhoneNumber))) passedFilters = false;
                                break;
                        }
                    }
                    if (passedFilters) newContact.Members.Add(member);
                }
                if (!newContact.Members.Any()) continue;
                result.Add(newContact);

            }

            return result;
        }
        #endregion
    }
}