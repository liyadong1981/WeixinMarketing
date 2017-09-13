/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：FullAdminUserInfoCache.cs

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

    public interface IFullAdminUserInfoCache : IBaseStringDictionaryCache<FullAdminUserInfo, AdminUserInfo>
    {

    }

    public class FullAdminUserInfoCache : BaseStringDictionaryCache<FullAdminUserInfo, AdminUserInfo>, IFullAdminUserInfoCache
    {
        public const string CACHE_KEY = "FullAdminUserInfoCache";
        private const int timeout = 1440;

        public FullAdminUserInfoCache(ISqlClientFinanceData db)
            : base(CACHE_KEY, db, timeout)
        {
        }

        public override FullAdminUserInfo InsertObjectToCache(string key)
        {
            var adminUserInfo = (base._db as ISqlClientFinanceData).DataContext.AdminUserInfoes.FirstOrDefault(z => z.UserName==key);

            var fullAdminUserInfo = this.InsertObjectToCache(key, adminUserInfo);
            return fullAdminUserInfo;
        }


        public override FullAdminUserInfo InsertObjectToCache(string key, AdminUserInfo obj)
        {
            var fullAdminUserInfo = base.InsertObjectToCache(key, obj);
            if (fullAdminUserInfo == null)
            {
                return null;
            }
            return fullAdminUserInfo;
        }
    }
}
