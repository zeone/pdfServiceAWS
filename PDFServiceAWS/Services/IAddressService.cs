using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.Services
{
    public interface IAddressService : IAppService
    {
  /// <summary>
        /// Return all addressesfor family
        /// </summary>
        /// <param name="famId"></param>
        /// <returns></returns>
        List<FamilyAddressDto> GetAddressesByFamilyId(int famId);
       
    }
}
