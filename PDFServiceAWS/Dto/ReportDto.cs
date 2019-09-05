using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using PDFServiceAWS.Enums;

namespace PDFServiceAWS.Dto
{
    [DataContract]
    public class ReportDto
    {
        [DataMember]
        FilterTransactionReport _filter;
        [DataMember]
        public int ReportId { get; set; }
        [DataMember]
        public ReportTypes ReportType { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime? Created { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        [JsonIgnore]
        public string Filter { get; set; }
        [DataMember]
        public ExportReportPreference AdditionalPreferences { get; set; }

        [DataMember]
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

        [DataMember]
        public string Country { get; set; }

        [DataContract]
        public class ExportReportPreference
        {
            [DataMember]
            public string ExportType { get; set; }
            [DataMember]
            public string CustomAddress { get; set; }
            [DataMember]
            public int? FamilyAddressId { get; set; }
            [DataMember]
            public int? FamilyId { get; set; }

        }
    }
}