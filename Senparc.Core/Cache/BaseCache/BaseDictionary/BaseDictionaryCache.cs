/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：BaseDictionaryCache.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Senparc.Core.Extensions;
using Senparc.Core.Models;
using Senparc.Log;
using Senparc.Weixin.HttpUtility;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Senparc.Core.Utility;

namespace Senparc.Core.Cache
{
    public interface IBaseDictionaryCache<TKey, TValue> : IBaseDictionaryCache<TKey, TValue, TValue>
        where TValue : class, new()
    {
    }

    public interface IBaseDictionaryCache<TKey, TValue, TEntity> : IBaseCache<TValue>
           where TValue : class, new()
    {
        TValue InsertObjectToCache(TKey key);
        TValue InsertObjectToCache(TKey key, TEntity obj);
        TValue GetObject(TKey key);
        void RemoveObject(TKey key);

        bool UpdateToCache(TKey key, TValue obj);
    }


    public abstract class BaseDictionaryCache<TKey, TValue> : BaseDictionaryCache<TKey, TValue, TValue>,
        IBaseDictionaryCache<TKey, TValue>
        where TValue : class, new()
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CACHE_KEY"></param>
        /// <param name="db"></param>
        /// <param name="timeOut">单位：分钟。1440为一天。</param>
        public BaseDictionaryCache(string CACHE_KEY, ISqlClientFinanceData db, int timeOut)
            : base(CACHE_KEY, db, timeOut)
        {
            base.TimeOut = timeOut;
        }
    }

    public abstract class BaseDictionaryCache<TKey, TValue, TEntity> :
        BaseCache<TValue>, IBaseDictionaryCache<TKey, TValue, TEntity>
        where TValue : class, new()
        where TEntity : class, new()
    {
        /// <summary>
        /// 获取缓存中最终的Key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string GetFinalCacheKey(TKey key)
        {
            //TODO:判断Key类型
            TypeCode code = key == null ? TypeCode.DBNull : Type.GetTypeCode(key.GetType());
            string keyCode = null;
            switch (code)
            {
                case TypeCode.DBNull: throw new Exception("Key不允许为空！");
                case TypeCode.String:
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.Double:
                case TypeCode.Single:
                    keyCode = key.ToString();
                    break;
                default:
                    code = TypeCode.Object;
                    keyCode = MD5.GetMD5Code(String.Concat(this.SerializeObject(key)), "");//将对象序列化，然后拼接成字符串并转成MD5，确保唯一性。性能上可能会有一些损失，所以尽量不要太复杂的类型做Key
                    break;
            }

            string finalKey = null;

            if (base.Cache is IRedisStrategy)
            {
                finalKey = "{0}".With(keyCode);
            }else
            {
                finalKey = "{0}@@@{1}".With(base.CacheKey, keyCode);//有的缓存策略可能不允许:作为分隔符[LocalCache 存在问题]
            }

            return finalKey;
        }

        protected virtual ArraySegment<byte> SerializeObject(object value)
        {
            using (var ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, value);

                return new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length);
            }
        }

        /// <summary>
        /// 获取指定Key下所有的子Key的Value
        /// </summary>
        /// <param name="key">xxx*</param>
        /// <returns></returns>
        protected virtual IList<TValue> GetObjectList(TKey key)
        {
            if (key == null)
            {
                return null;
            }
            var finalCacheKey = GetFinalCacheKey(key);
            return base.Cache.GetAll(finalCacheKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CACHE_KEY"></param>
        /// <param name="db"></param>
        /// <param name="timeOut">单位：分钟。1440为一天。</param>
        public BaseDictionaryCache(string CACHE_KEY, ISqlClientFinanceData db, int timeOut)
            : base(CACHE_KEY, db)
        {
            base.TimeOut = timeOut;

            //强制初始化
            //var load = base.Data;
            //var finalCacheKey = GetFinalCacheKey(key);
            //if (base.Cache.CheckExisted(finalCacheKey))
            //{

            //}
        }

        public override TValue Update()
        {
            return base.Update();
        }

        public abstract TValue InsertObjectToCache(TKey key);

        public virtual TValue InsertObjectToCache(TKey key, TEntity obj)
        {
            if (obj == null)
            {
                return null;
            }

            TValue fullObj = new TValue();
            var finalCacheKey = GetFinalCacheKey(key);
            if (fullObj is IBaseFullEntity<TEntity>)
            {
                try
                {
                    (fullObj as BaseFullEntity<TEntity>).CreateEntity(obj);

                    //if (base.Data == null)
                    //{
                    //    throw new Exception("base.Data=null");
                    //}
                    base.Cache.InsertToCache(finalCacheKey, fullObj);
                    return fullObj;
                }
                catch (Exception ex)
                {
                    //var msg = "系统调试记录cache长久以来的一个bug。发生错误：{0}。当前参数：base.Data：{1}（Count：{4}），key:{2}，obj：{3}。Null情况分别是：{4}，{5},{6}"
                    //    .With(ex.Message, base.Data, key, obj, base.Data == null, key == null, obj == null, base.Data.Count);
                    var msg = "系统调试记录cache长久以来的一个bug。发生错误：{0}。HttpRuntime.Cache=null:{1}。再次访问base.Data=null：{0}"
                        .With(ex.Message, System.Web.HttpRuntime.Cache == null, base.Data == null);//实际上这里base.Data还是为null
                    LogUtility.SystemLogger.Debug(msg, ex);
                    throw new Exception(msg, ex);
                }
            }
            else if (obj as TValue != null)
            {
                base.Cache.InsertToCache(finalCacheKey, obj as TValue);
                return obj as TValue;
            }

            base.Cache.InsertToCache(finalCacheKey, fullObj);
            return fullObj;
        }

        public virtual TValue GetObject(TKey key)
        {
            if (key == null)
            {
                return null;
            }

            var finalCacheKey = GetFinalCacheKey(key);

            if (base.Cache.CheckExisted(finalCacheKey))
            {
                return base.Cache.Get(finalCacheKey);
            }
            else
            {
                return InsertObjectToCache(key);
            }
        }

        public virtual void RemoveObject(TKey key)
        {
            var finalCacheKey = GetFinalCacheKey(key);

            if (base.Cache.CheckExisted(finalCacheKey))
            {
                base.Cache.RemoveFromCache(finalCacheKey);
            }
        }

        public virtual bool UpdateToCache(TKey key, TValue obj)
        {
            var finalKey = GetFinalCacheKey(key);
            return base.UpdateToCache(finalKey, obj);
        }
        

        public override void RemoveCache()
        {
            throw new Exception("不可以使用此方法");
        }
    }
}
