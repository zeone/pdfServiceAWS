using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    [DataContract]
    public class PaymentMethodDto
    {
        [DataMember]
        [JsonProperty(PropertyName = "PaymentMethodId")]
        public int PaymentMethodId { get; set; }

        [DataMember]
        public string Method { get; set; }

        [DataMember]
        public byte MethodTypeId { get; set; }

        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string MethodType { get; set; }
    }
}