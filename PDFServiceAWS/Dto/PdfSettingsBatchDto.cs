namespace PDFServiceAWS.Dto
{
    public class PdfSettingsBatchDto
    {

        public PdfSettingDto Settings { get; set; }
        public string CountryInSettings { get; set; }
        public LetterFieldsDto Fields { get; set; }
        public int LetterTypeId { get; set; }
    }
}