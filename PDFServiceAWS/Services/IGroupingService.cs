using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.Services
{
    public interface IGroupingService
    {
 
        /// <summary>
        /// List of Category and Subcategory records belongigng
        /// to Categories and Subcategories
        /// </summary>
        /// <returns></returns>
        CategoryDto[] GetCategoryTree();
       
    }
}
