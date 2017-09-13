/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：LocalCacheLock.cs

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
using System.Threading;
using System.Threading.Tasks;
using Senparc.Weixin;

namespace Senparc.Core.Cache.Lock
{
    public class LocalCacheLock : BaseCacheLock 
    {
        private ILocalCacheStrategy _localStrategy;
        public LocalCacheLock(ILocalCacheStrategy stragegy, string resourceName, string key, int retryCount, TimeSpan retryDelay)
            : base(stragegy, resourceName, key, retryCount, retryDelay)
        {
            _localStrategy = stragegy;
        }

        private static Dictionary<string, object> LockPool = new Dictionary<string, object>();
        private static Random _rnd = new Random();

        private bool RetryLock(string resourceName, int retryCount, TimeSpan retryDelay, Func<bool> action)
        {
            int currentRetry = 0;
            int maxRetryDelay = (int)retryDelay.TotalMilliseconds;
            while (currentRetry++ < retryCount)
            {
                if (action())
                {
                    return true;//取得锁
                }
                Thread.Sleep(_rnd.Next(maxRetryDelay));
            }
            return false;
        }

        public override bool Lock(string resourceName)
        {
            return Lock(resourceName, 9999 /*暂时不限制*/, new TimeSpan(0, 0, 0, 0, 20));
        }

        public override bool Lock(string resourceName, int retryCount, TimeSpan retryDelay)
        {
            var successful = RetryLock(resourceName, retryCount, retryDelay, () =>
            {
                try
                {
                    if (LockPool.ContainsKey(resourceName))
                    {
                        return false;//已被别人锁住，没有取得锁
                    }
                    else
                    {
                        LockPool.Add(resourceName, new object());//创建锁
                        return true;//取得锁
                    }
                }
                catch (Exception ex)
                {
                    WeixinTrace.Log("本地同步锁发生异常：" + ex.Message);
                    return false;
                }
            }
                           );
            return successful;
        }

        public override void UnLock(string resourceName)
        {
            LockPool.Remove(resourceName);
        }
    }
}
