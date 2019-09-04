﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Enums;
using PDFServiceAWS.Helpers;

namespace PDFServiceAWS.Services
{
    public class ReportGroupingService : IReportGroupingService
    {
        IEnumerable<PaymentMethodDto> _paymentMethods;
        IEnumerable<SolicitorDto> _solicitors;
        IEnumerable<MailingDto> _mailings;
        IEnumerable<DepartmentDto> _departments;
        IEnumerable<CategoryDto> _categoryTree;
        private Func<string, string> _translator;

        /// <summary>
        /// Initializate collections in case if we need name of one of them
        /// </summary>
        /// <param name="transFunc">Use delegate for translation method wich work with localResource file</param>
        /// <param name="paymentMethods"></param>
        /// <param name="solicitors"></param>
        /// <param name="mailings"></param>
        /// <param name="departments"></param>
        /// <param name="categoryTree"></param>
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

        public TransactionGrouped TotalBy(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            if (string.Equals(filter.GroupBy, "Subcategory", StringComparison.InvariantCultureIgnoreCase))
                filter.ShowDetails = false;
            if (!string.Equals(filter.subtotalBy, "None", StringComparison.InvariantCultureIgnoreCase))
                return SubTotalBy(transDto, filter);
            // matrix view
            if (!string.Equals(filter.totalOnlyBy, "totalOnly", StringComparison.InvariantCultureIgnoreCase))
                return TransactionTotalOnlyBy(transDto, filter);
            TransactionGrouped result = new TransactionGrouped();
            if (string.Equals(filter.GroupBy, "NameCompany", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = TotalByAllFamilies(transDto, filter);
            if (string.Equals(filter.GroupBy, "Category", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = TotalByAllCategories(transDto, filter);
            if (string.Equals(filter.GroupBy, "Subcategory", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = TotalByAllSubcategories(transDto, filter);
            if (string.Equals(filter.GroupBy, "Solicitor", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = TotalByAllSolicitor(transDto, filter);
            if (string.Equals(filter.GroupBy, "Method", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = TotalByAllMethods(transDto, filter);
            if (string.Equals(filter.GroupBy, "Mailing", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = TotalByAllMailing(transDto, filter);
            if (string.Equals(filter.GroupBy, "Depatrment", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = TotalByAllDepartment(transDto, filter);
            if (string.Equals(filter.GroupBy, "Weeks", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = TotalByAllWeeks(transDto, filter);
            if (string.Equals(filter.GroupBy, "Month", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = TotalByAllMonth(transDto, filter);
            if (string.Equals(filter.GroupBy, "Years", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = TotalByAllYears(transDto, filter);
            if (filter.view == TransFilterView.Total) CalcGrandAmounts(result, filter);
            return result;
        }




        /// <summary>
        /// Return subgrouped DTO for export
        /// </summary>
        /// <param name="transDto">DTO from database</param>
        /// <param name="filter">Filter criterias</param>
        /// <param name="customGroupBy">Use in case when you need use custom grouping</param>
        /// <param name="customSubTotalBy">Use in case when you need use custom subgrouping</param>
        /// <returns></returns>
        TransactionGrouped SubTotalBy(TransactionReportResultDto transDto, FilterTransactionReport filter, string customGroupBy = null, string customSubTotalBy = null)
        {
            string subtotalBy = string.IsNullOrEmpty(customSubTotalBy) ? filter.subtotalBy : customSubTotalBy;
            string totalBy = string.IsNullOrEmpty(customGroupBy) ? filter.GroupBy : customGroupBy;
            if (string.Equals(subtotalBy, "None", StringComparison.InvariantCultureIgnoreCase))
                return TotalBy(transDto, filter);
            if (string.Equals(filter.subtotalBy, "Subcategory", StringComparison.InvariantCultureIgnoreCase))
                filter.ShowDetails = false;

            var subTotal = new List<TransactionReportGroupedDTO>();
            var result = new TransactionGrouped();
            //subtotal use like total by
            if (string.Equals(subtotalBy, "NameCompany", StringComparison.InvariantCultureIgnoreCase))
                subTotal = TotalByAllFamilies(transDto, filter);
            if (string.Equals(subtotalBy, "Category", StringComparison.InvariantCultureIgnoreCase))
                subTotal = TotalByAllCategories(transDto, filter);
            if (string.Equals(subtotalBy, "Subcategory", StringComparison.InvariantCultureIgnoreCase))
                subTotal = TotalByAllSubcategories(transDto, filter);
            if (string.Equals(subtotalBy, "Solicitor", StringComparison.InvariantCultureIgnoreCase))
                subTotal = TotalByAllSolicitor(transDto, filter);
            if (string.Equals(subtotalBy, "Method", StringComparison.InvariantCultureIgnoreCase))
                subTotal = TotalByAllMethods(transDto, filter);
            if (string.Equals(subtotalBy, "Mailing", StringComparison.InvariantCultureIgnoreCase))
                subTotal = TotalByAllMailing(transDto, filter);
            if (string.Equals(subtotalBy, "Depatrment", StringComparison.InvariantCultureIgnoreCase))
                subTotal = TotalByAllDepartment(transDto, filter);
            if (string.Equals(subtotalBy, "Weeks", StringComparison.InvariantCultureIgnoreCase))
                subTotal = TotalByAllWeeks(transDto, filter);
            if (string.Equals(subtotalBy, "Month", StringComparison.InvariantCultureIgnoreCase))
                subTotal = TotalByAllMonth(transDto, filter);
            if (string.Equals(subtotalBy, "Years", StringComparison.InvariantCultureIgnoreCase))
                subTotal = TotalByAllYears(transDto, filter);

            //Total use subtotal methods
            if (string.Equals(totalBy, "NameCompany", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = SubGroupByFamilies(subTotal, transDto, filter);
            if (string.Equals(totalBy, "Category", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = SubGroupByCategory(subTotal, filter, transDto.CategoriesIDs.ToArray());
            if (string.Equals(totalBy, "Subcategory", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = SubGroupBySubcategory(subTotal, transDto, filter);
            if (string.Equals(totalBy, "Solicitor", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = SubGroupBySolicitor(subTotal, filter, transDto.Transactions.Select(r => r.SolicitorID).Distinct());
            if (string.Equals(totalBy, "Method", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = SubGroupByMethod(subTotal, transDto, filter);
            if (string.Equals(totalBy, "Mailing", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = SubGroupByMailing(subTotal, transDto, filter);
            if (string.Equals(totalBy, "Depatrment", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = SubGroupByDepatrment(subTotal, transDto, filter);
            if (string.Equals(totalBy, "Weeks", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = SubGroupByWeeks(subTotal, transDto, filter);
            if (string.Equals(totalBy, "Month", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = SubGroupByMonth(subTotal, transDto, filter);
            if (string.Equals(totalBy, "Years", StringComparison.InvariantCultureIgnoreCase))
                result.GroupedObj = SubGroupByYears(subTotal, transDto, filter);
            if (filter.view == TransFilterView.Total)
                CalcGrandAmounts(result, filter);
            return result;
        }


        #region Total by
        private List<TransactionReportGroupedDTO> TotalByAllFamilies(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> resp = new List<TransactionReportGroupedDTO>();
            Parallel.ForEach(transDto.Families, fm =>
            {
                var trans = new TransactionReportGroupedDTO()
                {
                    Name = fm.Family,
                    FamilyId = fm.FamilyID,
                    FamilyDetails = new FamilyDetails()
                    {
                        Addresses = fm.Addresses,
                        Contacts = fm.Contacts
                    },
                    Company = fm.Company,
                    Transactions = transDto.Transactions.AsParallel().Where(r => r.FamilyID == fm.FamilyID).OrderBy(c => c.Date).ToList(),
                    UnassignedPayments = transDto.UnassignedPayments.Any(e => e.FamilyID == fm.FamilyID)
                        ? transDto.UnassignedPayments.AsParallel().Where(r => r.FamilyID == fm.FamilyID).OrderBy(c => c.Date).ToList()
                        : new List<TransactionsReportList>(),
                    TotalName = GetTotalName(fm.Family)
                };
                CombineDetailsBySubcats(trans.Transactions);
                CalcTotalAmount(trans, filter);
                lock (resp)
                {
                    resp.Add(trans);
                }

            });
            return resp.OrderBy(x => x.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> TotalByAllSolicitor(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> resp = new List<TransactionReportGroupedDTO>();
            var solIds = transDto.Transactions.Select(d => d.SolicitorID).Distinct().ToList();
            Parallel.ForEach(solIds, solId =>
            {
                var solName = GetSolicitorName(solId);
                var trans = new TransactionReportGroupedDTO()
                {
                    Name = solName,
                    ID = solId,
                    Transactions = transDto.Transactions.AsParallel().Where(r => r.SolicitorID == solId).OrderBy(c => c.Date).ToList(),
                    UnassignedPayments = transDto.UnassignedPayments.Any(e => e.SolicitorID == solId)
                        ? transDto.UnassignedPayments.AsParallel().Where(r => r.SolicitorID == solId).OrderBy(c => c.Date).ToList()
                        : new List<TransactionsReportList>(),
                    TotalName = GetTotalName(solName)
                };
                CombineDetailsBySubcats(trans.Transactions);
                CalcTotalAmount(trans, filter);
                lock (resp)
                {
                    resp.Add(trans);
                }
            });
            return resp.OrderBy(d => d.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> TotalByAllCategories(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> resp = new List<TransactionReportGroupedDTO>();
            Parallel.ForEach(transDto.CategoriesIDs, ct =>
            {
                var catName = GetCategoryName(ct);
                var trans = new TransactionReportGroupedDTO
                {
                    Name = catName,
                    ID = ct,
                    CategoryId = ct,
                    TotalName = GetTotalName(catName),
                    Transactions = transDto.Transactions.Where(r => r.Details[0].CategoryID == ct).OrderBy(c => c.Date).ToList(),
                    UnassignedPayments = transDto.UnassignedPayments.Any(e => e.Details[0].CategoryID == ct)
                        ? transDto.UnassignedPayments.AsParallel().Where(r => r.Details[0].CategoryID == ct).OrderBy(c => c.Date).ToList()
                        : new List<TransactionsReportList>()
                };
                CombineDetailsBySubcats(trans.Transactions);
                CalcTotalAmount(trans, filter);
                lock (resp)
                {
                    resp.Add(trans);
                }
            });
            return resp.OrderBy(f => f.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> TotalByAllSubcategories(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<int?> subcatsIds = transDto.Transactions.SelectMany(r => r.Details.Select(w => w.SubcategoryID)).Distinct().ToList();
            List<TransactionReportGroupedDTO> resp = new List<TransactionReportGroupedDTO>();
            Parallel.ForEach(subcatsIds, subcatId =>
            {
                var subCatName = GetSubCategoryName(subcatId);
                int? catId = null;
                var trans = new TransactionReportGroupedDTO
                {
                    Name = subCatName,
                    TotalName = GetTotalName(subCatName),
                    Transactions = transDto.Transactions.SelectMany(e => e.Details
                            .Where(r => r.SubcategoryID == subcatId && e.TransType == (int)filter.ReportType)
                            .Select(
                                q =>
                                {
                                    if (catId == null) catId = q.CategoryID;
                                    var tr = new TransactionsReportList
                                    {
                                        TransactionID = e.TransactionID,
                                        FamilyID = e.FamilyID,
                                        BillID = q.BillID,
                                        InvoiceNo = e.InvoiceNo,
                                        Date = e.Date,
                                        Amount = q.Amount,
                                        AmountDue = q.AmountDue,
                                        CheckNo = e.CheckNo,
                                        TransType = q.TransType,
                                        Note = q.Note,
                                        Details = new List<TransactionDetailReportList>
                                        {
                                            new TransactionDetailReportList
                                            {
                                                CategoryID = q.CategoryID,
                                                SubcategoryID = q.SubcategoryID
                                            }
                                        }
                                    };
                                    return tr;
                                })).OrderBy(c => c.Date)
                        .ToList(),
                    UnassignedPayments = new List<TransactionsReportList>(),
                    UnasignedAmount = 0
                };
                CalcTotalAmount(trans, filter);
                lock (resp)
                {
                    resp.Add(trans);
                }
            });
            return resp.OrderBy(v => v.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> TotalByAllMethods(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> resp = new List<TransactionReportGroupedDTO>();
            var methodIds = transDto.Transactions.Select(e => e.PaymentMethodID).Distinct().ToList();
            Parallel.ForEach(methodIds, metId =>
            {
                var metName = GetMethodName(metId);
                var trans = new TransactionReportGroupedDTO()
                {
                    Name = metName,
                    TotalName = GetTotalName(metName),
                    ID = metId,
                    Transactions = transDto.Transactions.Where(r => r.PaymentMethodID == metId).OrderBy(c => c.Date).ToList(),
                    UnassignedPayments = transDto.UnassignedPayments.Any(e => e.PaymentMethodID == metId)
                        ? transDto.UnassignedPayments.AsParallel().Where(r => r.PaymentMethodID == metId).OrderBy(c => c.Date).ToList()
                        : new List<TransactionsReportList>(),
                };
                CombineDetailsBySubcats(trans.Transactions);
                CalcTotalAmount(trans, filter);
                lock (resp)
                {
                    resp.Add(trans);
                }
            });

            return resp.OrderBy(c => c.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> TotalByAllMailing(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {

            List<TransactionReportGroupedDTO> resp = new List<TransactionReportGroupedDTO>();
            var mailingIds = transDto.Transactions.Select(e => e.MailingID).Distinct().ToList();
            Parallel.ForEach(mailingIds, mailingId =>
            {
                var mailingName = GetMailingName(mailingId);
                var trans = new TransactionReportGroupedDTO()
                {
                    Name = mailingName,
                    TotalName = GetTotalName(mailingName),
                    Transactions = transDto.Transactions.Where(r => r.MailingID == mailingId).OrderBy(c => c.Date).ToList(),
                    UnassignedPayments = transDto.UnassignedPayments.Any(e => e.MailingID == mailingId)
                        ? transDto.UnassignedPayments.AsParallel().Where(r => r.MailingID == mailingId).OrderBy(c => c.Date).ToList()
                        : new List<TransactionsReportList>(),
                };
                CombineDetailsBySubcats(trans.Transactions);
                CalcTotalAmount(trans, filter);
                lock (resp)
                {
                    resp.Add(trans);
                }
            });

            return resp.OrderBy(c => c.Name).ToList();
        }


        private List<TransactionReportGroupedDTO> TotalByAllDepartment(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> resp = new List<TransactionReportGroupedDTO>();
            var depIds = transDto.Transactions.Select(e => e.DepartmentID).Distinct().ToList();
            Parallel.ForEach(depIds, depId =>
            {
                var metName = GetDepartmentName(depId);
                var trans = new TransactionReportGroupedDTO()
                {
                    Name = metName,
                    TotalName = GetTotalName(metName),
                    ID = depId,
                    Transactions = transDto.Transactions.Where(r => r.DepartmentID == depId).OrderBy(c => c.Date).ToList(),
                    UnassignedPayments = transDto.UnassignedPayments.Any(e => e.DepartmentID == depId)
                        ? transDto.UnassignedPayments.AsParallel().Where(r => r.DepartmentID == depId).OrderBy(c => c.Date).ToList()
                        : new List<TransactionsReportList>(),
                };
                CombineDetailsBySubcats(trans.Transactions);
                CalcTotalAmount(trans, filter);
                lock (resp)
                {
                    resp.Add(trans);
                }
            });

            return resp.OrderBy(c => c.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> TotalByAllWeeks(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> resp = new List<TransactionReportGroupedDTO>();
            var weeks = transDto.Transactions.Select(e => GetWeekOfYear(e.Date)).Distinct().OrderBy(w => w).ToList();

            Parallel.ForEach(weeks, weeknum =>
            {
                var trans = new TransactionReportGroupedDTO()
                {
                    Transactions = transDto.Transactions.AsParallel().Where(r => GetWeekOfYear(r.Date) == weeknum).OrderBy(c => c.Date).ToList(),
                    UnassignedPayments = transDto.UnassignedPayments.Any(e => GetWeekOfYear(e.Date) == weeknum)
                        ? transDto.UnassignedPayments.AsParallel().Where(r => GetWeekOfYear(r.Date) == weeknum).OrderBy(c => c.Date).ToList()
                        : new List<TransactionsReportList>(),
                };
                CombineDetailsBySubcats(trans.Transactions);
                var startDate = trans.Transactions.Min(e => e.Date).ToShortDateString();
                var endDate = trans.Transactions.Max(e => e.Date).ToShortDateString();
                trans.Name = GetTranslation("report_week_start_date") + " " + startDate + "(#" + weeknum + ")";
                trans.TotalName = GetTranslation("report_week_end_date") + " " + endDate + "(#" + weeknum + ")";
                CalcTotalAmount(trans, filter);
                lock (resp)
                {
                    resp.Add(trans);
                }
            });

            return resp.OrderBy(c => c.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> TotalByAllMonth(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> resp = new List<TransactionReportGroupedDTO>();
            var months = transDto.Transactions.Select(e => e.Date.Month).Distinct().OrderBy(w => w).ToList();

            Parallel.ForEach(months, monthNum =>
            {
                var trans = new TransactionReportGroupedDTO()
                {
                    Transactions = transDto.Transactions.AsParallel().Where(r => r.Date.Month == monthNum).OrderBy(c => c.Date).ToList(),
                    UnassignedPayments = transDto.UnassignedPayments.Any(e => e.Date.Month == monthNum)
                        ? transDto.UnassignedPayments.AsParallel().Where(r => r.Date.Month == monthNum).OrderBy(c => c.Date).ToList()
                        : new List<TransactionsReportList>(),
                };
                CombineDetailsBySubcats(trans.Transactions);
                var startDate = trans.Transactions.Min(e => e.Date).ToShortDateString();
                var endDate = trans.Transactions.Max(e => e.Date).ToShortDateString();
                trans.Name = GetTranslation("report_month_start_date") + " " + startDate + "(#" + monthNum + ")";
                trans.TotalName = GetTranslation("report_month_end_date") + " " + endDate + "(#" + monthNum + ")";
                CalcTotalAmount(trans, filter);
                lock (resp)
                {
                    resp.Add(trans);
                }
            });

            return resp.OrderBy(c => c.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> TotalByAllYears(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> resp = new List<TransactionReportGroupedDTO>();
            var years = transDto.Transactions.Select(e => e.Date.Year).Distinct().OrderBy(w => w).ToList();

            Parallel.ForEach(years, year =>
            {
                var trans = new TransactionReportGroupedDTO()
                {
                    Transactions = transDto.Transactions.AsParallel().Where(r => r.Date.Year == year).OrderBy(c => c.Date).ToList(),
                    UnassignedPayments = transDto.UnassignedPayments.Any(e => e.Date.Year == year)
                        ? transDto.UnassignedPayments.AsParallel().Where(r => r.Date.Year == year).OrderBy(c => c.Date).ToList()
                        : new List<TransactionsReportList>(),
                };
                CombineDetailsBySubcats(trans.Transactions);
                var startDate = trans.Transactions.Min(e => e.Date).ToShortDateString();
                var endDate = trans.Transactions.Max(e => e.Date).ToShortDateString();
                trans.Name = GetTranslation("report_year_start_date") + " " + startDate + "(#" + year + ")";
                trans.DateName = year.ToString();
                trans.TotalName = GetTranslation("report_year_end_date") + " " + endDate + "(#" + year + ")";
                CalcTotalAmount(trans, filter);
                lock (resp)
                {
                    resp.Add(trans);
                }
            });

            return resp.OrderBy(c => c.Name).ToList();
        }
        #endregion

        #region Subtotal by

        private List<TransactionReportGroupedDTO> SubGroupByFamilies(List<TransactionReportGroupedDTO> subTotal, TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> result = new List<TransactionReportGroupedDTO>();
            Parallel.ForEach(transDto.Families, fm =>
            {
                var obj = new TransactionReportGroupedDTO
                {
                    Name = fm.Family,
                    FamilyId = fm.FamilyID,
                    ID = fm.FamilyID,
                    FamilyDetails = new FamilyDetails
                    {
                        Addresses = fm.Addresses,
                        Contacts = fm.Contacts
                    },
                    Company = fm.Company,
                    TotalName = GetTotalName(fm.Family),
                    SubGrouped = subTotal.AsParallel().Where(r => r.Transactions.Any(d => d.FamilyID == fm.FamilyID)).Select(w =>
                    {
                        TransactionReportGroupedDTO res = (TransactionReportGroupedDTO)w.Clone();
                        res.Transactions = w.Transactions.Where(e => e.FamilyID == fm.FamilyID).OrderBy(r => r.Date).ToList();
                        if (w.UnassignedPayments != null && w.UnassignedPayments.Any())
                            res.UnassignedPayments = w.UnassignedPayments.Where(e => e.FamilyID == fm.FamilyID).OrderBy(r => r.Date).ToList();
                        CalcTotalAmount(res, filter);
                        return res;
                    }).ToList()
                };
                CalcTotalAmountForGrouping(obj, filter);
                lock (result)
                {
                    result.Add(obj);
                }
            });
            return result.OrderBy(r => r.Name).ToList();
        }

        List<TransactionReportGroupedDTO> SubGroupBySolicitor(List<TransactionReportGroupedDTO> subGroup,
           FilterTransactionReport filter, IEnumerable<int?> solisitorsIds)
        {
            List<TransactionReportGroupedDTO> result = new List<TransactionReportGroupedDTO>();
            foreach (int? solisitorsId in solisitorsIds)
            {
                var solName = GetSolicitorName(solisitorsId);
                var id = solisitorsId;
                var obj = new TransactionReportGroupedDTO
                {
                    Name = solName,
                    ID = solisitorsId,
                    TotalName = GetTotalName(solName),
                    SubGrouped = subGroup.AsParallel().Where(r => r.Transactions.Any(d => d.SolicitorID == id)).Select(w =>
                    {
                        TransactionReportGroupedDTO res = (TransactionReportGroupedDTO)w.Clone();
                        res.Transactions = w.Transactions.Where(e => e.SolicitorID == id).OrderBy(r => r.Date).ToList();
                        if (w.UnassignedPayments != null && w.UnassignedPayments.Any())
                            res.UnassignedPayments = w.UnassignedPayments.Where(e => e.SolicitorID == id).OrderBy(r => r.Date).ToList();
                        CalcTotalAmount(res, filter);
                        return res;
                    }).ToList()
                };
                CalcTotalAmountForGrouping(obj, filter);

                result.Add(obj);
            }


            return result.OrderBy(r => r.Name).ToList();
        }


        private List<TransactionReportGroupedDTO> SubGroupByCategory(List<TransactionReportGroupedDTO> subTotal, FilterTransactionReport filter, int[] catIds)
        {
            List<TransactionReportGroupedDTO> result = new List<TransactionReportGroupedDTO>();

            Parallel.ForEach(catIds, ct =>
            {
                var ctName = GetCategoryName(ct);
                var obj = new TransactionReportGroupedDTO
                {
                    Name = ctName,
                    ID = ct,
                    TotalName = GetTotalName(ctName),
                    SubGrouped = subTotal.AsParallel().Where(r => r.Transactions.Any(d => d.Details[0].CategoryID == ct)).Select(w =>
                    {
                        TransactionReportGroupedDTO res = (TransactionReportGroupedDTO)w.Clone();
                        res.Transactions = w.Transactions.Where(e => e.Details[0].CategoryID == ct).OrderBy(r => r.Date).ToList();
                        if (w.UnassignedPayments != null && w.UnassignedPayments.Any())
                            res.UnassignedPayments = w.UnassignedPayments.Where(e => e.Details[0].CategoryID == ct).OrderBy(r => r.Date).ToList();
                        CalcTotalAmount(res, filter);
                        return res;
                    }).ToList()
                };
                CalcTotalAmountForGrouping(obj, filter);
                lock (result)
                {
                    result.Add(obj);
                }
            });

            return result.OrderBy(r => r.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> SubGroupBySubcategory(
            List<TransactionReportGroupedDTO> subTotal, TransactionReportResultDto transDto,
            FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> result = new List<TransactionReportGroupedDTO>();
            var subcatsIds = transDto.Transactions.SelectMany(r => r.Details.Select(w => w.SubcategoryID)).Distinct();
            Parallel.ForEach(subcatsIds, subcatId =>
            {
                var subCatName = GetSubCategoryName(subcatId);
                int? catId = null;
                var trans = new TransactionReportGroupedDTO
                {
                    Name = subCatName,
                    TotalName = GetTotalName(subCatName),
                    SubGrouped = subTotal
                        .Where(r => r.Transactions.Any(e => e.Details.Any(q => q.SubcategoryID == subcatId)))
                        .Select(
                            t =>
                            {
                                var objTr = new TransactionReportGroupedDTO
                                {
                                    Name = t.Name,
                                    TotalName = t.TotalName,
                                    Transactions = t.Transactions.SelectMany(e => e.Details.Where(r => r.SubcategoryID == subcatId && e.TransType == (int)filter.ReportType).Select(
                                        q =>
                                        {
                                            if (catId == null) catId = q.CategoryID;
                                            var tr = new TransactionsReportList
                                            {
                                                TransactionID = e.TransactionID,
                                                FamilyID = e.FamilyID,
                                                BillID = q.BillID,
                                                InvoiceNo = e.InvoiceNo,
                                                Date = e.Date,
                                                Amount = q.Amount,
                                                AmountDue = q.AmountDue,
                                                CheckNo = e.CheckNo,
                                                TransType = q.TransType,
                                                Note = q.Note,
                                                Details = new List<TransactionDetailReportList>
                                                {
                                                    new TransactionDetailReportList
                                                    {
                                                        CategoryID = q.CategoryID,
                                                        SubcategoryID = q.SubcategoryID
                                                    }
                                                }
                                            };
                                            return tr;
                                        })).OrderBy(r => r.Date).ToList(),
                                    UnassignedPayments = new List<TransactionsReportList>()
                                };
                                CalcTotalAmount(objTr, filter);
                                return objTr;
                            })
                        .ToList()
                };
                CalcTotalAmountSubCats(trans);
                lock (result)
                {
                    result.Add(trans);
                }
            });
            return result.OrderBy(r => r.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> SubGroupByMethod(List<TransactionReportGroupedDTO> subTotal, TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> result = new List<TransactionReportGroupedDTO>();
            var methodIds = transDto.Transactions.Select(e => e.PaymentMethodID).Distinct();

            Parallel.ForEach(methodIds, metId =>
            {
                var metName = GetMethodName(metId);
                var obj = new TransactionReportGroupedDTO
                {
                    Name = metName,
                    ID = metId,
                    TotalName = GetTotalName(metName),
                    SubGrouped = subTotal.AsParallel().Where(r => r.Transactions.Any(d => d.PaymentMethodID == metId)).Select(w =>
                    {
                        TransactionReportGroupedDTO res = (TransactionReportGroupedDTO)w.Clone();
                        res.Transactions = w.Transactions.Where(e => e.PaymentMethodID == metId).OrderBy(r => r.Date).ToList();
                        if (w.UnassignedPayments != null && w.UnassignedPayments.Any())
                            res.UnassignedPayments = w.UnassignedPayments.Where(e => e.PaymentMethodID == metId).OrderBy(r => r.Date).ToList();
                        CalcTotalAmount(res, filter);
                        return res;
                    }).ToList()
                };
                CalcTotalAmountForGrouping(obj, filter);
                lock (result)
                {
                    result.Add(obj);
                }
            });
            return result.OrderBy(r => r.Name).ToList();
        }
        private List<TransactionReportGroupedDTO> SubGroupByMailing(List<TransactionReportGroupedDTO> subTotal, TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> result = new List<TransactionReportGroupedDTO>();
            var mailingIds = transDto.Transactions.Select(e => e.MailingID).Distinct();

            Parallel.ForEach(mailingIds, mailingId =>
            {
                var metName = GetMethodName(mailingId);
                var obj = new TransactionReportGroupedDTO
                {
                    Name = metName,
                    ID = mailingId,
                    TotalName = GetTotalName(metName),
                    SubGrouped = subTotal.AsParallel().Where(r => r.Transactions.Any(d => d.MailingID == mailingId)).Select(w =>
                    {
                        TransactionReportGroupedDTO res = (TransactionReportGroupedDTO)w.Clone();
                        res.Transactions = w.Transactions.Where(e => e.MailingID == mailingId).OrderBy(r => r.Date).ToList();
                        if (w.UnassignedPayments != null && w.UnassignedPayments.Any())
                            res.UnassignedPayments = w.UnassignedPayments.Where(e => e.MailingID == mailingId).OrderBy(r => r.Date).ToList();
                        CalcTotalAmount(res, filter);
                        return res;
                    }).ToList()
                };
                CalcTotalAmountForGrouping(obj, filter);
                lock (result)
                {
                    result.Add(obj);
                }
            });
            return result.OrderBy(r => r.Name).ToList();
        }
        private List<TransactionReportGroupedDTO> SubGroupByDepatrment(List<TransactionReportGroupedDTO> subTotal, TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> result = new List<TransactionReportGroupedDTO>();
            var depIds = transDto.Transactions.Select(e => e.DepartmentID).Distinct();

            Parallel.ForEach(depIds, depId =>
            {
                var depName = GetDepartmentName(depId);
                var obj = new TransactionReportGroupedDTO
                {
                    Name = depName,
                    ID = depId,
                    TotalName = GetTotalName(depName),
                    SubGrouped = subTotal.AsParallel().Where(r => r.Transactions.Any(d => d.DepartmentID == depId)).Select(w =>
                    {
                        TransactionReportGroupedDTO res = (TransactionReportGroupedDTO)w.Clone();
                        res.Transactions = w.Transactions.Where(e => e.DepartmentID == depId).OrderBy(r => r.Date).ToList();
                        if (w.UnassignedPayments != null && w.UnassignedPayments.Any())
                            res.UnassignedPayments = w.UnassignedPayments.Where(e => e.DepartmentID == depId).OrderBy(r => r.Date).ToList();
                        CalcTotalAmount(res, filter);
                        return res;
                    }).ToList()
                };
                CalcTotalAmountForGrouping(obj, filter);
                lock (result)
                {
                    result.Add(obj);
                }
            });
            return result.OrderBy(r => r.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> SubGroupByWeeks(List<TransactionReportGroupedDTO> subTotal, TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> result = new List<TransactionReportGroupedDTO>();
            var weeks = transDto.Transactions.Select(e => GetWeekOfYear(e.Date)).Distinct().OrderBy(w => w).ToList();

            Parallel.ForEach(weeks, weekNum =>
            {
                string startDate = String.Empty;
                string endDate = String.Empty;
                var obj = new TransactionReportGroupedDTO
                {
                    ID = weekNum,
                    SubGrouped = subTotal.AsParallel()
                        .Where(r => r.Transactions.Any(d => GetWeekOfYear(d.Date) == weekNum))
                        .Select(w =>
                        {
                            TransactionReportGroupedDTO res = (TransactionReportGroupedDTO)w.Clone();
                            res.Transactions = w.Transactions.Where(e => GetWeekOfYear(e.Date) == weekNum).OrderBy(r => r.Date).ToList();
                            if (w.UnassignedPayments != null && w.UnassignedPayments.Any())
                                res.UnassignedPayments = w.UnassignedPayments
                                    .Where(e => GetWeekOfYear(e.Date) == weekNum)
                                    .OrderBy(r => r.Date)
                                    .ToList();
                            CalcTotalAmount(res, filter);
                            startDate = res.Transactions.Min(e => e.Date).ToShortDateString();
                            endDate = res.Transactions.Max(e => e.Date).ToShortDateString();

                            return res;
                        })
                        .ToList(),
                    Name = GetTranslation("report_week_start_date") + " " + startDate + "(#" + weekNum + ")",
                    TotalName = GetTranslation("report_week_end_date") + " " + endDate + "(#" + weekNum + ")"
                };
                CalcTotalAmountForGrouping(obj, filter);
                lock (result)
                {
                    result.Add(obj);
                }
            });
            return result.OrderBy(r => r.Name).ToList();
        }

        private List<TransactionReportGroupedDTO> SubGroupByMonth(List<TransactionReportGroupedDTO> subTotal, TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> result = new List<TransactionReportGroupedDTO>();
            var months = transDto.Transactions.Select(e => e.Date.Month).Distinct().OrderBy(w => w).ToList();

            Parallel.ForEach(months, monthNum =>
            {
                string startDate = String.Empty;
                string endDate = String.Empty;
                var obj = new TransactionReportGroupedDTO
                {
                    ID = monthNum,
                    SubGrouped = subTotal.AsParallel()
                        .Where(r => r.Transactions.Any(d => d.Date.Month == monthNum))
                        .Select(w =>
                        {
                            TransactionReportGroupedDTO res = (TransactionReportGroupedDTO)w.Clone();
                            res.Transactions = w.Transactions.Where(e => e.Date.Month == monthNum).OrderBy(r => r.Date).ToList();
                            if (w.UnassignedPayments != null && w.UnassignedPayments.Any())
                                res.UnassignedPayments = w.UnassignedPayments.Where(e => e.Date.Month == monthNum).OrderBy(r => r.Date).ToList();
                            CalcTotalAmount(res, filter);
                            startDate = res.Transactions.Min(e => e.Date).ToShortDateString();
                            endDate = res.Transactions.Max(e => e.Date).ToShortDateString();

                            return res;
                        })
                        .ToList(),
                    Name = GetTranslation("report_month_start_date") + " " + startDate + "(#" + monthNum + ")",
                    TotalName = GetTranslation("report_month_end_date") + " " + endDate + "(#" + monthNum + ")"
                };
                CalcTotalAmountForGrouping(obj, filter);
                lock (result)
                {
                    result.Add(obj);
                }
            });
            return result.OrderBy(r => r.Name).ToList();
        }
        private List<TransactionReportGroupedDTO> SubGroupByYears(List<TransactionReportGroupedDTO> subTotal, TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            List<TransactionReportGroupedDTO> result = new List<TransactionReportGroupedDTO>();
            var years = transDto.Transactions.Select(e => e.Date.Year).Distinct().OrderBy(w => w).ToList();

            Parallel.ForEach(years, year =>
            {
                string startDate = String.Empty;
                string endDate = String.Empty;
                var obj = new TransactionReportGroupedDTO
                {
                    ID = year,
                    SubGrouped = subTotal.AsParallel()
                        .Where(r => r.Transactions.Any(d => d.Date.Year == year))
                        .Select(w =>
                        {
                            TransactionReportGroupedDTO res = (TransactionReportGroupedDTO)w.Clone();
                            res.Transactions = w.Transactions.Where(e => e.Date.Year == year).OrderBy(r => r.Date).ToList();
                            if (w.UnassignedPayments != null && w.UnassignedPayments.Any())
                                res.UnassignedPayments = w.UnassignedPayments.Where(e => e.Date.Year == year).OrderBy(r => r.Date).ToList();
                            CalcTotalAmount(res, filter);
                            startDate = res.Transactions.Min(e => e.Date).ToShortDateString();
                            endDate = res.Transactions.Max(e => e.Date).ToShortDateString();

                            return res;
                        })
                        .ToList(),
                    Name = GetTranslation("report_year_start_date") + " " + startDate + "(#" + year + ")",
                    DateName = year.ToString(),
                    TotalName = GetTranslation("report_year_end_date") + " " + endDate + "(#" + year + ")"
                };
                CalcTotalAmountForGrouping(obj, filter);
                lock (result)
                {
                    result.Add(obj);
                }
            });
            return result.OrderBy(r => r.Name).ToList();
        }
        #endregion

        #region Matrix

        public MatrixDTO TransactionTotalOnlyBy(TransactionReportResultDto transDto, FilterTransactionReport filter)
        {
            var result = new MatrixDTO();
            //grouping for final result
            var transGrouped = SubTotalBy(transDto, filter, customGroupBy: filter.totalOnlyBy, customSubTotalBy: filter.GroupBy);
            // Getting columns
            result.Columns = transGrouped.GroupedObj.Select(r =>
            {
                var col = new TransactionMatrixColumn { Name = r.Name };
                if (string.Equals(filter.totalOnlyBy, "Category", StringComparison.InvariantCultureIgnoreCase))
                    if (r.ID != null) col.Value = (int)r.ID;
                if (string.Equals(filter.totalOnlyBy, "Solicitor", StringComparison.InvariantCultureIgnoreCase))
                    if (r.ID != null) col.Value = (int)r.ID;
                if (string.Equals(filter.totalOnlyBy, "Method", StringComparison.InvariantCultureIgnoreCase))
                    if (r.ID != null) col.Value = (int)r.ID;
                if (string.Equals(filter.totalOnlyBy, "Depatrment", StringComparison.InvariantCultureIgnoreCase))
                    if (r.ID != null) col.Value = (int)r.ID;
                if (string.Equals(filter.totalOnlyBy, "Years", StringComparison.InvariantCultureIgnoreCase)) col.Value = int.Parse(r.DateName);
                return col;
            }
            ).Distinct().OrderBy(r => r.Value).ToList();

            //total amounts
            decimal[] totalAmounts = new decimal[result.Columns.Count];
            //setup all amount for roral to zero
            for (int i = 0; i < result.Columns.Count; i++)
            {
                totalAmounts[i] = 0;
            }
            // getting rows
            for (var index = 0; index < result.Columns.Count; index++)
            {
                TransactionMatrixColumn col = result.Columns[index];

                foreach (TransactionReportGroupedDTO obj in transGrouped.GroupedObj)
                {
                    if (string.Equals(obj.Name, col.Name, StringComparison.CurrentCultureIgnoreCase))
                    {
                        foreach (TransactionReportGroupedDTO det in obj.SubGrouped)
                        {
                            if (result.Rows != null && result.Rows.Any(e => string.Equals(e.Name, det.Name,
                                    StringComparison.InvariantCultureIgnoreCase)))
                            {
                                result.Rows
                                    .First(r => string.Equals(r.Name, det.Name,
                                        StringComparison.InvariantCultureIgnoreCase))
                                    .Amounts[index] = det.TotalAmount;
                            }
                            else
                            {
                                if (result.Rows == null) result.Rows = new List<TransactionMatrixRows>();
                                var row = new TransactionMatrixRows
                                {
                                    Name = det.Name,
                                    TotalName = det.TotalName,
                                    FamilyDetails =
                                        string.Equals(filter.GroupBy, "NameCompany",
                                            StringComparison.CurrentCultureIgnoreCase)
                                            ? det.FamilyDetails
                                            : null,
                                    Amounts = (new decimal[result.Columns.Count]).ToList()
                                };
                                //setup all to zero
                                for (var i = 0; i < result.Columns.Count; i++)
                                {
                                    row.Amounts[i] = 0;
                                }
                                row.Amounts[index] = det.TotalAmount;
                                result.Rows.Add(row);
                            }
                            totalAmounts[index] += det.TotalAmount;
                        }
                    }
                }
            }

            if (!string.Equals(filter.subtotalBy, "None", StringComparison.InvariantCultureIgnoreCase))
            {
                var groupedForSubtotal = SubTotalBy(transDto, filter);

                foreach (TransactionMatrixRows row in result.Rows)
                {
                    var obj = groupedForSubtotal.GroupedObj.FirstOrDefault(e => string.Equals(e.Name, row.Name,
                        StringComparison.InvariantCultureIgnoreCase));
                    if (obj == null)
                    {
                        continue;
                    }
                    foreach (TransactionReportGroupedDTO tr in obj.SubGrouped)
                    {
                        TransactioMatrixSubDetails subObj = new TransactioMatrixSubDetails
                        {
                            Name = tr.Name,
                            Amounts = result.Columns.Select(r => GetAmountsForSubtotal(tr, r.Value, filter)).ToList()
                        };
                        var total = subObj.Amounts.Sum();
                        subObj.Amounts.Add(total);
                        if (row.SubDetails == null) row.SubDetails = new List<TransactioMatrixSubDetails>();
                        row.SubDetails.Add(subObj);
                    }
                }
            }
            result.Rows = result.Rows.OrderBy(f => f.Name).ToList();
            result.Rows.Add(new TransactionMatrixRows
            {
                Name = GetTranslation("report_totalAmount_total"),
                TotalName = GetTranslation("report_totalAmount_total"),
                Amounts = totalAmounts.ToList(),
                FamilyDetails = null,
                SubDetails = null
            });
            //for years show only years
            if (string.Equals(filter.totalOnlyBy, "Years", StringComparison.InvariantCultureIgnoreCase))
                foreach (TransactionMatrixColumn col in result.Columns)
                {
                    col.Name = col.Value.ToString();
                }
            result.Columns.Add(new TransactionMatrixColumn
            {
                Name = "Total"
            });
            foreach (TransactionMatrixRows rw in result.Rows)
            {
                var totalSum = rw.Amounts.Sum();
                rw.Amounts.Add(totalSum);
            }
            return result;
        }
        #endregion
        void CombineDetailsBySubcats(List<TransactionsReportList> trans)
        {
            if (trans != null && trans.Any())
            {
                Parallel.ForEach(trans, tr =>
                {
                    if (tr.Details.Any(r => r.TransType == 1))
                    {
                        foreach (TransactionDetailReportList det in tr.Details)
                        {
                            var det1 = det;
                            var payments =
                                tr.Details.Where(e => e.TransType == 1 && e.SubcategoryID == det1.SubcategoryID).ToList();
                            det.PaidAmount = payments.Any() ? payments.Sum(e => e.Amount) : 0;
                        }
                    }
                });
            }
        }


        void CalcTotalAmount(TransactionReportGroupedDTO trans, FilterTransactionReport filter)
        {
            if (filter.ReportType == TransFilterType.Payment)
                trans.TotalAmount = trans.Transactions.Sum(r => r.Amount);

            if (filter.ReportType == TransFilterType.Bill)
            {
                trans.TotalAmount = trans.Transactions.Sum(r => -r.Amount);
                trans.UnasignedAmount = trans.UnassignedPayments.Any() ? trans.UnassignedPayments.Sum(r => r.Amount) : 0;
                var sum = trans.Transactions.Sum(r => r.AmountDue);
                if (sum != null)
                    trans.DueTotalAmount = (decimal)sum;
                trans.PaidTotalAmount = trans.TotalAmount - trans.DueTotalAmount;
            }
        }

        void CalcTotalAmountForGrouping(TransactionReportGroupedDTO trans, FilterTransactionReport filter)
        {
            trans.TotalAmount = trans.SubGrouped.Sum(r => r.TotalAmount);

            if (filter.ReportType != TransFilterType.Bill) return;

            trans.UnasignedAmount = trans.SubGrouped.Sum(r => r.UnasignedAmount);
            trans.DueTotalAmount = trans.SubGrouped.Sum(r => r.DueTotalAmount);

            trans.PaidTotalAmount = trans.TotalAmount - trans.DueTotalAmount;

        }

        void CalcGrandAmounts(TransactionGrouped trans, FilterTransactionReport filter)
        {
            trans.GrandAmount = trans.GroupedObj.Sum(r => r.TotalAmount);
            if (filter.ReportType == TransFilterType.Bill)
            {
                trans.GrandDue = trans.GroupedObj.Sum(r => r.DueTotalAmount);
                trans.GrandPaid = trans.GroupedObj.Sum(r => r.PaidTotalAmount);
                trans.GrandUnasignedAmount = trans.GroupedObj.Sum(r => r.UnasignedAmount);
            }
        }
        private void CalcTotalAmountSubCats(TransactionReportGroupedDTO trans)
        {
            trans.TotalAmount = trans.SubGrouped.Sum(e => e.TotalAmount);
        }



        #region Private methods

        decimal GetAmountsForSubtotal(TransactionReportGroupedDTO tr, int subValue, FilterTransactionReport filter)
        {
            if (string.Equals(filter.totalOnlyBy, "Category", StringComparison.CurrentCultureIgnoreCase)) return GetAmountByCategory(tr, subValue, filter);
            if (string.Equals(filter.totalOnlyBy, "Solicitor", StringComparison.CurrentCultureIgnoreCase)) return GetAmountBySolicitor(tr, subValue);
            if (string.Equals(filter.totalOnlyBy, "Method", StringComparison.CurrentCultureIgnoreCase)) return GetAmountByMethod(tr, subValue);
            if (string.Equals(filter.totalOnlyBy, "Depatrment", StringComparison.CurrentCultureIgnoreCase)) return GetAmountByDepatrment(tr, subValue);
            if (string.Equals(filter.totalOnlyBy, "Years", StringComparison.CurrentCultureIgnoreCase)) return GetAmountByYears(tr, subValue);
            return 0;
        }

        private decimal GetAmountByYears(TransactionReportGroupedDTO tr, int subValue)
        {
            return tr.Transactions.Where(e => e.Date.Year == subValue).Sum(f => f.Amount);
        }

        private decimal GetAmountByDepatrment(TransactionReportGroupedDTO tr, int subValue)
        {
            return tr.Transactions.Where(a => a.DepartmentID == subValue).Sum(d => d.Amount);
        }

        private decimal GetAmountByMethod(TransactionReportGroupedDTO tr, int subValue)
        {
            return tr.Transactions.Where(w => w.PaymentMethodID == subValue).Sum(e => e.Amount);
        }

        private decimal GetAmountBySolicitor(TransactionReportGroupedDTO tr, int subValue)
        {
            return tr.Transactions.Where(trans => trans.SolicitorID == subValue).Sum(trans => trans.Amount);
        }

        private decimal GetAmountByCategory(TransactionReportGroupedDTO tr, int subValue, FilterTransactionReport filter)
        {

            decimal amount = 0;
            foreach (TransactionsReportList trans in tr.Transactions)
            {
                if (string.Equals(filter.subtotalBy, "Subcategory", StringComparison.InvariantCultureIgnoreCase))
                {
                    var subCat = trans.Details[0];
                    if (subCat.CategoryID == subValue) amount += trans.Amount;
                }
                else
                {
                    amount += trans.Details.Where(e => e.CategoryID == subValue).Sum(det => det.Amount);
                }
            }
            return amount;
        }


        #endregion

        #region translation

        string GetTranslation(string toTranslate)
        {
            if (toTranslate == null) throw new ArgumentNullException();
            return _translator.Invoke(toTranslate);
        }

        string GetSolicitorName(int? solId)
        {
            var result = NameHelper.GetSolicitorName(solId, _solicitors.ToList());

            return string.IsNullOrEmpty(result) ? GetTranslation("report_no_solicitors") : result;
        }

        string GetCategoryName(int? catId)
        {
            var result = NameHelper.GetCategoryName(catId, _categoryTree.ToList());
            return string.IsNullOrEmpty(result) ? GetTranslation("report_no_category") : result;
        }

        string GetSubCategoryName(int? subcatId)
        {
            var result = NameHelper.GetSubCategoryName(subcatId, _categoryTree.ToList());
            return string.IsNullOrEmpty(result) ? GetTranslation("report_no_subcategory") : result;
        }

        string GetMethodName(int? metId)
        {
            var result = NameHelper.GetMethodName(metId, _paymentMethods.ToList());
            return string.IsNullOrEmpty(result) ? GetTranslation("report_no_paymenMethod") : result;
        }
        string GetMailingName(int? mailingId)
        {
            var result = NameHelper.GetMailingName(mailingId, _mailings.ToList());
            return string.IsNullOrEmpty(result) ? GetTranslation("report_no_mailingDesc") : result;
        }

        string GetDepartmentName(int? depId)
        {
            var result = NameHelper.GetDepartmentName(depId, _departments.ToList());
            return string.IsNullOrEmpty(result) ? GetTranslation("report_no_department") : result;
        }

        private int GetWeekOfYear(DateTime date)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            if (dfi != null)
            {
                Calendar cal = dfi.Calendar;
                return cal.GetWeekOfYear(date, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
            }
            return -1;
        }

        string GetTotalName(string name)
        {
            return "Total " + name;
        }
        #endregion
    }
}