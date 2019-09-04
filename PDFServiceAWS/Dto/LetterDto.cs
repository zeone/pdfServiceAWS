using System;
using Newtonsoft.Json;

namespace PDFServiceAWS.Dto
{
    public class LetterDto
    {
     
        public short? LetterId { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateChanged { get; set; }

        public string LetterText { get; set; }

        public string LetterName { get; set; }

        public byte LetterTypeId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string LetterType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Note { get; set; }

        /// <summary>
        ///  one Default Letter for each letter type
        /// </summary>
        public bool IsDefault { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Subject { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public string FromName { get; set; }

        public string FromEmail { get; set; }

        public string BackgroundImg { get; set; }

        public int? PDFSettingID { get; set; }

        public string PDFLetterText { get; set; }

        public PdfSettingDto PdfSettings { get; set; }
    }
}