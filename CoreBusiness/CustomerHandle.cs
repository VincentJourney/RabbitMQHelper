using PublicDAL.Dapper;
using RabbitMQHelper.Model.BusinessModel;
using RabbitMQHelper.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQHelper.CoreBusiness
{

    public class CustomerHandle
    {
        /// <summary>
        /// 根据人脸Id查询会员信息
        /// </summary>
        /// <param name="FaceId"></param>
        /// <returns></returns>
        public static Customer GetCustomerInfo(string FaceId)
        {
            var sql = $@"SELECT c.* FROM Customer AS c
            LEFT JOIN  CustomerContacts AS cc ON c.CustomerID=cc.CustomerID
            WHERE cc.ContactType=11 AND cc.ContactNo='{FaceId}'";
            return DbContext.Query<Customer>(sql).FirstOrDefault();
        }

        /// <summary>
        /// 获取企业微信发送消息相关信息
        /// </summary>
        /// <returns></returns>
        public static CRMWeChatInfo GetCRMWeChatInfo()
        {
            var sql = $@"select cai.AppID,cai.AppSecret,cai.WeChatPublicNo,crs.ReminderContent,cu.UserId
            FROM CSReminderSetting AS crs
            LEFT JOIN CorpAccountInfo AS cai ON crs.CompanyID=cai.CompanyID 
            AND cai.OrgID=crs.OrgID AND cai.TokenValidto > GETDATE()
            LEFT JOIN CorpUser AS cu ON crs.KFAccount=cu.CorpUserId AND cu.[Enabled]=1
            WHERE crs.[Status]=1 
            AND crs.ScenarioCode='06' 
            AND crs.ScenariosubCode='0601'";
            return DbContext.Query<CRMWeChatInfo>(sql).FirstOrDefault();
        }
    }
}
