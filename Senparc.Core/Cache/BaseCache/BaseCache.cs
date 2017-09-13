/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：BaseCache.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Web.Caching;
using Senparc.Core.Cache.BaseCache;
using Senparc.Core.Config;
using Senparc.Core.Enums;
using Senparc.Core.Models;

namespace Senparc.Core.Cache
{
    public abstract class BaseCache<T> : IBaseCache<T> where T : class, new()
    {
        protected virtual bool UpdateToCache(string key, T obj)
        {
            Cache.UpdateData(key, obj);
            return true;
        }

        public BaseCache() { }

        public delegate void UpdateWithBataBase(T obj);

        protected ISqlClientFinanceData _db;

        protected string CacheKey;
        private T _data;

        public DateTime CacheTime { get; set; }
        public DateTime CacheTimeOut { get; set; }

        //private ICacheStrategy _cache;

        /// <summary>
        /// 缓存策略。
        /// 请尽量不要再BaseCache以外调用这个对象的方法，尤其Cache的Key在DictionaryCache中是会被重新定义的
        /// </summary>
        public IBaseCacheStrategy<T> Cache { get; set; }
        /// <summary>
        /// 超时时间，1400分钟为1天。
        /// </summary>
        public int TimeOut { get; set; }

        public BaseCache(string cacheKey)
            : this(cacheKey, null)
        { }

        public BaseCache(string cacheKey, ISqlClientFinanceData db)
        {
            CacheKey = cacheKey;
            _db = db ?? StructureMap.ObjectFactory.GetInstance<ISqlClientFinanceData>();
            if (TimeOut == 0)
            {
                TimeOut = 1440;
            }


            Cache = CacheStrategyFactory.GetCacheStrategy<T>();
            Cache.CacheSetKey = cacheKey;//设置缓存集合键，必须提供
        }

        /// <summary>
        /// Data不能在Update()方法中调用，否则会引发循环调用。Update()方法中应该使用SetData()方法
        /// Data只适用于简单类型，如果缓存类型为列表，则不适用
        /// </summary>
        public virtual T Data
        {
            get
            {
                if (_data != null)
                {
                    return _data;
                }

                if (Cache == null)
                {
                    var msg = "Cache==null!系统调试记录cache长久以来的一个bug。";
                    throw new Exception(msg);
                }

                if (Cache.Get(CacheKey) == null)
                {
                    _data = this.Update();
                }
                return Cache.Get(CacheKey);
            }
            set
            {
                //if (Data != value)//TODO:这一句的意义？
                {
                    Cache.InsertToCache(CacheKey, value, TimeOut);
                }
            }
        }

        /// <summary>
        /// 设置整个缓存数据
        /// </summary>
        /// <param name="value"></param>
        /// <param name="timeOut"></param>
        /// <param name="dependencies"></param>
        /// <param name="updateWithDatabases"></param>
        public virtual void SetData(T value, int timeOut, CacheDependency dependencies, UpdateWithBataBase updateWithDatabases)
        {
            Cache.InsertToCache(CacheKey, value, timeOut, dependencies);

            //记录缓存时间
            this.CacheTime = DateTime.Now;
            this.CacheTimeOut = this.CacheTime.AddMinutes(timeOut);

            if (updateWithDatabases != null)
            {
                updateWithDatabases.Invoke(value);
            }
        }

        public virtual void RemoveCache()
        {
            Cache.RemoveFromCache(CacheKey);
        }

        public virtual T Update()
        {
            return null;
        }

        public virtual void UpdateToDatabase(T obj)
        {
        }

    }
}
