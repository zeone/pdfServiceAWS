using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    public class SubclassDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [XmlElement("SubclassId")]
        public int SubclassId { get; set; }

        [XmlElement("ClassId")]
        public int ClassId { get; set; }

        [XmlElement("Subclass")]
        public string Subclass { get; set; }

        [XmlElement("IsActive")]
        public bool IsActive { get; set; }
    }
}