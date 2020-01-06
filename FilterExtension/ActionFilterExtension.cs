using Exceptionless;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQHelper.Commom;
using RabbitMQHelper.LogExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQHelper.FilterExtension
{
    public class ActionFilterExtension : ActionFilterAttribute
    {
        private IExceptionLessLogger _logger { get; }
        private ILogger _Mlogger { get; }

        public ActionFilterExtension(IExceptionLessLogger exceptionLessLogger, ILoggerFactory loggerFactory)
        {
            _logger = exceptionLessLogger;
            _Mlogger = loggerFactory.CreateLogger(typeof(ActionFilterExtension));
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            var info = filterContext.HttpContext.GetRequestInfo(ExceptionlessClient.Default.Configuration);
            var postData = info?.PostData?.ToString();
            var data = postData.Replace("\n", "").Replace(" ", "");
            //var mes = $@"【本机IP : {info?.Host} 】| 【接口路由 : {info?.Path}】 | 【客户端IP地址 : {info?.ClientIpAddress}】 | 【入参 : {data}】 | 【出参 : {Result}】";
            //_logger.Info(JsonConvert.SerializeObject(info));
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            var info = filterContext.HttpContext.GetRequestInfo(ExceptionlessClient.Default.Configuration);
            var Result = JsonConvert.SerializeObject(filterContext.Result);
            var postData = info?.PostData?.ToString();
            var data = postData.Replace("\n", "").Replace(" ", "");
            var mes = $@"【本机IP : {info?.Host} 】| 【接口路由 : {info?.Path}】 | 【客户端IP地址 : {info?.ClientIpAddress}】 | 【入参 : {data}】 | 【出参 : {Result}】";
            if (filterContext.Exception == null)
            {
                if (ConfigurationUtil.ExceptionlessIsEnabled)
                    _logger.Info(JsonConvert.SerializeObject(mes));
                else
                    _Mlogger.LogInformation(mes);

            }
        }
    }
}
