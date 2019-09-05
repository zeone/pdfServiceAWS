using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PDFServiceAWS.DB.SP;
using PDFServiceAWS.Dto;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public sealed class AddressService : BaseService, IAddressService
    {
        private string _schema;
        public AddressService(
            IQueryProvider queryProvider,string schema) : base(queryProvider)
        {
            _schema = schema;
        }

       public List<FamilyAddressDto> GetAddressesByFamilyId(int famId)
        {
            var query = QueryProvider.CreateQuery<SelectAddressStoredProcedure>(_schema);
            query.FamilyId = famId;
            var result = query.Execute();
            return result.DataRows != null ? result.ResultToArray<FamilyAddressDto>().ToList() : null;
        }
    }
}