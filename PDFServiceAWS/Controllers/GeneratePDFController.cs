using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PDFServiceAWS.Services;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.Controllers
{
    [Route("GeneratePDF/[action]")]
    public class GeneratePDFController : Controller
    {
       private readonly IReportService _reportService;
       private readonly IExecutionService _executionService ;

       public GeneratePDFController(IReportService reportService, IExecutionService executionService)
       {
           _reportService = reportService;
           _executionService = executionService;
       }

       [HttpPost]
        public IActionResult GetContactPdf(ContactReportFilterRequest request)
        {
            return Ok();
        }
    }
}