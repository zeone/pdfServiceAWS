using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Ninject;
using Ninject.Parameters;
using PDFServiceAWS.Services;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Helpers;

namespace PDFServiceAWS.Controllers
{
    [Route("GeneratePDF/[action]")]
    public class GeneratePDFController : Controller
    {
        private IReportService _reportService;
        private readonly IExecutionService _executionService;

        public GeneratePDFController(IExecutionService executionService)
        {
            _executionService = executionService;
        }

        #region Filter and create Pdf report file
        [HttpPost]
        public IActionResult GetContactPdf([FromBody]BaseFilterRequest request)
        {
            try
            {
                _reportService = NinjectBulder.Container.Get<IReportService>(new ConstructorArgument("schema", request.Schema));

                _executionService.AddTask($"{request.PdfReportId}_{request.Schema}",
                    (object)request.ReportDto, _reportService.GetContactPdf);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }


        [HttpPost]
        public IActionResult GetTransactionPdf([FromBody]BaseFilterRequest request)
        {
            try
            {
                _reportService = NinjectBulder.Container.Get<IReportService>(new ConstructorArgument("schema", request.Schema));
                request.ReportDto = new ReportDto();
                request.ReportDto.Country = request.CountryName;
                _executionService.AddTask($"{request.PdfReportId}_{request.Schema}",
                    (object)request.Filter, _reportService.GetTransactionPdf);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        #endregion

        #region Create only Pdf report file

        [HttpPost]
        public IActionResult CreateContactReportPDf([FromBody]ContactReportPdfOnlyRequest request)
        {
            try
            {
                var contactPdfService = NinjectBulder.Container.Get<IContactReportPdfService>(new ConstructorArgument("schema", request.Schema));
   
                var pdfDoc = new PdfDocumentDto
                {
                    ReportDto = request.ReportDto,
                    Contacts = request.Contacts
                };
                _executionService.AddTask($"{request.PdfReportId}_{request.Schema}",
                    (object)pdfDoc, contactPdfService.CreateDocument);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }


        [HttpPost]
        public IActionResult CreateTransactionReportPDf([FromBody]TransactionReportPdfOnlyRequest request)
        {
            try
            {
                var transPdfService = NinjectBulder.Container.Get<ITransactionReportPdfService>(new ConstructorArgument("schema", request.Schema));
                transPdfService.InitializeCollections(Startup.GetTranslation, request.PaymentMethods, request.Solicitors, request.Mailings, request.Departments, request.CategoryTree);
           
                var pdfDoc = new PdfDocumentDto
                {
                    Filter = request.Filter,
                    Grouped = request.Grouped,
                    CountTrans = request.TransactionCount
                };
                _executionService.AddTask($"{request.PdfReportId}_{request.Schema}",
                    (object)pdfDoc, transPdfService.CreateDocument);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            return Ok();
        }

        #endregion

    }
}