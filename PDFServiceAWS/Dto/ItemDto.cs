using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace PDFServiceAWS.Dto
{
    
    [XmlRoot("Item")]
    public class ItemDto
    {
         
        [XmlElement("ItemID")]
        public int? ItemId { get; set; }

         
        [XmlElement("SubcategoryId")]
        public int SubcategoryId { get; set; }

         
        [XmlElement("ItemName")]
        public string ItemName { get; set; }

         
        [XmlElement("Amount")]
        public decimal Amount { get; set; }

         
        [XmlElement("IsActive")]
        public bool IsActive { get; set; }
    }
}