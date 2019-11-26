using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQHelper.Commom
{
    public static class ConfigurationUtil
    {
        static IConfigurationRoot configuration;
        static ConfigurationUtil()
        {
            configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = "appsettings.json" })
                .Build();
        }

        public static T GetConfig<T>(string name)
        {
            try
            {
                return configuration.GetValue<T>(name);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        #region 阅面
        public static string ReadSenceMQHostName => GetConfig<string>("RabbitMq:ReadSence:HostName");
        public static int ReadSenceMQPort => GetConfig<int>("RabbitMq:ReadSence:Port");
        public static string ReadSenceMQUserName => GetConfig<string>("RabbitMq:ReadSence:UserName");
        public static string ReadSenceMQPassWord => GetConfig<string>("RabbitMq:ReadSence:PassWord");
        public static string ReadSenceMQExchangeName => GetConfig<string>("RabbitMq:ReadSence:ExchangeName");
        public static string ReadSenceAppKey => GetConfig<string>("ReadSenceAppKey");
        public static string ReadSenceMQQueueName => GetConfig<string>("RabbitMq:ReadSence:QueueName");
        #endregion

        #region CRM
        public static string CRMMQHostName => GetConfig<string>("RabbitMq:CRM:HostName");
        public static int CRMMQPort => GetConfig<int>("RabbitMq:CRM:Port");
        public static string CRMMQUserName => GetConfig<string>("RabbitMq:CRM:UserName");
        public static string CRMMQPassWord => GetConfig<string>("RabbitMq:CRM:PassWord");
        public static string CRMMQExchangeName => GetConfig<string>("RabbitMq:CRM:ExchangeName");
        public static string CRMMQQueueName => GetConfig<string>("RabbitMq:CRM:QueueName");
        public static string CRMMQRoutingKey => GetConfig<string>("RabbitMq:CRM:RoutingKey");
        #endregion

        public static string ConnectionStringSqlServer => GetConfig<string>("ConnectionString:SqlServer");


        #region Exceptionless配置项
        public static bool ExceptionlessIsEnabled => GetConfig<bool>("Exceptionless:Enabled");
        public static string ExceptionlessApiKey => GetConfig<string>("Exceptionless:ApiKey");
        public static string ExceptionlessServerUrl => GetConfig<string>("Exceptionless:ServerUrl");
        #endregion



    }
}
