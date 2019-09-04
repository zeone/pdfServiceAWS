using System;
using PDFServiceAWS.Enums;
using PDFServiceAWS.Services;

namespace PDFServiceAWS.Helpers
{
    public class TimeHelper
    {
        private readonly IAppSettingsProvider _settingsProvider;

        public TimeHelper(string schema)
        {
            _settingsProvider = NinjectBulder.Container.Get<IAppSettingsProvider>(new ConstructorArgument("schema", schema));
        }


        /// <summary>
        /// Return time by org time zone
        /// </summary>
        /// <param name="orgTime"></param>
        /// <returns></returns>
        public bool TryConvertLocalTimeToOrgTime(out DateTime orgTime)
        {
            orgTime = DateTime.MinValue;
            TimeZoneInfo orgTimeZone;
            try
            {
                var timeZoneId = _settingsProvider.GetSetting(Settings.Timezone).Value;
                if (string.IsNullOrEmpty(timeZoneId)) return false;
                orgTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
            catch (Exception)
            {
                return false;
            }

            orgTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, orgTimeZone);
            return true;
        }




    }
}