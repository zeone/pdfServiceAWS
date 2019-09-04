using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PDFServiceAWS.Services
{
    public interface IExecutionService
    {
        void AddTask(string qRepName, object filter, Func<object,byte[]> executor);

    }
}