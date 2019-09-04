using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Ninject;
using Ninject.Parameters;
using PDFServiceAWS.DB.SP;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Helpers;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly ILetterService _letterService;
        private string _schema;
        public DepartmentService(IQueryProvider queryProvider, string schema) : base(queryProvider)
        {
            _letterService = NinjectBulder.Container.Get<ILetterService>(new ConstructorArgument("schema",schema));
            _schema = schema;
        }

        public IEnumerable<DepartmentDto> GetAllDepartments()
        {
            return GetDepartments(null);
        }

        public DepartmentDto GetDepartment(int departmentId)
        {
            return GetDepartments(departmentId).FirstOrDefault();
        }


        private IEnumerable<DepartmentDto> GetDepartments(int? departmentId = null, bool? isprimary = null)
        {
            var selectDepartmentSp = QueryProvider.CreateQuery<SelectDepartmentsStoredProcedure>(_schema);
            selectDepartmentSp.DepartmentId = departmentId;
            selectDepartmentSp.IsPrimary = isprimary;
            var result = selectDepartmentSp.Execute();
            if (result.HasNoDataRows) return ArrayUtils.Empty<DepartmentDto>();
            var resp = result.ResultToArray<DepartmentDto>();
            foreach (DepartmentDto departmentDto in resp)
            {
                departmentDto.PdfSettings = _letterService.GetPdfSetting(departmentDto.PDFSettingID);
            }
            return resp;
        }

    }
}