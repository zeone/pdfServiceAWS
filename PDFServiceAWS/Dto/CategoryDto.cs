using System.Runtime.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    [DataContract]
    public class CategoryDto
    {
        [DataMember]
        public int? CategoryId { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public bool IsActive { get; set; }

        [DataMember]
        public bool IsReceipt { get; set; }

        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? DefaultLetterId { get; set; }

        [DataMember]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? DefaultDepartmentId { get; set; }

        [DataMember]
        [XmlElement("Subcategory")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SubcategoryDto[] Subcategories { get; set; }
    }
}