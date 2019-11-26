using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQHelper.Model.BusinessModel
{
    public class CRMWeChatInfo
    {
        /// <summary>
        /// 企业微信 corpid
        /// </summary>
        public string AppID { get; set; }
        /// <summary>
        /// 企业微信 corpsecret
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 应用Id
        /// </summary>
        public string WeChatPublicNo { get; set; }
        /// <summary>
        /// 消息模板
        /// </summary>
        public string ReminderContent { get; set; }
        /// <summary>
        /// 企业微信 UserId
        /// </summary>
        public string UserId { get; set; }
    }
}
