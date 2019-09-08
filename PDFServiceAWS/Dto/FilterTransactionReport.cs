using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PDFServiceAWS.Enums;

namespace PDFServiceAWS.Dto
{
    
    public class FilterTransactionReport : FilterContactReport
    {
         
        public bool? ExByTransSubCat { get; set; }

         
        public DateTime? DateDueFrom { get; set; }
         
        public DateTime? DateDueTo { get; set; }

        /// <summary>
        /// Billed - use onli billed transactions
        /// Due - use all transactions
        /// </summary>
         
        public int? DueBilled { get; set; }

         
        public int? PaidStatus { get; set; }
        /// <summary>
        ///0 false 1 -true used for calculating amount value
        /// </summary>
         
        public bool? CalcTotalSum { get; set; }
        /// <summary>
        /// Credit cards, cash, check, other
        /// </summary>
         
        public int?[] MethodTypes { get; set; }

         
        public int?[] SolicitorIDs { get; set; }

         
        public bool? ExSolicitors { get; set; }
         
        public int?[] DepartmentIDs { get; set; }
         
        public bool? ExByDepartment { get; set; }
         
        public int?[] ExFamilyIds { get; set; }
         
        public List<int> FamilyIds { get; set; }
        /// <summary>
        /// used for checking if we need get family members first
        /// </summary>
         
        public bool HasFamilyCriterias { get; set; }
        /// <summary>
        /// If bill has a card and date due more than current, will skip that bill
        /// </summary>
         
        public bool? ExcludeBillsWithCards { get; set; }

         
        public int? ReceiptNumberMin { get; set; }
         
        public int? ReceiptNumberMax { get; set; }
        //saving addition params
        /// <summary>
        ///transaction report type 
        ///  1- Payment
        ///  2 - Bill
        /// </summary>
         
        public int? type { get; set; }

        /// <summary>
        /// Show detail or total report
        /// 0 - details
        /// 1 - total
        /// </summary>
         
        public TransFilterView? view { get; set; }

        /// <summary>
        /// use for matrix view
        /// </summary>
         
        public string totalOnlyBy { get; set; }

        /// <summary>
        ///use for grouping 
        /// </summary>
         
        public string subtotalBy { get; set; }

        /// <summary>
        /// if we have some family id from other criteria, it will be work like "OR"
        /// in other case lit will be wor like "AND"
        /// </summary>
         
        public bool? FamilyOtherCriteria { get; set; }

         
        public bool ShowDetails { get; set; }
         
        public bool ShowUnasinged { get; set; }

         
        public bool? ReturnOnlyTransactionIds { get; set; }
    }
}