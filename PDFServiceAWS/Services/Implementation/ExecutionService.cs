using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Ninject;
using Ninject.Parameters;
using PDFServiceAWS.Dto;

namespace PDFServiceAWS.Services.Implementation
{
    public class ExecutionService : IExecutionService
    {
        static IReportService _service;

        ConcurrentDictionary<string, QueueObject> queueDict = new ConcurrentDictionary<string, QueueObject>();
        ConcurrentDictionary<string, ServiceResponse> _sendingDict = new ConcurrentDictionary<string, ServiceResponse>();
        private bool isWorking = false;
        private bool isSending = false;
        private object lockObj = new object();
        public void AddTask(string qRepName, object filter, Func<object, byte[]> executor)
        {
            if (queueDict.ContainsKey(qRepName)) return;
            queueDict.TryAdd(qRepName, new QueueObject(filter, executor));
            if (!isSending && _sendingDict.Any())
                SendReport();
            if (!isWorking && queueDict.Any())
            {
                var qObj = queueDict.First();
                GenerateReport(qRepName, qObj.Value);
            }


        }

        private void GenerateReport(string qRepName, QueueObject obj)
        {
            isWorking = true;
            var repInfo = qRepName.Split('_');
            var resp = new ServiceResponse
            {
                ReportQId = int.Parse(repInfo[0]),
                Schema = repInfo[1]
            };
            try
            {
                switch (obj.Executor.Method.Name)
                {
                    case "GetContactPdf":
                        resp.PdfByteArr = obj.Executor.Invoke((ContactReportFilterRequest)obj.FilterDataObj);
                        break;
                    case "GetTransactionPdf":
                        resp.PdfByteArr = obj.Executor.Invoke((TransactionReportFilterRequest)obj.FilterDataObj);
                        break;
                    case "CreateContactReportPDf":
                        resp.PdfByteArr = obj.Executor.Invoke((ContactReportPdfOnlyRequest)obj.FilterDataObj);
                        break;
                    case "CreateTransactionReportPDf":
                        resp.PdfByteArr = obj.Executor.Invoke((TransactionReportPdfOnlyRequest)obj.FilterDataObj);
                        break;
                    default:
                        throw new MissingMethodException("can't get correct method for delegate");
                }

            }
            catch (Exception e)
            {
                resp.IsSuccess = false;
                resp.ErrorMessage = e.Message;
                resp.StackTrace = e.StackTrace;
                isWorking = false;
            }

            AddToSending(resp);
            isWorking = false;

            if (!isSending && _sendingDict.Any())
                SendReport();
        }

        private void AddToSending(ServiceResponse resp)
        {
            _sendingDict.TryAdd($"{resp.ReportQId}_{resp.Schema}", resp);
        }

        void SendReport()
        {
            lock (lockObj)
            {
                isSending = true;
            }
            if (!_sendingDict.Any()) return;
            foreach (var response in _sendingDict)
            {
                ServiceResponse tsResp;
                Task<bool> ts = new Task<bool>(Sender);
                ts.Wait();
                if (ts.Result)
                    _sendingDict.TryRemove(response.Key, out tsResp);
            }
            lock (lockObj)
            {
                isSending = false;
            }
        }

        private bool Sender()
        {
            throw new NotImplementedException();
        }

        //byte[] GetContactPdf(ContactReportFilterRequest request)
        //{
        //    _service = NinjectBulder.Container.Get<IReportService>(new ConstructorArgument("schema", request.Schema));

        //    return _service.get(request.ReportDto, TranslateHelper.GetTranslation, request.CountryName);
        //}

        //byte[] GetTransactionPdf(TransactionReportFilterRequest request)
        //{
        //    var contactPdfService = NinjectBulder.Container.Get<IReportService>(new ConstructorArgument("schema", request.Schema));

        //    return contactPdfService.GetPdf(request.Filter, TranslateHelper.GetTranslation);

        //}

        //byte[] CreateContactReportPDf(ContactReportPdfOnlyRequest request)
        //{
        //    var contactPdfService = NinjectBulder.Container.Get<IContactReportPdfService>(new ConstructorArgument("schema", request.Schema));

        //    return contactPdfService.CreateDocument(request.ReportDto, request.Contacts, request.CountryName,
        //        TranslateHelper.GetTranslation);
        //}

        //byte[] CreateTransactionReportPDf(TransactionReportPdfOnlyRequest request)
        //{
        //    var transPdfService = NinjectBulder.Container.Get<ITransactionReportPdfService>(new ConstructorArgument("schema", request.Schema));
        //    transPdfService.InitializeCollections(TranslateHelper.GetTranslation, request.PaymentMethods, request.Solicitors, request.Mailings, request.Departments, request.CategoryTree);

        //    return transPdfService.CreateDocument(request.Filter, request.Grouped, request.TransactionCount);
        //}
    }
}