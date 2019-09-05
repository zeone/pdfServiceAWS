using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PDFServiceAWS.Enums;

namespace PDFServiceAWS.Dto
{
    [DataContract]
    public class FilterContactReport
    {
        /// 0 - family, 1 - Member for contacts
        ///  0 - bill 1 - payment, 2 - payment/bills for transactions

        [DataMember]
        public TransFilterType ReportType { get; set; }
        [DataMember]
        public bool? GlobalAndOr { get; set; }
        [DataMember]
        //family categories
        public int?[] FCategoryIds { get; set; }
        [DataMember]
        public int?[] ExcludeFCategoryIds { get; set; }
        [DataMember]
        public DateTime? FCatEnrolledDate { get; set; }
        /// false - AND, true - OR
        [DataMember]
        public bool? FCatAndOr { get; set; }
        [DataMember]
        public bool? FCatExAndOr { get; set; }
        [DataMember]
        public bool FCatCurrent { get; set; }
        //member categories
        [DataMember]
        public int?[] MCategoryIds { get; set; }
        [DataMember]
        public int?[] ExcludeMCategoryIds { get; set; }
        [DataMember]
        public DateTime? MCatEnrolledDate { get; set; }
        /// false - AND, true - OR
        [DataMember]
        public bool? MCatAndOr { get; set; }
        [DataMember]
        public bool? MCatExAndOr { get; set; }
        [DataMember]
        public bool MCatCurrent { get; set; }
        //member criterias
        [DataMember]
        public int?[] RelationsLimit { get; set; }
        [DataMember]
        public bool? NotBirthDate { get; set; }
        [DataMember]
        public string Gender { get; set; }
        [DataMember]
        public DateTime? MinBirthDate { get; set; }
        [DataMember]
        public DateTime? MaxBirthDate { get; set; }

        /// 0 - not include 1 - include
        [DataMember]
        public int? MemberType { get; set; }

        // Contacts params

        ///1 - use only primary contacts, 0 - use all contacts
        [DataMember]
        public bool? PrimaryOnly { get; set; }

        ///0 - use only aloowed contacts, 1 - use all contacts
        [DataMember]
        public bool? ExcludeNC { get; set; }

        //Address 
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public bool? ExCountry { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public bool? ExCity { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public bool? ExState { get; set; }
        [DataMember]
        public string[] Zip { get; set; }
        [DataMember]
        public bool? ExZip { get; set; }

        /// false - AND, true - OR
        [DataMember]
        public bool? AddressAndOr { get; set; }
        /// false - all addresses, true - only primary addresses
        [DataMember]
        public bool AddressPrymary { get; set; }
        /// false - all addresses, true - only addresses marked for mail
        [DataMember]
        public bool? AddressExNM { get; set; }
        ///0 - any, 1 - home 2 - work
        [DataMember]
        public int? AddressType { get; set; }
        //   params for complete address
        /// Filter by requaired fields
        [DataMember]
        public bool? AddIsComplete { get; set; }
        /// false - skip, true - must contain
        [DataMember]
        public bool? AddCPAddress { get; set; }
        /// false - skip, true - must contain
        [DataMember]
        public bool? AddCPCity { get; set; }
        /// false - skip, true - must contain
        [DataMember]
        public bool? AddCPState { get; set; }
        /// false - skip, true - must contain
        [DataMember]
        public bool? AddCPZip { get; set; }

        //Transactions
        [DataMember]
        public bool? NotTransactions { get; set; }
        [DataMember]
        public int?[] TransSubCatIDs { get; set; }
        ///1 - payment, 2 - bill
        [DataMember]
        public int? PaymentOrBill { get; set; }
        ///0-SUM, 1-MAX, 2-LAST
        [DataMember]
        public int? TransactionSort { get; set; }
        [DataMember]
        public decimal? MinSum { get; set; }
        [DataMember]
        public decimal? MaxSum { get; set; }

        [DataMember]
        public DateTime? TransactionStartDate { get; set; }
        [DataMember]
        public DateTime? TransactionEndDate { get; set; }

        //Other
        /// sorted member by modify date
        [DataMember]
        public DateTime? CreateFrom { get; set; }

        [DataMember]
        public DateTime? CreateTo { get; set; }

        /// sorted member by modify date
        [DataMember]
        public DateTime? ModifyFrom { get; set; }

        [DataMember]
        public DateTime? ModifyTo { get; set; }

        /// Sorted member by last name
        [DataMember]
        public char? StartChar { get; set; }

        [DataMember]
        public char? EndChar { get; set; }

        [DataMember]
        public IEnumerable<ReportColumn> Columns { get; set; }
        ///additional info
        [DataMember]
        public string Name { get; set; }
        //// 1 - Contact 2 - transactions
        [JsonIgnore]
        [DataMember]
        public int Type { get; set; }
        [JsonIgnore]
        [DataMember]
        public int? CountRows { get; set; }

        [DataMember]
        public bool ShowAll { get; set; }
        [DataMember]
        public bool SortReverse { get; set; }
        [DataMember]
        public string SortType { get; set; }
        [DataMember]
        public string GroupBy { get; set; }
        [DataMember]
        public bool SkipTitles { get; set; }
        [DataMember]
        public bool ShowNC { get; set; }

        [DataMember]
        public bool? IncludeDeceased { get; set; }
    }

    public class ReportColumn
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public ReportColumns? Column { get; set; }
        [DataMember]
        public TransactioReportColumns? TransactionColumn { get; set; }
        [DataMember]
        public ReportColumnsFilter Filter { get; set; }
        [DataMember]
        public int Position { get; set; }
        [DataMember]
        public bool IsChecked { get; set; }
        [DataMember]
        public bool ColumnOnly { get; set; }
        [DataMember]
        public bool IsContact { get; set; }
        [DataMember]
        public ReportTypes ReportType { get; set; }
        [DataMember]
        public TransFilterType? TransType { get; set; }
        [DataMember]
        public string Sort { get; set; }
    }
}