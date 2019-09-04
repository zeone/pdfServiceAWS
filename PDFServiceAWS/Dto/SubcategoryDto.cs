using System.Runtime.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    [DataContract]
    [XmlRoot("Subcategory")]
    public class SubcategoryDto
    {
        [DataMember]
        [XmlElement("CategoryId")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? CategoryId { get; set; }

        [DataMember]
        [XmlElement("SubcategoryId")]
        public int? SubcategoryId { get; set; }

        [DataMember]
        [XmlElement("Subcategory")]
        public string Subcategory { get; set; }

        [DataMember]
        [XmlElement("IsActive")]
        public bool IsActive { get; set; }

        [DataMember]
        [XmlElement("DefaultLetterId")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? DefaultLetterId { get; set; }

        [DataMember]
        [XmlElement("Item", Type = typeof(ItemDto))]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ItemDto[] Items { get; set; }

    }
}