using System;
using System.Runtime.Serialization;

namespace PDFServiceAWS.Dto
{
    [DataContract]
    public class PdfSettingDto
    {
        [DataMember]
        public string TemplateName { get; set; }
        [DataMember]
        public int? PDFSettingID { get; set; }
        [DataMember]
        public string Template { get; set; }
        [DataMember]
        public double? PageHeight { get; set; }
        [DataMember]
        public double? PageWidth { get; set; }
        [DataMember]
        public string BodyText { get; set; }
        [DataMember]
        public double LeftMargin { get; set; }
        [DataMember]
        public double TopMargin { get; set; }
        [DataMember]
        public double RightMargin { get; set; }
        [DataMember]
        public double BottomMargin { get; set; }
        [DataMember]
        public string FontName { get; set; }
        [DataMember]
        public int FontSize { get; set; }
        [DataMember]
        public double BlockSpacing { get; set; }
        [DataMember]
        public ParagraphAlignment Aligment { get; set; }
        [DataMember]
        public double LineSpacing { get; set; }
        [DataMember]
        public double FirstLineIntent { get; set; }
        [DataMember]
        public bool HasReceiptStub { get; set; }
        [DataMember]
        //Date
        public bool ShowDate { get; set; }
        [DataMember]
        public bool UseLongDateFormat { get; set; }
        [DataMember]
        public bool ShowHebrewDate { get; set; }
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public double DateIndent { get; set; }
        [DataMember]
        public ParagraphAlignment DateAligment { get; set; }

        // address
        [DataMember]
        public bool ShowAddress { get; set; }
        [DataMember]
        public string Label { get; set; }
        [DataMember]
        public string CSZ { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string State { get; set; }
        [DataMember]
        public string Zip { get; set; }
        [DataMember]
        public int AddressId { get; set; }
        [DataMember]
        public string CompanyName { get; set; }
        [DataMember]
        public string Address1 { get; set; }
        [DataMember]
        public string Address2 { get; set; }
        [DataMember]
        public double AddressIndent { get; set; }
        [DataMember]
        public ParagraphAlignment AddressAligment { get; set; }
        //Signature
        [DataMember]
        public bool ShowSingnature { get; set; }
        [DataMember]
        public string SignatureImage { get; set; }
        [DataMember]
        public double? SignatureHeight { get; set; }
        [DataMember]
        public double? SignatureWidth { get; set; }
        [DataMember]
        public double SignatureIndent { get; set; }
        [DataMember]
        public string Closing { get; set; }
        [DataMember]
        public double ClosingAdj { get; set; }
        [DataMember]
        public string SignatureName { get; set; }
        [DataMember]
        public double SignatureNameAdj { get; set; }
        [DataMember]
        public string SignatureTitle { get; set; }
        [DataMember]
        public string PS { get; set; }
    }
}