using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PDFService.Enums;
using PDFServiceAWS.DB.SP;
using PDFServiceAWS.Helpers;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public class SolicitorService : BaseService, ISolicitorService
    {
        private string _schema;

        public SolicitorService(IQueryProvider queryProvider, string schema) : base( queryProvider)
        {
            _schema = schema;
        }

        public IEnumerable<SolicitorDto> GetAllSolicitors()
        {
            return GetSolicitors(null);
        }

        private IEnumerable<SolicitorDto> GetSolicitors(int? solicitorId)
        {
            var selectSolicitorsSp = QueryProvider.CreateQuery<SelectSolicitorsStoredProcedure>(_schema);
            selectSolicitorsSp.SolicitorID = solicitorId;
            var result = selectSolicitorsSp.Execute();
            return !result.HasNoDataRows
                ? result.ResultToArray<SolicitorDto>()
                : ArrayUtils.Empty<SolicitorDto>();
        }

    }
}