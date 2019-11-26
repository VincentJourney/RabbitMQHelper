using Newtonsoft.Json;
using PublicDAL.Dapper;
using RabbitMQHelper.Commom;
using RabbitMQHelper.Model.BusinessModel;
using RabbitMQHelper.Model.Entity;
using RabbitMQHelper.Model.Request;
using RabbitMQHelper.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQHelper.CoreBusiness
{
    public class EnterpriseWeChatUtil
    {
        private string corpid;
        private string corpsecret;
        private string token;
        public EnterpriseWeChatUtil(string CorpId, string CorpSecret)
        {
            corpid = CorpId;
            corpsecret = CorpSecret;
            token = CacheUtil.Get<string>("WeChat_Token");
            if (string.IsNullOrWhiteSpace(token))
            {
                var result = GetToken(corpid, corpsecret);
                if (result == null) throw new Exception("请求企业微信token失败");
                if (result.errcode != 0) throw new Exception(result.errmsg);
                if (string.IsNullOrWhiteSpace(result.access_token)) throw new Exception("请求企业微信token失败");
                token = CacheUtil.GetValueByCache<string>("WeChat_Token", result.access_token, TimeSpan.FromSeconds(result.expires_in));
            }
        }
        /// <summary>
        /// 获取企业微信Token
        /// </summary>
        /// <param name="corpid"></param>
        /// <param name="corpsecret"></param>
        /// <returns></returns>
        public static GetTokenResponse GetToken(string corpid, string corpsecret)
        {
            var url = $" https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpid}&corpsecret={corpsecret}";
            return JsonConvert.DeserializeObject<GetTokenResponse>(HttpUtil.HttpGet(url));
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public SendMessageResponse Send(SendMessageRequest req)
        {
            var url = $"https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={token}";
            var result = HttpUtil.HttpPost(url, JsonConvert.SerializeObject(req));
            return JsonConvert.DeserializeObject<SendMessageResponse>(result);
        }

    }
}
