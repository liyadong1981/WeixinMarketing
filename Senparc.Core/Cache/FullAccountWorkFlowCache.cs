/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：FullAccountWorkFlowCache.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Senparc.Core.Cache.BaseCache.Redis;
using Senparc.Core.Models;
using Senparc.Core.Models.WorkFlowModules;
using ServiceStack;
using StructureMap;

namespace Senparc.Core.Cache
{
    using Senparc.Core.Enums;
    using Senparc.Core.Models;
    using Senparc.Core.Utility;
    using Senparc.Core.Extensions;

    public interface IFullAccountWorkFlowCache : IBaseStringDictionaryCache<FullAccountWorkFlow>
    {

    }
    public class FullAccountWorkFlowCache : BaseStringDictionaryCache<FullAccountWorkFlow>, IFullAccountWorkFlowCache
    {
        public const string CACHE_KEY = "FullAccountWorkFlowCache";
        private const int timeout = 1440;

        public FullAccountWorkFlowCache(ISqlClientFinanceData db) : base(CACHE_KEY, db, timeout)
        {
        }

        public override FullAccountWorkFlow InsertObjectToCache(string key)
        {
            //int activityId;
            //int accountId;
            //FullAccountWorkFlow.GetKeyValues(key, out activityId, out accountId);
            //var activity = base._db.DataContext.APP_RedPackage_Activity.FirstOrDefault(z => z.Id == activityId);
            //if (activity == null)
            //{
            //    return null;
            //}
            //var fullAccuntWorkFlow = CreateFullAccountWorkFlow(activity, accountId);
            //return fullAccuntWorkFlow;
            return null;
        }
        /// <summary>
        /// 创建用户工作流
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="accountId"></param>
        public FullAccountWorkFlow CreateFullAccountWorkFlow(APP_RedPackage_Activity activity, int accountId)
        {
            var moduleList = activity.Rule.FromJson<List<BaseModule>>();
            var fullWorkFlowModule = new FullAccountWorkFlow
            {
                AccountId = accountId,
                ActivityId = activity.Id,
                BaseModuleList = moduleList,
                Input = new Input()
            };
            var key = FullAccountWorkFlow.GetKey(activity.Id, accountId);
            base.InsertObjectToCache(key, fullWorkFlowModule);
            return fullWorkFlowModule;
        }

        public FullAccountWorkFlow GetFullAccountWorkFlow(int activityId, int accountId)
        {
            var key = FullAccountWorkFlow.GetKey(activityId, accountId);
            return base.GetObject(key);
        }
        /// <summary>
        /// 更新缓存
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public FullAccountWorkFlow UpdateFullAccountWorkFlowToCache(FullAccountWorkFlow obj)
        {
            var key = FullAccountWorkFlow.GetKey(obj.ActivityId, obj.AccountId);
            base.UpdateToCache(key, obj);
            return obj;
        }
        /// <summary>
        /// 删除制定活动下的所有已经创建的用户工作流信息
        /// </summary>
        /// <param name="activityId">活动Id</param>
        public void RemoveActivityAllFullAccountWorkFlow(int activityId)
        {
            var fullAccountWorkFlowList = base.GetObjectList(activityId.ToString());
            foreach (var fullAccountWorkFlow in fullAccountWorkFlowList)
            {
                var finalCacheKey = FullAccountWorkFlow.GetKey(fullAccountWorkFlow.ActivityId, fullAccountWorkFlow.AccountId);
                base.RemoveObject(finalCacheKey);
            }
        }
    }
}
