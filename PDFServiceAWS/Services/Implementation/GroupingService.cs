using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PDFServiceAWS.DB.SP;
using PDFServiceAWS.Dto;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public class GroupingService : BaseService, IGroupingService
    {
        private string _schema;
        public GroupingService(IQueryProvider queryProvider, string schema) : base(queryProvider)
        {
            _schema = schema;
        }
        
        public CategoryDto[] GetCategoryTree()
        {
            var selectTransactionCategoryTreeSp =
                QueryProvider.CreateQuery<SelectTransactionCategoryTreeStoredProcedure>(_schema);

            var result = selectTransactionCategoryTreeSp.Execute();
            return result.ResultToArray<CategoryDto>();
        }
        
    }
}