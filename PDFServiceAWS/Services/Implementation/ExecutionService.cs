using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using PDFServiceAWS.Dto;
using PDFServiceAWS.Enums;

namespace PDFServiceAWS.Services.Implementation
{
    public class ExecutionService : IExecutionService
    {
        //    static IReportService _service;

        ConcurrentDictionary<string, QueueObject> queueDict = new ConcurrentDictionary<string, QueueObject>();
        ConcurrentDictionary<string, UploadPdfReportDto> _sendingDict = new ConcurrentDictionary<string, UploadPdfReportDto>();
        private bool isWorking = false;
        private bool isSending = false;
        private object lockObj = new object();
        private readonly string baseSenderAddress;

        public ExecutionService()
        {
            baseSenderAddress = Startup.Configuration["baseSenderAddress"];
        }
        public async Task<bool> AddTask(string qRepName, object filter, Func<object, byte[]> executor)
        {
            if (queueDict.ContainsKey(qRepName))
            {
                if (!isWorking)
                    await GenerateReports();
                return true;
            }
            queueDict.TryAdd(qRepName, new QueueObject(filter, executor));
            if (!isSending && _sendingDict.Any())
                await SendReport();
            if (!isWorking && queueDict.Any())
                await GenerateReports();
            return true;
        }

        async Task<bool> GenerateReports()
        {
            lock (lockObj)
            {
                isWorking = true;
            }

            foreach (KeyValuePair<string, QueueObject> pair in queueDict)
            {
                await GenerateReport(pair.Key, pair.Value);
            }

            lock (lockObj)
            {
                isWorking = false;
            }

            return true;
        }
        private async Task<bool> GenerateReport(string qRepName, QueueObject obj)
        {
            var reportData = (BaseFilterReportRequest)obj.FilterDataObj;
            var resp = new UploadPdfReportDto
            {
                PdfReportId = reportData.PdfReportId,
                Schema = reportData.Schema,
                Key = reportData.Key,
                Status = PDFReportStatus.Ready
            };
            try
            {
                PdfDocumentDto pdfDoc;
                BaseFilterReportRequest req;
                switch (obj.Executor.Method.Name)
                {
                    case "GetContactPdf":
                        req = (BaseFilterReportRequest)obj.FilterDataObj;
                        resp.PdfByteArr = obj.Executor.Invoke(req.ReportDto);
                        break;
                    case "GetTransactionPdf":
                        req = (BaseFilterReportRequest)obj.FilterDataObj;
                        resp.PdfByteArr = obj.Executor.Invoke(req.Filter);
                        break;
                    case "CreateContactReportPDf":
                        var reqContPdf = (ContactReportPdfOnlyRequest)obj.FilterDataObj;
                        pdfDoc = new PdfDocumentDto
                        {
                            ReportDto = reqContPdf.ReportDto,
                            Contacts = reqContPdf.Contacts
                        };
                        resp.PdfByteArr = obj.Executor.Invoke(pdfDoc);
                        break;
                    case "CreateTransactionReportPDf":
                        var reqTransPdf = (TransactionReportPdfOnlyRequest)obj.FilterDataObj;
                        pdfDoc = new PdfDocumentDto
                        {
                            Filter = reqTransPdf.Filter,
                            Grouped = reqTransPdf.Grouped,
                            CountTrans = reqTransPdf.TransactionCount
                        };
                        resp.PdfByteArr = obj.Executor.Invoke(pdfDoc);
                        break;
                    default:
                        throw new MissingMethodException("can't get correct method for delegate");
                }
            }
            catch (Exception e)
            {
                resp.Status = PDFReportStatus.GenerationError;
                resp.Message = e.Message;
                resp.StackTrace = e.StackTrace;
                isWorking = false;
            }

            AddToSending(resp);


            if (!isSending && _sendingDict.Any())
                await SendReport();
            return true;
        }

        private void AddToSending(UploadPdfReportDto resp)
        {
            _sendingDict.TryAdd($"{resp.PdfReportId}_{resp.Schema}", resp);
        }

        async Task<bool> SendReport()
        {
            lock (lockObj)
            {
                isSending = true;
            }
            if (!_sendingDict.Any()) return false;
            foreach (var response in _sendingDict)
            {
                UploadPdfReportDto tsResp;
                QueueObject dicResp;
                var result = await Sender(response.Value);
                if (result)
                {
                    _sendingDict.TryRemove(response.Key, out tsResp);
                    queueDict.TryRemove(response.Key, out dicResp);
                }
            }
            lock (lockObj)
            {
                isSending = false;
            }

            return true;
        }

        private async Task<bool> Sender(UploadPdfReportDto request)
        {
            HttpClient client = new HttpClient();
            var baseUrlStr = string.Format(baseSenderAddress, request.Schema);
            client.BaseAddress = new Uri(baseUrlStr);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var result = await client.PostAsJsonAsync("api/report/UploadPdfReport", request);
            return result.IsSuccessStatusCode;
        }

    }
}