using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    
    public class MailingDto
    {
         
        public int? MailingId { get; set; }

         
        public string Name { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? Date { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Note { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? TotalPieces { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public decimal? Cost { get; set; }
    }
}