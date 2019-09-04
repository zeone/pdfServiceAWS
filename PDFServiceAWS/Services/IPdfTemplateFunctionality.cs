using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDFServiceAWS.Services
{
    public interface IPdfTemplateFunctionality
    {
        byte[] GetPdfFromTamplate(PdfSettingDto settings, string countryInSettings, LetterFieldsDto fields);
        byte[] GetBatchPdfFromTemplate(List<PdfSettingsBatchDto> pdfSettings, string countryInSettings);
        byte[] GetPdfPreview(PdfSettingDto settings, string countryInSettings, LetterFieldsDto fields);

        string GetBatchPdfFromTamplate(PdfSettingDto settings, string countryInSettings, LetterFieldsDto fields,
            string path);

        byte[] GetBatchEndYearPdfFromTemplate(List<PdfSettingsBatchDto> pdfSettings, string countryInSettings);
    }
}
