using System.Resources;

namespace PDFServiceAWS.Helpers
{
    public static class TranslateHelper
    {
        public static string GetTranslation(string translateFrom)
        {
            if (string.IsNullOrEmpty(translateFrom)) return string.Empty;
            ResourceManager resource = new ResourceManager("Resources.Global", global::System.Reflection.Assembly.Load("App_GlobalResources"));
            var result = resource.GetString(translateFrom);
            return string.IsNullOrEmpty(result) ? translateFrom : result;
        }
    }
}