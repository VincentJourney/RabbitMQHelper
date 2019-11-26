using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PublicDAL.Dapper;
using RabbitMQHelper.Commom;

namespace RabbitMQHelper.Middleware
{
    public class DBMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger _log;

        public DBMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _log = loggerFactory.CreateLogger(typeof(RabbitMqMiddleware));
        }

        public async Task Invoke(HttpContext contexts)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ConfigurationUtil.ConnectionStringSqlServer))
                    throw new Exception("数据库连接字符串为空");
                
                //创建单一实例
                DapperHelper.GetInstance(ConfigurationUtil.ConnectionStringSqlServer);
                await _next(contexts);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
