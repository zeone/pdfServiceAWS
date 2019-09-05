using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.Services
{
    public interface ILetterService : IAppService
    {
        /// <summary>
        /// Get pdfSetting by Id
        /// </summary>
        /// <param name="id">pdf setting id</param>
        /// <returns></returns>
        PdfSettingDto GetPdfSetting(int id);

        /// <summary>
        /// Get letter by id.
        /// </summary>
        /// <returns>Letter</returns>
        LetterDto GetLetterById(short id);

        /// <summary>
        /// Gets the fields data to fill letter template
        /// </summary>
        /// <param name="transactionId">Transaction id for witch leter we are preparing</param>
        /// <returns>Return fields</returns>
        LetterFieldsDto GetLetterFields(int transactionId, int?[] subCategoryIds = null, bool includeSubCats = true, string currency = "$");

    }
}
