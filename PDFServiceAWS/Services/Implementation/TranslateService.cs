using System.Resources;
using Microsoft.Extensions.Localization;

namespace PDFServiceAWS.Services.Implementation
{
    public class TranslateService { 
        private readonly IStringLocalizer<TranslateService> _localizer;
        public TranslateService(IStringLocalizer<TranslateService> localizer)
        {
            _localizer = localizer;
        }

        public string GetTranslation(string translateFrom)
        {
            if (string.IsNullOrEmpty(translateFrom)) return string.Empty;
            var result = _localizer[translateFrom];
            return string.IsNullOrEmpty(result) ? translateFrom : result;
        }
    }
}