using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exceptionless;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQHelper.Commom;
using RabbitMQHelper.FilterExtension;
using RabbitMQHelper.LogExtension;
using RabbitMQHelper.Middleware;

namespace RabbitMQHelper
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMvc(cfg =>
            {
                cfg.Filters.Add(typeof(ActionFilterExtension));
            });
            services.AddSingleton<IExceptionLessLogger, ExceptionLessLogger>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            ExceptionlessClient.Default.Configuration.ApiKey = ConfigurationUtil.ExceptionlessApiKey;
            ExceptionlessClient.Default.Configuration.ServerUrl = ConfigurationUtil.ExceptionlessServerUrl;
            //ExceptionlessClient.Default.SubmittingEvent += OnSubmittingEvent;
            app.UseExceptionless();
            app.UseMiddleware<CustomExceptionHandlerMiddleware>();//全局异常
            app.UseMiddleware<DBMiddleware>();//数据库
            app.UseMiddleware<RabbitMqMiddleware>();//MQ
            app.UseMvc();
        }
    }
}
