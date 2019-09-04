using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.Services
{
    /// <summary>
    /// Represents a service to perform CRUD operstion to Solicitors table
    /// </summary>
    public interface ISolicitorService : IAppService
    {
        /// <summary>
        /// Get all solicitors from database
        /// </summary>
        /// <returns>List of <see cref="SolicitorDto"/></returns>
        IEnumerable<SolicitorDto> GetAllSolicitors();

    }
}
