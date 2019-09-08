using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PDFServiceAWS.Enums;

namespace PDFServiceAWS.Dto
{
    
    public class FilterContactReport
    {
        /// 0 - family, 1 - Member for contacts
        ///  0 - bill 1 - payment, 2 - payment/bills for transactions

         
        public TransFilterType ReportType { get; set; }
         
        public bool? GlobalAndOr { get; set; }
         
        //family categories
        public int?[] FCategoryIds { get; set; }
         
        public int?[] ExcludeFCategoryIds { get; set; }
         
        public DateTime? FCatEnrolledDate { get; set; }
        /// false - AND, true - OR
         
        public bool? FCatAndOr { get; set; }
         
        public bool? FCatExAndOr { get; set; }
         
        public bool FCatCurrent { get; set; }
        //member categories
         
        public int?[] MCategoryIds { get; set; }
         
        public int?[] ExcludeMCategoryIds { get; set; }
         
        public DateTime? MCatEnrolledDate { get; set; }
        /// false - AND, true - OR
         
        public bool? MCatAndOr { get; set; }
         
        public bool? MCatExAndOr { get; set; }
         
        public bool MCatCurrent { get; set; }
        //member criterias
         
        public int?[] RelationsLimit { get; set; }
         
        public bool? NotBirthDate { get; set; }
         
        public string Gender { get; set; }
         
        public DateTime? MinBirthDate { get; set; }
         
        public DateTime? MaxBirthDate { get; set; }

        /// 0 - not include 1 - include
         
        public int? MemberType { get; set; }

        // Contacts params

        ///1 - use only primary contacts, 0 - use all contacts
         
        public bool? PrimaryOnly { get; set; }

        ///0 - use only aloowed contacts, 1 - use all contacts
         
        public bool? ExcludeNC { get; set; }

        //Address 
         
        public string Country { get; set; }
         
        public bool? ExCountry { get; set; }
         
        public string City { get; set; }
         
        public bool? ExCity { get; set; }
         
        public string State { get; set; }
         
        public bool? ExState { get; set; }
         
        public string[] Zip { get; set; }
         
        public bool? ExZip { get; set; }

        /// false - AND, true - OR
         
        public bool? AddressAndOr { get; set; }
        /// false - all addresses, true - only primary addresses
         
        public bool AddressPrymary { get; set; }
        /// false - all addresses, true - only addresses marked for mail
         
        public bool? AddressExNM { get; set; }
        ///0 - any, 1 - home 2 - work
         
        public int? AddressType { get; set; }
        //   params for complete address
        /// Filter by requaired fields
         
        public bool? AddIsComplete { get; set; }
        /// false - skip, true - must contain
         
        public bool? AddCPAddress { get; set; }
        /// false - skip, true - must contain
         
        public bool? AddCPCity { get; set; }
        /// false - skip, true - must contain
         
        public bool? AddCPState { get; set; }
        /// false - skip, true - must contain
         
        public bool? AddCPZip { get; set; }

        //Transactions
         
        public bool? NotTransactions { get; set; }
         
        public int?[] TransSubCatIDs { get; set; }
        ///1 - payment, 2 - bill
         
        public int? PaymentOrBill { get; set; }
        ///0-SUM, 1-MAX, 2-LAST
         
        public int? TransactionSort { get; set; }
         
        public decimal? MinSum { get; set; }
         
        public decimal? MaxSum { get; set; }

         
        public DateTime? TransactionStartDate { get; set; }
         
        public DateTime? TransactionEndDate { get; set; }

        //Other
        /// sorted member by modify date
         
        public DateTime? CreateFrom { get; set; }

         
        public DateTime? CreateTo { get; set; }

        /// sorted member by modify date
         
        public DateTime? ModifyFrom { get; set; }

         
        public DateTime? ModifyTo { get; set; }

        /// Sorted member by last name
         
        public char? StartChar { get; set; }

         
        public char? EndChar { get; set; }

         
        public IEnumerable<ReportColumn> Columns { get; set; }
        ///additional info
         
        public string Name { get; set; }
        //// 1 - Contact 2 - transactions
        [JsonIgnore]
         
        public int Type { get; set; }
        [JsonIgnore]
         
        public int? CountRows { get; set; }

         
        public bool ShowAll { get; set; }
         
        public bool SortReverse { get; set; }
         
        public string SortType { get; set; }
         
        public string GroupBy { get; set; }
         
        public bool SkipTitles { get; set; }
         
        public bool ShowNC { get; set; }

         
        public bool? IncludeDeceased { get; set; }
    }

    public class ReportColumn
    {
         
        public string Name { get; set; }
         
        public ReportColumns? Column { get; set; }
         
        public TransactioReportColumns? TransactionColumn { get; set; }
         
        public ReportColumnsFilter Filter { get; set; }
         
        public int Position { get; set; }
         
        public bool IsChecked { get; set; }
         
        public bool ColumnOnly { get; set; }
         
        public bool IsContact { get; set; }
         
        public ReportTypes ReportType { get; set; }
         
        public TransFilterType? TransType { get; set; }
         
        public string Sort { get; set; }
    }
}