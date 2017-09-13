/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：LocalCacheStrategy.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using Senparc.Core.Cache.Lock;

namespace Senparc.Core.Cache
{

    public interface ILocalCacheStrategy: IBaseCacheStrategy
    {
    }

    public partial class LocalCacheStrategy<T> : IBaseCacheStrategy<T>, ILocalCacheStrategy where T : class, new()
    {
        /// <summary>
        /// 当前缓存
        /// </summary>
        private static volatile System.Web.Caching.Cache cache = HttpRuntime.Cache;

        ///// <summary>
        ///// 默认到期时间
        ///// </summary>
        //private static int _timeout = 60;//单位：分

        #region 单例

        /// <summary>
        /// BaseCacheStrategy的构造函数
        /// </summary>
        LocalCacheStrategy()
        {
        }

        //静态BaseCacheStrategy
        public static LocalCacheStrategy<T> Instance
        {
            get
            {
                return Nested.instance; //返回Nested类中的静态成员instance
            }
        }

        class Nested
        {
            static Nested()
            {
            }

            //将instance设为一个初始化的BaseCacheStrategy新实例
            internal static readonly LocalCacheStrategy<T> instance = new LocalCacheStrategy<T>();
        }

        #endregion

        #region ICacheStrategy 成员

        public string CacheSetKey { get; set; }

        public ICacheLock BeginCacheLock(string resourceName, string key, int retryCount = 0, TimeSpan retryDelay = new TimeSpan())
        {
            return new LocalCacheLock(this, resourceName, key, retryCount, retryDelay).LockNow();
        }

        public virtual void InsertToCache(string key, T obj)
        {
            this.InsertToCache(key, obj, 1440 /*默认分钟*/);
        }

        public virtual void InsertToCache(string key, T obj, int timeout)
        {
            if (string.IsNullOrEmpty(key) || obj == null)
            {
                return;
            }
            cache.Insert(key, obj, null, DateTime.Now.AddMinutes(timeout), System.Web.Caching.Cache.NoSlidingExpiration,
                System.Web.Caching.CacheItemPriority.High, null);
        }

        public virtual void InsertToCache(string key, T obj, int timeout, CacheDependency dependencies)
        {
            if (string.IsNullOrEmpty(key) || obj == null)
            {
                return;
            }
            cache.Insert(key, obj, dependencies, DateTime.Now.AddMinutes(timeout),
                System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
        }

        public virtual void RemoveFromCache(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            cache.Remove(key);
        }

        public virtual T Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return cache.Get(key) as T;
        }

        public IList<T> GetAll()
        {
            //TODO:临时方案
            List<string> cacheKeys = new List<string>();
            IDictionaryEnumerator cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                cacheKeys.Add(cacheEnum.Key.ToString());
            }

            var objectKeys = cacheKeys.Where(z => z.StartsWith(CacheSetKey));
            List<T> objects = new List<T>();
            foreach (var objectKey in objectKeys)
            {
                objects.Add(cache[objectKey] as T);
            }
            return objects;
        }

        public IList<T> GetAll(string key)
        {
            List<string> cacheKeys = new List<string>();
            IDictionaryEnumerator cacheEnum = cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                cacheKeys.Add(cacheEnum.Key.ToString());
            }
            var objectKeys = cacheKeys.Where(z => z.StartsWith(key)).ToList();
            List<T> objects = new List<T>();
            foreach (var objectKey in objectKeys)
            {
                objects.Add(cache[objectKey] as T);
            }
            return objects;
        }


        public bool CheckExisted(string key)
        {
            return cache.Get(key) != null;
        }

        /// <summary>
        /// 将返回所有类型的缓存数量
        /// </summary>
        /// <returns></returns>
        public int GetCountForType()
        {
            return cache.Count;
            //List<string> cacheKeys = new List<string>();
            //IDictionaryEnumerator cacheEnum = cache.GetEnumerator();
            //while (cacheEnum.MoveNext())
            //{
            //    cacheKeys.Add(cacheEnum.Key.ToString());
            //}
            //Collection<string> s = 
            //var needKeys = cacheKeys.Where(z => z.StartsWith(CacheSetKey));
            //return needKeys.Count();
        }

        public long GetCount()
        {
            return cache.Count;
        }

        public void UpdateData(string key, object obj)
        {
            cache[key] = obj;
        }

        #endregion
    }
}