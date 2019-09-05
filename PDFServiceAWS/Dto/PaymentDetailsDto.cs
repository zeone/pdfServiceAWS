using System.ComponentModel;
using System.Xml.Serialization;
using Newtonsoft.Json;
using PDFServiceAWS.Enums;

namespace PDFServiceAWS.Dto
{
    [XmlRoot("Payment")]
    public class PaymentDetailsDto
    {
        [XmlElement("TransactionDetailID")]
        public int? TransactionDetailId { get; set; }

        [XmlElement("BillID")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? BillId { get; set; }

     
        [XmlElement("CategoryID")]
        public int CategoryId { get; set; }

        [XmlElement("Category")]
        public string Category { get; set; }

        
        [XmlElement("SubcategoryID")]
        public int SubcategoryId { get; set; }

        [XmlElement("Subcategory")]
        public string Subcategory { get; set; }

        [XmlElement("ClassID")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ClassId { get; set; }

        [XmlElement("Class")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Class { get; set; }

        [XmlElement("SubclassID")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? SubclassId { get; set; }

        [XmlElement("Subclass")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Subclass { get; set; }

     
        [XmlElement("Amount")]
        public decimal Amount { get; set; }

        [XmlElement("Note")]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Note { get; set; }

        [DefaultValue(DtoClientState.Unchanged)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public byte Status { get; set; }

        public bool CardCharged { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ChargeMsg { get; set; }

        [DefaultValue(0)]
        public int ChargeStatus { get; set; }
    }
}