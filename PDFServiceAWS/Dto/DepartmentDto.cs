using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    
    public class DepartmentDto
    {
         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? DepartmentId { get; set; }

         
        public string Name { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string City { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string State { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Zip { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Phone { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Website { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public short? MerchantAccountId { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MerchantAccount { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Country { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Fax { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Note { get; set; }

         
        public bool IsPrimary { get; set; }

         
        public int PDFSettingID { get; set; }

         
        public PdfSettingDto PdfSettings { get; set; }
    }
}