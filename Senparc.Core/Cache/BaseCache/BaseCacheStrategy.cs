using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Senparc.Core.Cache
{
    public class BaseCacheStrategy : IBaseCacheStrategy
    {
        /// <summary>
        /// 当前缓存
        /// </summary>
        private static volatile System.Web.Caching.Cache cache = HttpRuntime.Cache;

        /// <summary>
        /// 默认到期时间
        /// </summary>
        private static int _timeout = 60;//单位：分

        #region 单例

        /// <summary>
        /// SearchCache的构造函数
        /// </summary>
        BaseCacheStrategy()
        {
        }

        //静态SearchCache
        public static BaseCacheStrategy Instance
        {
            get
            {
                return Nested.instance;//返回Nested类中的静态成员instance
            }
        }

        class Nested
        {
            static Nested()
            {
            }
            //将instance设为一个初始化的BaseCacheStrategy新实例
            internal static readonly BaseCacheStrategy instance = new BaseCacheStrategy();
        }

        #endregion

        #region ICacheStrategy 成员

        public void InsertToCache(string key, object obj)
        {
            this.InsertToCache(key, obj);
        }

        public void InsertToCache(string key, object obj, int timeout)
        {
            if (string.IsNullOrEmpty(key) || obj == null)
            {
                return;
            }
            cache.Insert(key, obj, null, DateTime.Now.AddMinutes(timeout), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
        }

        public void InsertToCache(string key, object obj, int timeout, CacheDependency dependencies)
        {
            if (string.IsNullOrEmpty(key) || obj == null)
            {
                return;
            }
            cache.Insert(key, obj, dependencies, DateTime.Now.AddMinutes(timeout), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
        }

        public void RemoveFromCache(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            cache.Remove(key);
        }

        public object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return cache.Get(key);
        }

        #endregion
    }
}
