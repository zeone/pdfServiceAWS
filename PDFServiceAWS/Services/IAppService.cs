using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace PDFServiceAWS.Services
{
    public interface IAppService
    {

    }
    public interface IWorkingContext
    {
        /// <summary>
        /// Get a CMS user currently executing application code
        /// </summary>
        // CmsUser CurrentUser { get; }

        /// <summary>
        /// Get the current HTTP context 
        /// </summary>
        HttpContext HttpContext { get; }
    }
}
