using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PublicDAL.Dapper;
using RabbitMQ.Client;
using RabbitMQHelper.Commom;
using RabbitMQHelper.Model;
using RabbitMQHelper.Model.BusinessModel;
using RabbitMQHelper.Model.Entity;
using RabbitMQHelper.Model.Response;

namespace RabbitMQHelper.CoreBusiness
{
    public class FaceRecognitionRecordHandle
    {
        private static ILogger _logger;

        public FaceRecognitionRecordHandle(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger<FaceRecognitionRecordHandle>();
        }

        /// <summary>
        /// 添加到CRM表记录
        /// </summary>
        /// <param name="value"></param>
        /// <param name="MId"></param>
        public void Add(string value, Guid MId)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    var mesInfo = JsonConvert.DeserializeObject<ReadSenceMQMessage>(value);
                    if (mesInfo != null && mesInfo.data != null)
                    {
                        if (!string.IsNullOrWhiteSpace(mesInfo.data.person_uuid))
                        {
                            var customer = CustomerHandle.GetCustomerInfo(mesInfo.data.person_uuid);

                            var model = new FaceRecognitionRecord
                            {
                                Id = Guid.NewGuid(),
                                AddedOn = DateTime.Now,
                                CustomerId = customer.CustomerID,
                                DeviceName = mesInfo.data.device_name,
                                RegionName = mesInfo.data.device_location,
                                FaceId = mesInfo.data.person_uuid
                            };
                            DbContext.Add(model);

                            var MesInfo = new CRMMessageInfo
                            {
                                CustomerId = customer.CustomerID,
                                CustomerName = customer.FullName,
                                RegionName = mesInfo.data.device_location,
                                CreatedOn = DateTime.Now,
                            };

                            CRMProductor(JsonConvert.SerializeObject(MesInfo), MId);
                        }

                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"【添加到CRM表记录异常】{ex.Message}");
                }

            }
        }


        /// <summary>
        /// 推送到CRM前端
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="MId"></param>
        private static void CRMProductor(string Value, Guid MId)
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

                _logger.LogInformation($"【Title : CRM-MQ发送消息 】  【Mid : {MId}】  【Body : {Value}】");
            }
        }

    }
}
