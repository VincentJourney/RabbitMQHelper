using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using RabbitMQHelper.Commom;
using RabbitMQHelper.LogExtension;
using RabbitMQHelper.CoreBusiness;
using PublicDAL.Dapper;
using RabbitMQHelper.Model.Entity;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace RabbitMQHelper.Middleware
{
    public class RabbitMqMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger _logger { get; }
        private IExceptionLessLogger _exceptionLess { get; }

        private FaceRecognitionRecordHandle recognitionRecordHandle;

        public RabbitMqMiddleware(RequestDelegate next, ILoggerFactory logger, IExceptionLessLogger exceptionLessLogger)
        {
            _next = next;
            _logger = logger.CreateLogger<RabbitMqMiddleware>();
            recognitionRecordHandle = new FaceRecognitionRecordHandle(logger);
        }

        public async Task Invoke(HttpContext context)
        {
            ReadSenseTopicConsumer();
            await this._next.Invoke(context);
        }

        /// <summary>
        /// 接收阅面推送
        /// </summary>
        private void ReadSenseTopicConsumer()
        {
            IConnection conn = null;
            IModel channel = null;
            try
            {
                var appKey = ConfigurationUtil.ReadSenceAppKey;  //猎户账号的AppKey
                var QueueName = ConfigurationUtil.ReadSenceMQQueueName;
                var exchangeName = ConfigurationUtil.ReadSenceMQExchangeName;

                var factory = new ConnectionFactory()
                {
                    HostName = ConfigurationUtil.ReadSenceMQHostName,
                    Port = ConfigurationUtil.ReadSenceMQPort,
                    UserName = ConfigurationUtil.ReadSenceMQUserName,
                    Password = ConfigurationUtil.ReadSenceMQPassWord
                };

                //不阻塞主线程
                ThreadPool.QueueUserWorkItem(o =>
                {
                    using (conn = factory.CreateConnection())
                    using (channel = conn.CreateModel())
                    {
                        #region 事件
                        //string queueName = appKey;
                        //channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);
                        //channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                        //channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: appKey, arguments: null);

                        //var consumer = new EventingBasicConsumer(channel);
                        //consumer.Received += (model, ea) =>
                        //{
                        //    var body = ea.Body;
                        //    var message = Encoding.UTF8.GetString(body);
                        //    var MId = Guid.NewGuid();
                        //    _logger.Info($"【Title : 阅面MQ接收消息】  【Mid : {MId}】  【Body : {message}】");
                        //    CRM(message, MId);
                        //    channel.BasicAck(ea.DeliveryTag, false);
                        //};
                        //channel.BasicConsume(queueName, false, consumer);
                        #endregion
                        channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);
                        channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                        channel.QueueBind(queue: QueueName, exchange: exchangeName, routingKey: appKey, arguments: null);
                        var consumer = new QueueingBasicConsumer(channel);
                        channel.BasicConsume(QueueName, false, consumer);
                        while (true)
                        {
                            var ea = consumer.Queue.Dequeue();
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            var MId = Guid.NewGuid();
                            _logger.LogInformation($"【Title : 阅面MQ接收消息】  【Mid : {MId}】  【Body : {message}】");

                            recognitionRecordHandle.Add(message, MId);
                            channel.BasicAck(ea.DeliveryTag, false);
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                throw new Exception($" ReadSenseConsumer 异常 ===》{ex.Message}");
            }
        }



    }
}
