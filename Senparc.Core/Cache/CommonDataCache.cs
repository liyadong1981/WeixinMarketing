/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：CommonDataCache.cs

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

namespace Senparc.Core.Cache
{
    public class CommonDataCache<T> : BaseCache<T>, ICommonDataCache<T> where T : class,new()
    {
        public const string CACHE_KEY = "__CommonDataCache";
        string _key;
        Func<T> _fun;

        /// <summary>
        /// 公用缓存模块，默认超时时间：1440分钟（1天）
        /// </summary>
        /// <param name="key">全局唯一Key（只需要确保在CommonDataCache模块内唯一）</param>
        /// <param name="fun"></param>
        public CommonDataCache(string key, Func<T> fun)
            :this(key, 1440, fun)
        {
        }

        /// <summary>
        /// 公用缓存模块
        /// </summary>
        /// <param name="key">全局唯一Key（只需要确保在CommonDataCache模块内唯一）</param>
        /// <param name="timeout">缓存时间（分钟）</param>
        /// <param name="fun"></param>
        public CommonDataCache(string key, int timeout, Func<T> fun)
            : base(CACHE_KEY + key)
        {
            _key = CACHE_KEY + key;
            base.TimeOut = timeout;
            this._fun = fun;
        }

        public override T Update()
        {
            base.SetData(_fun(), base.TimeOut, null, null);
            return base.Data;
        }
    }
}
