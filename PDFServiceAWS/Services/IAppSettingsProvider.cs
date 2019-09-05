using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.Services
{
    /// <summary>
    /// Service used to fetch application settings 
    /// </summary>
    public interface IAppSettingsProvider : IAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        AppSettingRecord GetSetting(string setting);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="NewSetting"></param>
        /// <returns></returns>
        bool CreateSetting(AppSettingRecord NewSetting);
     
    }
}
