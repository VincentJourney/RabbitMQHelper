using Exceptionless;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQHelper.Commom;
using RabbitMQHelper.Model.Response;
using System;
using System.Threading.Tasks;

namespace RabbitMQHelper.Middleware
{
    /// <summary>
    /// 全局
    /// </summary>
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger _log;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _log = loggerFactory.CreateLogger(typeof(RabbitMqMiddleware));
        }

        public async Task Invoke(HttpContext contexts)
        {
            try
            {
                await _next(contexts);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(contexts, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception e)
        {
            if (e == null) return;

            var info = context.GetRequestInfo(ExceptionlessClient.Default.Configuration);
            var Result = "";
            var postData = info?.PostData?.ToString();
            var data = postData.Replace("\n", "").Replace(" ", "");
            var mes = $@"【本机IP : {info?.Host} 】| 【接口路由 : {info?.Path}】 | 【客户端IP地址 : {info?.ClientIpAddress}】 | 【入参 : {data}】 | 【出参 : {Result}】";

            var exceptionMes = $"{mes} | 【异常信息 ： {e.Message}】 | 【堆栈信息 ： {e.StackTrace}】";

            if (ConfigurationUtil.ExceptionlessIsEnabled)
                e.ToExceptionless().AddObject(mes, "HttpContextInfo").Submit();
            else
                _log.LogError(exceptionMes);

            var res = new ResponseModel<string>
            {
                Data = "",
                Result = new Result
                {
                    HasError = true,
                    Message = e.Message
                }
            };
            context.Response.ContentType = "application/json;charset=utf-8";
            context.Response.WriteAsync(JsonConvert.SerializeObject(res));
        }
    }
}
