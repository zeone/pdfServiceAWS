using System.Runtime.Serialization;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    
    public class CategoryDto
    {
         
        public int? CategoryId { get; set; }

         
        public string Category { get; set; }

         
        public bool IsActive { get; set; }

         
        public bool IsReceipt { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? DefaultLetterId { get; set; }

         
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? DefaultDepartmentId { get; set; }

         
        [XmlElement("Subcategory")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public SubcategoryDto[] Subcategories { get; set; }
    }
}