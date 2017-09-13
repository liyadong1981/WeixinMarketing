/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：APP_RedPackage_Activity_LogService.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using Senparc.Core.Models;
using Senparc.Log;
using Senparc.Repository;
using StructureMap;

namespace Senparc.Service
{
    public interface IAPP_RedPackage_Activity_LogService : IBaseClientService<APP_RedPackage_Activity_Award_Log>
    {
    }

    public class APP_RedPackage_Activity_LogService : BaseClientService<APP_RedPackage_Activity_Award_Log>,
        IAPP_RedPackage_Activity_LogService
    {
        public APP_RedPackage_Activity_LogService(
            IAPP_RedPackage_Activity_LogRepository appRedPackageActivityAwardLogRepository)
            : base(appRedPackageActivityAwardLogRepository)
        {
        }

        /// <summary>
        /// 创建中奖纪录
        /// </summary>
        /// <param name="actionId"></param>
        /// <param name="accountId"></param>
        /// <param name="money"></param>
        /// <param name="awardName"></param>
        /// <param name="registerInfo">中奖填入的信息</param>
        /// <returns></returns>
        public APP_RedPackage_Activity_Award_Log CreateAppRedPackageActivityAwardLog(int actionId, int accountId,
            decimal money, string awardName, string registerInfo)
        {
            var accountService = ObjectFactory.GetInstance<AccountService>();
            var appRedPackageActivityService = ObjectFactory.GetInstance<APP_RedPackage_ActivityService>();
            var appRedPackageActivity = appRedPackageActivityService.GetObject(z => z.Id == actionId);
            if (appRedPackageActivity == null)
            {
                throw new Exception("活动不存在！");
            }
            var account = accountService.GetObject(z => z.Id == accountId);
            if (account == null)
            {
                throw new Exception("用户不存在！");
            }
            var appRedPackageActivityLog = GetObject(z => z.AccountId == accountId && z.ActivityId == actionId);
            if (appRedPackageActivityLog != null)
            {
                throw new Exception("已经参加过该活动！！");
            }
            var log = new APP_RedPackage_Activity_Award_Log()
            {
                AccountId = accountId,
                ActivityId = actionId,
                AwardName = awardName,
                Money = money,
                State = 0,
                AddTime = DateTime.Now,
                RegisterInfo = registerInfo,
            };
            SaveObject(log);
            return log;
        }


        public override void SaveObject(APP_RedPackage_Activity_Award_Log obj)
        {
            var isInsert = base.IsInsert(obj);
            base.SaveObject(obj);
            LogUtility.WebLogger.InfoFormat("APP_RedPackage_Activity_Award_Log{2}：{0}（ID：{1}）", obj.Id, obj.Id, isInsert ? "新增" : "编辑");
        }

        public override void DeleteObject(APP_RedPackage_Activity_Award_Log obj)
        {
            LogUtility.WebLogger.InfoFormat("APP_RedPackage_Activity_Award_Log被删除：{0}（ID：{1}）", obj.Id, obj.Id);
            base.DeleteObject(obj);
        }
    }
}