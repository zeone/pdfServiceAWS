using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PDFServiceAWS.Enums;

namespace PDFServiceAWS.Dto
{
    
    public class ReportDto
    {
         
        FilterTransactionReport _filter;
         
        public int ReportId { get; set; }
         
        public ReportTypes ReportType { get; set; }
         
        public string Name { get; set; }
         
        public DateTime? Created { get; set; }
         
        public string Author { get; set; }
         
        [JsonIgnore]
        public string Filter { get; set; }
         
        public ExportReportPreference AdditionalPreferences { get; set; }

         
        public FilterTransactionReport Criteria
        {
            get
            {
                if (_filter == null && Filter != null)
                    _filter = JsonConvert.DeserializeObject<FilterTransactionReport>(Filter);
                return _filter;
                //return Filter == null ? new FilterContactReport() : JsonConvert.DeserializeObject<FilterContactReport>(Filter);
            }
            set
            {
                _filter = value;

                if (_filter != null) Filter = JsonConvert.SerializeObject(value);
            }
        }

         
        public string Country { get; set; }

        
        public class ExportReportPreference
        {
             
            public string ExportType { get; set; }
             
            public string CustomAddress { get; set; }
             
            public int? FamilyAddressId { get; set; }
             
            public int? FamilyId { get; set; }

        }
    }
}