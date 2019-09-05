using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PDFServiceAWS.DB.SP;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Helpers;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;


namespace PDFServiceAWS.Services.Implementation
{
    public class AppSettingsProvider : BaseService, IAppSettingsProvider
    {
        private string _schema;
        public AppSettingsProvider(IQueryProvider queryProvider, string schema)
            : base( queryProvider)
        {
            _schema = schema;
        }

        public AppSettingRecord GetSetting(string setting)
        {
            if (string.IsNullOrEmpty(setting))
            {

                throw new ArgumentException("Parameter [setting] is not defined", "setting");
            }
            IEnumerable<AppSettingRecord> result = GetSettings(null, setting);
            return result.FirstOrDefault<AppSettingRecord>();
        }

        public bool CreateSetting(AppSettingRecord NewSetting)
        {
            var query = QueryProvider.CreateQuery<InsertSettingStoredProcedure>(_schema);
            // applying parameters
            query.SettingID = NewSetting.SettingID;
            query.Setting = NewSetting.Setting;
            query.Value = NewSetting.Value;
            query.ParentID = NewSetting.ParentID;
            return !query.Execute().NonZeroReturnCode;
        }

        public IEnumerable<AppSettingRecord> GetSettings(short? settingID = 0, string setting = null)
        {
            var spGetSetting = QueryProvider.CreateQuery<SelectSettingsStoredProcedure>(_schema);
            spGetSetting.Setting = setting;
            spGetSetting.SettingID = settingID;
            var res = spGetSetting.Execute();
            return !res.HasNoDataRows ? res.ResultToArray<AppSettingRecord>() : ArrayUtils.Empty<AppSettingRecord>();
        }
    }
}