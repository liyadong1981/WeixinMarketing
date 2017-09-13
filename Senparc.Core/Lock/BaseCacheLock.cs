/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：BaseCacheLock.cs

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
using System.Threading.Tasks;

namespace Senparc.Core.Cache.Lock
{
    public abstract class BaseCacheLock : ICacheLock
    {
        protected string _resourceName;
        protected IBaseCacheStrategy _cacheStragegy;
        protected int _retryCount;
        protected TimeSpan _retryDelay;
        public bool LockSuccessful { get; set; }

        protected BaseCacheLock(IBaseCacheStrategy stragegy, string resourceName, string key, int retryCount, TimeSpan retryDelay)
        {
            _cacheStragegy = stragegy;
            _resourceName = resourceName + key;/*加上Key可以针对某个AppId加锁*/
            _retryCount = retryCount;
            _retryDelay = retryDelay;
        }

        public void Dispose()
        {
            UnLock(_resourceName);
        }

        public ICacheLock LockNow()
        {
            if (_retryCount != 0 && _retryDelay.Ticks != 0)
            {
                LockSuccessful = Lock(_resourceName, _retryCount, _retryDelay);
            }
            else
            {
                LockSuccessful = Lock(_resourceName);
            }
            return this;
        }

        public abstract bool Lock(string resourceName);

        public abstract bool Lock(string resourceName, int retryCount, TimeSpan retryDelay);

        public abstract void UnLock(string resourceName);
    }
}
