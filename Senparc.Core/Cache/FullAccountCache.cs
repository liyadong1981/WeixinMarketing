/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：FullAccountCache.cs

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
using StructureMap;

namespace Senparc.Core.Cache
{
    using Senparc.Core.Enums;
    using Senparc.Core.Models;
    using Senparc.Core.Utility;
    using Senparc.Core.Extensions;

    public interface IFullAccountCache : IBaseStringDictionaryCache<FullAccount, Account>
    {
        /// <summary>
        /// 根据AccountId查找
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        FullAccount GetFullAccount(int accountId);

        string GetUserName(int accountId);

        /// <summary>
        /// 根据OpenId查找
        /// </summary>
        /// <param name="weixinOpenId"></param>
        /// <returns></returns>
        FullAccount GetObjectByOpenId(string weixinOpenId);
    }

    public class FullAccountCache : BaseStringDictionaryCache<FullAccount, Account>, IFullAccountCache
    {
        public const string CACHE_KEY = "FullAccountCache";
        private const int timeout = 1440;

        /// <summary>
        /// Account的WeixinOpen和UserName的映射关系
        /// </summary>
        public static Dictionary<string, string> AccountOpenIdNameRelationshop = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// AccountId和UserName的映射关系
        /// </summary>
        public static Dictionary<int, string> AccountIdNameRelationshop = new Dictionary<int, string>();

        public FullAccountCache(ISqlClientFinanceData db)
            : base(CACHE_KEY, db, timeout)
        {
        }

        /// <summary>
        /// 根据判断条件获取Account
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        private Account GetAccount(Func<Account, bool> where)
        {
            var account = (base._db as ISqlClientFinanceData).DataContext.Accounts
                                            //.Include("Developer")
                                            //.Include("AgentAreas")
                //.Include("UserRoles")
                //.Include("UserRoles.UserRoleModule")
                //.Include("Station")
                //.Include("MainAccounts")
                //.Include("SubAccounts")
                //.Include("Station.Department")
                //.Include("Station.UserRoleModule")
                                            .FirstOrDefault(where);

            return account;
        }

        public override FullAccount InsertObjectToCache(string key)
        {
            var account = this.GetAccount(z => z.UserName.Equals(key, StringComparison.OrdinalIgnoreCase));
            var fullAccount = this.InsertObjectToCache(key, account);
            return fullAccount;
        }

        public override FullAccount InsertObjectToCache(string key, Account obj)
        {
            var fullAccount = base.InsertObjectToCache(key, obj);
            if (fullAccount == null)
            {
                return null;
            }
            else
            {
                AccountIdNameRelationshop[fullAccount.Id] = fullAccount.UserName;

                if (!fullAccount.WeixinOpenId.IsNullOrEmpty())
                {
                    AccountOpenIdNameRelationshop[fullAccount.WeixinOpenId] = fullAccount.UserName;
                }
            }

            return fullAccount;
        }

        public string GetUserName(int accountId)
        {
            var userName = AccountIdNameRelationshop.ContainsKey(accountId)
                            ? AccountIdNameRelationshop[accountId]
                            : null;
            return userName;
        }

        public FullAccount GetObjectByOpenId(string weixinOpenId)
        {
            if (weixinOpenId.IsNullOrEmpty())
            {
                return null;
            }

            var userName = AccountOpenIdNameRelationshop.ContainsKey(weixinOpenId)
                ? AccountOpenIdNameRelationshop[weixinOpenId]
                : null;
            if (userName == null)
            {
                //未命中，查找数据库
                var account = this.GetAccount(z => z.WeixinOpenId == weixinOpenId);
                if (account == null)
                {
                    return null;
                }
                var fullAccount = this.InsertObjectToCache(account.UserName, account);
                return fullAccount;
            }
            else
            {
                return GetObject(userName);
            }
        }

        public FullAccount GetFullAccount(int accountId)
        {
            var userName = GetUserName(accountId);

            if (userName == null)
            {
                //未命中，查找数据库
                var account = this.GetAccount(z => z.Id == accountId);
                if (account == null)
                {
                    return null;
                }
                var fullAccount = this.InsertObjectToCache(account.UserName, account);
                return fullAccount;
            }
            else
            {
                return GetObject(userName);
            }
        }
    }
}
