using System;
using System.Runtime.Serialization;
using MigraDoc.DocumentObjectModel;

namespace PDFServiceAWS.Dto
{
    
    public class PdfSettingDto
    {
         
        public string TemplateName { get; set; }
         
        public int? PDFSettingID { get; set; }
         
        public string Template { get; set; }
         
        public double? PageHeight { get; set; }
         
        public double? PageWidth { get; set; }
         
        public string BodyText { get; set; }
         
        public double LeftMargin { get; set; }
         
        public double TopMargin { get; set; }
         
        public double RightMargin { get; set; }
         
        public double BottomMargin { get; set; }
         
        public string FontName { get; set; }
         
        public int FontSize { get; set; }
         
        public double BlockSpacing { get; set; }
         
        public ParagraphAlignment Aligment { get; set; }
         
        public double LineSpacing { get; set; }
         
        public double FirstLineIntent { get; set; }
         
        public bool HasReceiptStub { get; set; }
         
        //Date
        public bool ShowDate { get; set; }
         
        public bool UseLongDateFormat { get; set; }
         
        public bool ShowHebrewDate { get; set; }
         
        public DateTime Date { get; set; }
         
        public double DateIndent { get; set; }
         
        public ParagraphAlignment DateAligment { get; set; }

        // address
         
        public bool ShowAddress { get; set; }
         
        public string Label { get; set; }
         
        public string CSZ { get; set; }
         
        public string Country { get; set; }
         
        public string City { get; set; }
         
        public string State { get; set; }
         
        public string Zip { get; set; }
         
        public int AddressId { get; set; }
         
        public string CompanyName { get; set; }
         
        public string Address1 { get; set; }
         
        public string Address2 { get; set; }
         
        public double AddressIndent { get; set; }
         
        public ParagraphAlignment AddressAligment { get; set; }
        //Signature
         
        public bool ShowSingnature { get; set; }
         
        public string SignatureImage { get; set; }
         
        public double? SignatureHeight { get; set; }
         
        public double? SignatureWidth { get; set; }
         
        public double SignatureIndent { get; set; }
         
        public string Closing { get; set; }
         
        public double ClosingAdj { get; set; }
         
        public string SignatureName { get; set; }
         
        public double SignatureNameAdj { get; set; }
         
        public string SignatureTitle { get; set; }
         
        public string PS { get; set; }
    }
}