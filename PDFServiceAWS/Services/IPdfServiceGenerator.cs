using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PDFServiceAWS.Services.Implementation.PdfServiceFormats;

namespace PDFServiceAWS.Services
{
    public interface IPdfServiceGenerator
    {
        /// <summary>
        /// generate single pdf document with labels 
        /// </summary>
        /// <param name="labels"> text for labels - recipient addresses </param>
        /// <returns></returns>
        byte[] GeneratePDFLabels(Dictionary<int, EnvelopeAddress> labels);

        /// <summary>
        /// generate single pdf document with envelopes
        /// </summary>
        /// <param name="envelopes"> texts for recipient/sender address area for each envelope</param>
        /// <returns></returns>
        byte[] GeneratePDFEnvelopes(Dictionary<int, EnvelopeAddress> envelopes);

        /// <summary>
        /// generate single pdf document with letters from list
        /// </summary>
        /// <param name="letters"> letter body</param>
        /// <returns></returns>
        byte[] GeneratePDFLetters(List<string> letters);

        /// <summary>
        /// Render pdf document
        /// </summary>
        /// <param name="text">Text that should be in document</param>
        /// <returns>Path to this document</returns>
        string PdfDoc(string text, int transactionID, string userId);

        /// <summary>
        /// Render pdf document for display in a browser
        /// </summary>
        /// <param name="textHtml">Text that should be in document</param>
        /// <returns>Bytes array encoded in Base64</returns>
        byte[] PdfDoc(object textHtml);

        byte[] PdfDoc(PdfDto dto, bool isBase64Encode = true, string currency = "$");

        byte[] PdfDoc(BatchLetterDto dto, bool isBase64Encode = true, string currency = "$");

        byte[] PdfTemplateBatch(List<PdfSettingsBatchDto> settings);

        PdfSettingDto PreparePdfSetting(LetterDto letter, DepartmentDto department,
            List<FamilyAddressDto> addresses, PaymentDto transaction, LetterFieldsDto fields);

        byte[] PdfPreview(PdfSettingDto setting, bool isBase64Encode = true, string currency = "$");

        string PdfDocWithPath(BatchLetterDto dto, string userId);

        string PdfDocPath(BatchLetterDto dto, string path, string currency = "$");

        byte[] PdfEndYearBatch(List<PdfSettingsBatchDto> settings);
    }
}