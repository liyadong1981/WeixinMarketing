/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：IBaseCacheStrategy.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using Senparc.Core.Cache.Lock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Caching;

namespace Senparc.Core.Cache
{

    public interface IBaseCacheStrategy
    {
        /// <summary>
        /// 开始一个同步锁
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="key"></param>
        /// <param name="retryCount"></param>
        /// <param name="retryDelay"></param>
        /// <returns></returns>
        ICacheLock BeginCacheLock(string resourceName, string key, int retryCount = 0,
                    TimeSpan retryDelay = new TimeSpan());

    }

    /// <summary>
    /// 公共缓存策略接口
    /// </summary>
    public interface IBaseCacheStrategy<T> : IBaseCacheStrategy where T : class, new()
    {
        /// <summary>
        /// 整个Cache集合的Key
        /// </summary>
        string CacheSetKey { get; set; }

        ///// <summary>
        ///// 缓存强类型
        ///// </summary>
        //Type CacheSetType { get; set; }


        /// <summary>
        /// 添加指定ID的对象
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        void InsertToCache(string key, T value);
        /// <summary>
        /// 添加指定ID的对象
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeout">单位：分</param>
        void InsertToCache(string key, T value, int timeout);
        /// <summary>
        /// 添加指定ID的对象，使用依赖项
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="value">缓存值</param>
        /// <param name="timeout">单位：分</param>
        /// <param name="dependencies">依赖项</param>
        void InsertToCache(string key, T value, int timeout, CacheDependency dependencies);

        /// <summary>
        /// 移除指定缓存键的对象
        /// </summary>
        /// <param name="key">缓存键</param>
        void RemoveFromCache(string key);

        /// <summary>
        /// 返回指定缓存键的对象
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns></returns>
        T Get(string key);

        /// <summary>
        /// 获取所有细信息
        /// </summary>
        /// <returns></returns>
        IList<T> GetAll();

        /// <summary>
        /// 获取指定Key下所有的子Key的Value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IList<T> GetAll(string key);
        /// <summary>
        /// 检查是否存在Key及对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool CheckExisted(string key);

        /// <summary>
        /// 获取缓存集合总数（注意：每个缓存框架的计数对象不一定一致！）
        /// </summary>
        /// <returns></returns>
        long GetCount();


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        void UpdateData(string key, object obj);


    }
}
