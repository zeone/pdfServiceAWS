using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PDFServiceAWS.Dto
{
    [DataContract]
    public class FilterTransactionReport : FilterContactReport
    {
        [DataMember]
        public bool? ExByTransSubCat { get; set; }

        [DataMember]
        public DateTime? DateDueFrom { get; set; }
        [DataMember]
        public DateTime? DateDueTo { get; set; }

        /// <summary>
        /// Billed - use onli billed transactions
        /// Due - use all transactions
        /// </summary>
        [DataMember]
        public int? DueBilled { get; set; }

        [DataMember]
        public int? PaidStatus { get; set; }
        /// <summary>
        ///0 false 1 -true used for calculating amount value
        /// </summary>
        [DataMember]
        public bool? CalcTotalSum { get; set; }
        /// <summary>
        /// Credit cards, cash, check, other
        /// </summary>
        [DataMember]
        public int?[] MethodTypes { get; set; }

        [DataMember]
        public int?[] SolicitorIDs { get; set; }

        [DataMember]
        public bool? ExSolicitors { get; set; }
        [DataMember]
        public int?[] DepartmentIDs { get; set; }
        [DataMember]
        public bool? ExByDepartment { get; set; }
        [DataMember]
        public int?[] ExFamilyIds { get; set; }
        [DataMember]
        public List<int> FamilyIds { get; set; }
        /// <summary>
        /// used for checking if we need get family members first
        /// </summary>
        [DataMember]
        public bool HasFamilyCriterias { get; set; }
        /// <summary>
        /// If bill has a card and date due more than current, will skip that bill
        /// </summary>
        [DataMember]
        public bool? ExcludeBillsWithCards { get; set; }

        [DataMember]
        public int? ReceiptNumberMin { get; set; }
        [DataMember]
        public int? ReceiptNumberMax { get; set; }
        //saving addition params
        /// <summary>
        ///transaction report type 
        ///  1- Payment
        ///  2 - Bill
        /// </summary>
        [DataMember]
        public int? type { get; set; }

        /// <summary>
        /// Show detail or total report
        /// 0 - details
        /// 1 - total
        /// </summary>
        [DataMember]
        public TransFilterView? view { get; set; }

        /// <summary>
        /// use for matrix view
        /// </summary>
        [DataMember]
        public string totalOnlyBy { get; set; }

        /// <summary>
        ///use for grouping 
        /// </summary>
        [DataMember]
        public string subtotalBy { get; set; }

        /// <summary>
        /// if we have some family id from other criteria, it will be work like "OR"
        /// in other case lit will be wor like "AND"
        /// </summary>
        [DataMember]
        public bool? FamilyOtherCriteria { get; set; }

        [DataMember]
        public bool ShowDetails { get; set; }
        [DataMember]
        public bool ShowUnasinged { get; set; }

        [DataMember]
        public bool? ReturnOnlyTransactionIds { get; set; }
    }
}