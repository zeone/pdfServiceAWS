using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    public class ClassDto
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int ClassId { get; set; }

    
        public string Class { get; set; }

  
        public bool IsActive { get; set; }

        [XmlElement("Subclass")]
        public SubclassDto[] Subclasses { get; set; }
    }
}