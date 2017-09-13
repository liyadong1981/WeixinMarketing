/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：RedisCacheLock.cs

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
    public class RedisCacheLock<T> : BaseCacheLock where T : class, new()
    {
        private Redlock.CSharp.Redlock _dlm;
        private Redlock.CSharp.Lock _lockObject;

        private IRedisStrategy _redisStragegy;

        public RedisCacheLock(IRedisStrategy stragegy, string resourceName, string key, int retryCount, TimeSpan retryDelay)
            : base(stragegy, resourceName, key, retryCount, retryDelay)
        {
            _redisStragegy = stragegy;
        }

        public override bool Lock(string resourceName)
        {
            return Lock(resourceName, 0, new TimeSpan());
        }

        public override bool Lock(string resourceName, int retryCount, TimeSpan retryDelay)
        {
            if (retryCount != 0)
            {
                _dlm = new Redlock.CSharp.Redlock(retryCount, retryDelay, _redisStragegy._client);
            }
            else if (_dlm == null)
            {
                _dlm = new Redlock.CSharp.Redlock(_redisStragegy._client);
            }

            var successfull = _dlm.Lock(resourceName, new TimeSpan(0, 0, 10), out _lockObject);
            return successfull;
        }

        public override void UnLock(string resourceName)
        {
            if (_lockObject != null)
            {
                _dlm.Unlock(_lockObject);
            }
        }
    }
}
