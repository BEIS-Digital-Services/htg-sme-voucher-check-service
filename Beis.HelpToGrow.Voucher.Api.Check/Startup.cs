using Beis.Htg.VendorSme.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Beis.HelpToGrow.Voucher.Api.Check.Common;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using Beis.HelpToGrow.Voucher.Api.Check.Interfaces;
using Microsoft.Extensions.Logging;
using Beis.HelpToGrow.Voucher.Api.Check.Services.Repositories;
using Beis.HelpToGrow.Voucher.Api.Check.Services;

namespace Beis.HelpToGrow.Voucher.Api.Check
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<EncryptionSettings>(options =>
                Configuration.Bind(options));

            services.AddLogging(options =>
            {
                // hook the Console Log Provider
                options.AddConsole();
                options.SetMinimumLevel(LogLevel.Trace);

            });
            services.AddApplicationInsightsTelemetry(Configuration["AZURE_MONITOR_INSTRUMENTATION_KEY"]);

            services.AddControllers().AddNewtonsoftJson(options =>
            {
                var dateConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter
                {
                    DateTimeFormat = "dd'/'MM'/'yyyy' 'HH':'mm"
                };

                options.SerializerSettings.Converters.Add(dateConverter);
                options.SerializerSettings.Culture = new CultureInfo("en-GB");
                options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
            });
            services.AddSingleton<IEncryptionService, AesEncryption>();
            services.AddDbContext<HtgVendorSmeDbContext>(options => options.UseNpgsql(Configuration["HELPTOGROW_CONNECTIONSTRING"]), ServiceLifetime.Transient);

            services.AddTransient<IVoucherCheckService, VoucherCheckService>();
            services.AddTransient<IVendorAPICallStatusServices, VendorAPICallStatusServices>();

            services.AddTransient<ITokenRepository, TokenRepository>();
            services.AddTransient<IProductRepository, ProductRepository>();
            services.AddTransient<IVendorCompanyRepository, VendorCompanyRepository>();
            services.AddTransient<IVendorAPICallStatusRepository, VendorApiCallStatusRepository>();
            services.AddTransient<IEnterpriseRepository, EnterpriseRepository>();

            services.AddTransient<ITokenVoucherGeneratorService, TokenVoucherGeneratorService>();
            services.AddTransient<IVoucherGeneratorService, VoucherGenerationService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "beis-htg-sme-voucher-check-service", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "beis-htg-sme-voucher-chek-service v1"));

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
    
}
