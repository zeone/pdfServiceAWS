using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using PDFServiceAWS.Resources;
using PDFServiceAWS.Services;
using PDFServiceAWS.Services.Implementation;

namespace PDFServiceAWS
{
    public class Startup
    {
        public readonly string ConnectionString;
        private static IStringLocalizer<Global> _localizer;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add S3 to the ASP.NET Core dependency injection framework.
            services.AddAWSService<Amazon.S3.IAmazonS3>();
            services.AddSingleton<IExecutionService, ExecutionService>();
            //add localization
            services.AddLocalization();
            services.Configure<RequestLocalizationOptions>(t =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en-US")
                };
                t.DefaultRequestCulture = new RequestCulture("en-US");
                // Formatting numbers, dates, etc.
                t.SupportedCultures = supportedCultures;
                // UI strings that we have localized.
                t.SupportedUICultures = supportedCultures;
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider srvProvider)
        {
            _localizer = srvProvider.GetRequiredService<IStringLocalizer<Global>>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }


        public static string GetTranslation(string translateFrom)
        {
            if (string.IsNullOrEmpty(translateFrom)) return string.Empty;
            var result = _localizer[translateFrom];
            return string.IsNullOrEmpty(result) ? translateFrom : result;
        }
    }
}
