/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：IBaseCache.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
namespace Senparc.Core.Cache
{
    public interface IBaseCache<T>
       where T : class, new()
    {
        IBaseCacheStrategy<T> Cache { get; set; }
        /// <summary>
        /// Data不能在Update()方法中调用，否则会引发循环调用。Update()方法中应该使用SetData()方法
        /// </summary>
        T Data { get; set; }
        DateTime CacheTime { get; set; }
        DateTime CacheTimeOut { get; set; }
        void RemoveCache();
        void SetData(T value, int timeOut, System.Web.Caching.CacheDependency dependencies, BaseCache<T>.UpdateWithBataBase updateWithDatabases);
        T Update();
        void UpdateToDatabase(T obj);

        ///// <summary>
        ///// 更新到缓存
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="obj"></param>
        ///// <returns></returns>
        //bool UpdateToCache(string key, T obj);
    }
}
