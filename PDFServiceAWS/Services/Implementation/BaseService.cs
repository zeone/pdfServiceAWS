﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IQueryProvider = PDFServiceAWS.DB.IQueryProvider;

namespace PDFServiceAWS.Services.Implementation
{
    public abstract class BaseService : IAppService
    {

        protected readonly IQueryProvider QueryProvider;

        protected BaseService(IQueryProvider queryProvider)
        {
            if (queryProvider == null)
                throw new ArgumentNullException("queryProvider", "Query provider is not defined!");

            QueryProvider = queryProvider;
        }



        /// <summary>
        /// CMS user that is currently executing the service code
        /// </summary>
        //protected CmsUser ExecutingUser
        //{
        //    get
        //    {
        //        return WorkingContext.CurrentUser;
        //    }
        //}
    }
}