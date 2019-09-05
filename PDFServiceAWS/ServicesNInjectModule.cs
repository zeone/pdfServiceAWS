using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ninject;
using Ninject.Modules;
using PDFServiceAWS.DB;
using PDFServiceAWS.DB.Injections;
using PDFServiceAWS.Services;
using PDFServiceAWS.Services.Implementation;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS
{
    public sealed class ServicesNInjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IReportService>().To<ReportService>();
            Bind<IAddressService>().To<AddressService>();
            Bind<IContactReportPdfService>().To<ContactReportPdfService>();
            Bind<IDepartmentService>().To<DepartmentService>();
            Bind<IGroupingService>().To<GroupingService>();
            Bind<IHebrewDateService>().To<HebrewDateService>();
            Bind<ILetterService>().To<LetterService>();
            Bind<IMailingService>().To<MailingService>();
            Bind<IPaymentMethodService>().To<PaymentMethodService>();
            Bind<IPdfServiceGenerator>().To<PdfServiceGenerator>();
            Bind<IPdfTemplateFunctionality>().To<PdfTemplateFunctionality>();
            Bind<IReportGroupingService>().To<ReportGroupingService>();
            Bind<ISolicitorService>().To<SolicitorService>();
            Bind<ITransactionReportPdfService>().To<TransactionReportPdfService>();
            Bind<ITransactionService>().To<TransactionService>();
            Bind<IXMLService>().To<XMLService>();
            Bind<IAppSettingsProvider>().To<AppSettingsProvider>();
        }
    }

    public static class NinjectBulder
    {
        private static StandardKernel _appInjectKernel;
        public static StandardKernel Container => _appInjectKernel ?? (_appInjectKernel = CreateInjectionContainer());


        private static StandardKernel CreateInjectionContainer()
        {
            var kernel = new StandardKernel(new DalNInjectModule(),
                new QueriesNInjectModule(),
                new ServicesNInjectModule()
            );

            // bind the injection kernel to self with a singleton value
            kernel.Bind<StandardKernel>()
                .ToConstant<StandardKernel>(kernel)
                .InSingletonScope();

            kernel.Bind<IKernel>()
                .ToConstant<StandardKernel>(kernel)
                .InSingletonScope();
            //kernel.Bind<IExecutionService>()
            //    .To<ExecutionService>()
            //    .InSingletonScope();
            QueryProvider appQueryProvider = new QueryProvider(kernel);
            kernel.Bind<IQueryProvider>()
                .ToConstant<QueryProvider>(appQueryProvider);
            // kernel.Bind<IReportService>().To<ReportService>();
            return kernel;
        }
    }
}
