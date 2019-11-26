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

namespace RabbitMQHelper.Middleware
{
    public class RabbitMqMiddleware
    {
        private readonly RequestDelegate _next;
        private IExceptionLessLogger _logger { get; }

        public RabbitMqMiddleware(RequestDelegate next, IExceptionLessLogger exceptionLessLogger)
        {
            _next = next;
            _logger = exceptionLessLogger;
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
                using (conn = factory.CreateConnection())
                using (channel = conn.CreateModel())
                {
                    string queueName = appKey;
                    channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic, durable: true);
                    channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: appKey, arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        var MId = Guid.NewGuid();
                        _logger.Info($"【Title : 阅面MQ接收消息】  【Mid : {MId}】  【Body : {message}】");
                        CRM(message, MId);
                        channel.BasicAck(ea.DeliveryTag, false);
                    };
                    channel.BasicConsume(queueName, false, consumer);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($" ReadSenseConsumer 异常 ===》{ex.Message}");
            }
        }

        /// <summary>
        /// 接收阅面推送
        /// </summary>
        private void ReadSenseTopicConsumer2()
        {
            try
            {
                var appKey = ConfigurationUtil.ReadSenceAppKey;  //猎户账号的AppKey
                var QueueName = ConfigurationUtil.ReadSenceMQQueueName;
                var ExchangeName = ConfigurationUtil.ReadSenceMQExchangeName;

                var factory = new ConnectionFactory()
                {
                    HostName = ConfigurationUtil.ReadSenceMQHostName,
                    Port = ConfigurationUtil.ReadSenceMQPort,
                    UserName = ConfigurationUtil.ReadSenceMQUserName,
                    Password = ConfigurationUtil.ReadSenceMQPassWord
                };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.ExchangeDeclare(exchange: ExchangeName, type: "topic", durable: true);
                    channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
                    channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: appKey);
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        try
                        {
                            var MId = Guid.NewGuid();
                            _logger.Info($"【Title : 阅面MQ接收消息】  【Mid : {MId}】  【Body : {message}】");
                            CRM(message, MId);

                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($" 阅面MQ接收消息 异常 ===》{ex.Message} ");
                        }
                    };
                    channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
                    //channel.BasicConsume(queue: QueueName, true, consumer: consumer);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($" ReadSenseConsumer 异常 ===》{ex.Message}");
            }
        }

        /// <summary>
        /// 添加到CRM表记录
        /// </summary>
        /// <param name="value"></param>
        /// <param name="MId"></param>
        private void CRM(string value, Guid MId)
        {
            var model = new FaceRecognitionRecord
            {
                Id = Guid.NewGuid(),
                AddedOn = DateTime.Now,
                CustomerId = Guid.NewGuid(),
                DeviceName = "",
                RegionName = "",
                FaceId = ""
            };
            DbContext.Add(model);
            CRMProductor(value, MId);
        }

        /// <summary>
        /// 推送到CRM前端
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MId"></param>
        private void CRMProductor(string Value, Guid MId)
        {
            var ExchangeName = ConfigurationUtil.CRMMQExchangeName;
            var QueueName = ConfigurationUtil.CRMMQQueueName;
            var RoutingKey = ConfigurationUtil.CRMMQRoutingKey;

            var factory = new ConnectionFactory()
            {
                HostName = ConfigurationUtil.CRMMQHostName,
                Port = ConfigurationUtil.CRMMQPort,
                UserName = ConfigurationUtil.CRMMQUserName,
                Password = ConfigurationUtil.CRMMQPassWord,
            };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: ExchangeName, type: ExchangeType.Fanout);
                channel.QueueDeclare(queue: QueueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                channel.QueueBind(queue: QueueName, exchange: ExchangeName, routingKey: RoutingKey);
                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                var body = Encoding.UTF8.GetBytes(Value);
                channel.BasicPublish(exchange: ExchangeName,
                                     routingKey: RoutingKey,
                                     basicProperties: properties,
                                     body: body);

                _logger.Info($"【Title : CRM-MQ接收消息 】  【Mid : {MId}】  【Body : {Value}】");
            }
        }
    }
}
