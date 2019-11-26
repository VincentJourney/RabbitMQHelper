using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PublicDAL.Dapper;
using RabbitMQHelper.Commom;
using RabbitMQHelper.CoreBusiness;
using RabbitMQHelper.LogExtension;
using RabbitMQHelper.Model;
using RabbitMQHelper.Model.Entity;
using RabbitMQHelper.Model.Request;
using RabbitMQHelper.Model.Response;

namespace RabbitMQHelper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadSenseController : ControllerBase
    {
        private IExceptionLessLogger _logger { get; }
        public ReadSenseController(IExceptionLessLogger exceptionLessLogger)
        {
            _logger = exceptionLessLogger;
        }

        [HttpPost]
        [Route("Receive")]
        public IActionResult Receive([FromBody]ReadSenceEventRequest req)
        {
            if (req == null || req.error != null
                || req.Info == null || string.IsNullOrWhiteSpace(req.Info.person_id))
                throw new Exception("阅面消息推送：参数异常");

            ResponseModel<SendMessageResponse> res = new ResponseModel<SendMessageResponse>
            {
                Result = new Result
                {
                    HasError = true,
                    Message = ""
                }
            };

            var customer = CustomerHandle.GetCustomerInfo(req.Info.person_id); //获取会员信息
            if (customer == null) throw new Exception("找不到会员");

            var model = new FaceRecognitionRecord
            {
                Id = Guid.NewGuid(),
                AddedOn = DateTime.Now,
                CustomerId = customer.CustomerID,
                DeviceName = req.Info.device_name,
                RegionName = req.Info.region_name,
                FaceId = req.Info.person_id
            };
            DbContext.Add<FaceRecognitionRecord>(model);

            var CRMWeChatInfo = CustomerHandle.GetCRMWeChatInfo(); //获取企业微信推送配置
            if (!ReflectionUtil.ObjectIsNull(CRMWeChatInfo))
                throw new Exception("CRM企业微信推送配置异常");

            var EnterpriseWeChatUtil = new EnterpriseWeChatUtil(CRMWeChatInfo.AppID, CRMWeChatInfo.AppSecret);

            var MesContent = CommomUitl.ClearHTML(CRMWeChatInfo.ReminderContent);

            var mes = string.Format(MesContent, customer.FullName, req.Info.region_name, DateTime.Now);

            var sendReq = new SendMessageRequest
            {
                touser = CRMWeChatInfo.UserId,
                agentid = int.Parse(CRMWeChatInfo.WeChatPublicNo),
                text = new Content { content = mes }
            };

            var result = EnterpriseWeChatUtil.Send(sendReq);

            res.Result.HasError = false;
            res.Data = result;

            return Ok(res);
        }
    }
}
