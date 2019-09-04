using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using PDFServiceAWS.Enums;
using PDFServiceAWS.DB.SP;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Helpers;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public class MailingService : BaseService, IMailingService
    {
        private string _schema;

        public MailingService(IQueryProvider queryProvider, string schema) : base(queryProvider)
        {
            _schema = schema;
        }

        public IEnumerable<MailingDto> GetAllMailings()
        {
            return GetMailings(null);
        }
        
        private IEnumerable<MailingDto> GetMailings(int? mailigID)
        {
            var selectMailingsSp = QueryProvider.CreateQuery<SelectMailingsStoredProcedure>(_schema);
            selectMailingsSp.MailingID = mailigID;
            var res = selectMailingsSp.Execute();
            return !res.HasNoDataRows ? res.ResultToArray<MailingDto>() : ArrayUtils.Empty<MailingDto>();
        }
        
    }
}