/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：MethodCache.cs

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
using System.Net.Cache;
using System.Web;

namespace Senparc.Core.Cache
{
    public static class MethodCache
    {
        public static T GetMethodCache<T>(string cacheKey, Func<T> func, int timeoutSeconds) where T : class
        {
            cacheKey = cacheKey.ToUpper();
            System.Web.Caching.Cache cache = HttpRuntime.Cache;
            if (cache[cacheKey] == null)
            {
                //TODO:可以创建一个依赖项，全局实现更新
                cache.Insert(cacheKey, func(), null, DateTime.Now.AddSeconds(timeoutSeconds),System.Web.Caching.Cache.NoSlidingExpiration);
            }
            return cache[cacheKey] as T;
        }

        public static T GetMethodCache<T>(string cacheKey, Func<T> func) where T : class
        {
            return GetMethodCache(cacheKey, func, 60 * 60);
        }
    }
}
