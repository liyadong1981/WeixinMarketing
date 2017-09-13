/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：BaseStringDictionaryCache.cs

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
using Senparc.Core.Models;

namespace Senparc.Core.Cache
{
    public interface IBaseStringDictionaryCache<TValue> : IBaseStringDictionaryCache<TValue, TValue> where TValue : class,new()
    {
    }
    public interface IBaseStringDictionaryCache<TValue, TEntity> : IBaseDictionaryCache<string, TValue, TEntity> where TValue : class,new()
    {

    }

    public abstract class BaseStringDictionaryCache<TValue> : BaseStringDictionaryCache<TValue, TValue>,
                                                                       IBaseStringDictionaryCache<TValue>
        where TValue : class, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CACHE_KEY"></param>
        /// <param name="db"></param>
        /// <param name="timeOut">单位：分钟。1440为一天。</param>
        public BaseStringDictionaryCache(string CACHE_KEY, ISqlClientFinanceData db, int timeOut)
            : base(CACHE_KEY, db, timeOut)
        {
            base.TimeOut = timeOut;
        }
    }

    public abstract class BaseStringDictionaryCache<TValue, TEntity> : BaseDictionaryCache<string, TValue, TEntity>, IBaseStringDictionaryCache<TValue, TEntity> 
        where TValue : class,new()
        where TEntity : class, new()
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CACHE_KEY"></param>
        /// <param name="db"></param>
        /// <param name="timeOut">单位：分钟。1440为一天。</param>
        public BaseStringDictionaryCache(string CACHE_KEY, ISqlClientFinanceData db, int timeOut)
            : base(CACHE_KEY, db, timeOut)
        {
            base.TimeOut = timeOut;
        }
    }
}
