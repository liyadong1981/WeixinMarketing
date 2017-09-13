/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：RedisStrategy.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Caching;
using log4net.Util;
using Senparc.Core.Cache.BaseCache.Redis;
using Senparc.Core.Cache.Lock;
using Senparc.Core.Config;
using Senparc.Core.Extensions;
using Senparc.Log;
using StackExchange.Redis;


namespace Senparc.Core.Cache
{
    public interface IRedisStrategy : IBaseCacheStrategy
    {
        ConnectionMultiplexer _client { get; set; }
    }

    public class RedisStrategy<T> : IBaseCacheStrategy<T>, IRedisStrategy where T : class, new()
    {
        //private static volatile System.Web.Caching.Cache cache = HttpRuntime.Cache;
        //private MemcachedClient _cache;
        //private RedisConfigInfo _config;
        public ConnectionMultiplexer _client { get; set; }
        private IDatabase _cache;

        #region 单例

        //静态SearchCache
        public static RedisStrategy<T> Instance
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
            internal static readonly RedisStrategy<T> instance = new RedisStrategy<T>();
        }

        #endregion

        static RedisStrategy()
        {
            DateTime dt1 = DateTime.Now;
            var manager = RedisManager.Manager;
            var cache = manager.GetDatabase();

            var testKey = Guid.NewGuid().ToString();
            var testValue = Guid.NewGuid().ToString();
            cache.StringSet(testKey, testValue);
            var storeValue = cache.StringGet(testKey);
            if (storeValue != testValue)
            {
                throw new Exception("RedisStrategy失效，没有计入缓存！");
            }

            cache.StringSet(testKey, (string)null);
            DateTime dt2 = DateTime.Now;
            LogUtility.Cache.Info("RedisStrategy正常启用，启动及测试耗时：{0}ms".With((dt2 - dt1).TotalMilliseconds));
        }

        public RedisStrategy()
        {
            //_config = RedisConfigInfo.GetConfig();
            _client = RedisManager.Manager;
            _cache = _client.GetDatabase();
        }

        ~RedisStrategy()
        {
            _client.Dispose(); //释放
            //GC.SuppressFinalize(_client);
        }

        private string GetFinalKey(string key)
        {
            return "{0}:{1}".With(CacheSetKey, key);
            //return "{0}".With(CacheSetKey);
        }

        #region ICacheStrategy 成员

        public string CacheSetKey { get; set; }
        public Type CacheSetType { get; set; }

        /// <summary>
        /// 获取 Server 对象
        /// </summary>
        /// <returns></returns>
        private IServer GetServer()
        {
            //https://github.com/StackExchange/StackExchange.Redis/blob/master/Docs/KeysScan.md
            var server = _client.GetServer(_client.GetEndPoints()[0]);
            return server;
        }

        public ICacheLock BeginCacheLock(string resourceName, string key, int retryCount = 0, TimeSpan retryDelay = new TimeSpan())
        {
            return new RedisCacheLock<T>(this, resourceName, key, retryCount, retryDelay).LockNow();
        }

        public virtual void InsertToCache(string key, T obj)
        {
            this.InsertToCache(key, obj, 1440);
        }

        public virtual void InsertToCache(string key, T obj, int timeout)
        {
            if (string.IsNullOrEmpty(key) || obj == null)
            {
                return;
            }

            var cacheKey = GetFinalKey(key);

            if (obj is IDictionary)
            {
                //Dictionary类型
            }

            _cache.StringSet(cacheKey, obj.Serialize());
            //_cache.HashSet(cacheKey, key, obj.Serialize());

#if DEBUG
            var value1 = _cache.StringGet(cacheKey); //正常情况下可以得到 //_cache.GetValue(cacheKey);
            //var value1 = _cache.HashGet(cacheKey, key); //正常情况下可以得到 //_cache.GetValue(cacheKey);
#endif
        }

        public virtual void InsertToCache(string key, T obj, int timeout, CacheDependency dependencies)
        {
            if (string.IsNullOrEmpty(key) || obj == null)
            {
                return;
            }
            var cacheKey = GetFinalKey(key);
            _cache.StringSet(cacheKey, obj.Serialize());
            //_cache.HashSet(cacheKey, key, obj.Serialize());

            //var hash = GetHash();

            //var result = _cache.SetEntryInHash(hash, cacheKey, obj);//_cache.GetAndSetValue(cacheKey, obj);
            //if (result == false)
            //{
            //    LogUtility.Cache.ErrorFormat("InsertToCache失败！key：{0}", key);
            //}

#if DEBUG
            //var value1 = _cache.GetFromHash(cacheKey); //_cache.GetValue(cacheKey);
            //var value2 = _cache.GetValueFromHash(hash, cacheKey);
            //var value3 = _cache.GetValue(cacheKey);
#endif
        }

        public virtual void RemoveFromCache(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return;
            }
            var cacheKey = GetFinalKey(key);

            //SenparcMessageQueue.OperateQueue();//延迟缓存立即生效
            _cache.KeyDelete(cacheKey); //删除键
            //_cache.HashDelete(cacheKey, key);
        }

        public virtual T Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            if (!CheckExisted(key))
            {
                return null;
                //InsertToCache(key, new ContainerItemCollection());
            }

            var cacheKey = GetFinalKey(key);

            var value = _cache.StringGet(cacheKey);
            //var value = _cache.HashGet(cacheKey, key);
            return StackExchangeRedisExtensions.Deserialize<T>(value);
        }

        public IList<T> GetAll()
        {
            var keys = GetServer().Keys();
            var list = new List<T>();
            foreach (var redisKey in keys)
            {
                list.Add(Get(redisKey));
            }
            return list;

            //_cache.;
            //var hash = GetHash();
            //return _cache.GetHashValues(hash);//.GetAllEntriesFromHash(hash);
        }

        /// <summary>
        /// 获取所有指定Key下所有Value【大数据量不要使用容易造成Redis线程阻塞】
        /// </summary>
        /// <param name="key">xxxxxx*</param>
        /// <returns></returns>
        public IList<T> GetAll(string key)
        {
            //TODO:consider using SCAN or sets
            var list = new List<T>();
            //var finalKey = GetFinalKey(key);
            //var hashList = _cache.HashGetAll(finalKey);
            //foreach (var hashEntry in hashList)
            //{
            //    var value = hashEntry.Value;
            //    list.Add(StackExchangeRedisExtensions.Deserialize<T>(value));
            //}

            var keys = GetServer().Keys(pattern: GetFinalKey(key + "*"));
            foreach (var redisKey in keys)
            {
                var value = _cache.StringGet(redisKey);
                list.Add(StackExchangeRedisExtensions.Deserialize<T>(value));
            }
            return list;
        }

        public bool CheckExisted(string key)
        {
            var cacheKey = GetFinalKey(key);
            return _cache.KeyExists(cacheKey);
            //return _cache.HashExists(cacheKey, key);
        }

        public int GetCountForType()
        {
            return GetServer().Keys(pattern: GetFinalKey("*")).Count();
           // return (int)_cache.HashLength(GetFinalKey(""));

            //return _client.GetAllKeys().Count(z => z.StartsWith(CacheSetKey));
        }

        public long GetCount()
        {
            var count = GetServer().Keys().Count();
            return count;
        }

        public void UpdateData(string key, object obj)
        {
            var cacheKey = GetFinalKey(key);
            _cache.StringSet(cacheKey, obj.Serialize());
            //_cache.HashSet(cacheKey, key, obj.Serialize());
        }

        #endregion
    }
}