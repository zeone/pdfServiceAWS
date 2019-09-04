using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    public class PaymentDto
    {
        public int? TransactionId { get; set; }

        public string Family { get; set; }

        public int FamilyId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MemberId { get; set; }


        public int AddressId { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string CheckNo { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? PayId { get; set; }

        public string AuthNumber { get; set; }

        public string AuthTransactionId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public byte? PaymentMethodId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string PaymentMethod { get; set; }


        public bool IsReceipt { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? ReceiptNo { get; set; }


        public bool ReceiptSent { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public short? LetterId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? SolicitorId { get; set; }

        public int DepartmentId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? MailingId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HonorMemory { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Note { get; set; }


        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Subcategory { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Class { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Subclass { get; set; }


        [XmlElement("Payment")]
        public PaymentDetailsDto[] PaymentDetails { get; set; }

        public int? SubmissionId { get; set; }
    }
}