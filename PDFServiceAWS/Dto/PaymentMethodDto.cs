using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    
    public class PaymentMethodDto
    {
         
        [JsonProperty(PropertyName = "PaymentMethodId")]
        public int PaymentMethodId { get; set; }

         
        public string Method { get; set; }

         
        public byte MethodTypeId { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MethodType { get; set; }
    }
}