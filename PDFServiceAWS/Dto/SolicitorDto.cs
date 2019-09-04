using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    [DataContract]
    public class SolicitorDto
    {
        [DataMember]
        public int? SolicitorId { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string Label { get { return string.Format("{0} {1}", FirstName, LastName); } }

        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MemberId { get; set; }
    }
}