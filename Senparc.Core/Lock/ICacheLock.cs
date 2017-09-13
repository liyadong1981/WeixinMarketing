/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：ICacheLock.cs

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
    /// <summary>
    /// 缓存锁接口
    /// </summary>
    public interface ICacheLock : IDisposable
    {
        /// <summary>
        /// 是否成功获得锁
        /// </summary>
        bool LockSuccessful { get; set; }

        /// <summary>
        /// 立即开始锁定
        /// </summary>
        /// <returns></returns>
        ICacheLock LockNow();

        /// <summary>
        /// 开始锁
        /// </summary>
        /// <param name="resourceName"></param>
        bool Lock(string resourceName);

        /// <summary>
        /// 开始锁，并设置重试条件
        /// </summary>
        /// <param name="resourceName"></param>
        /// <param name="retryCount"></param>
        /// <param name="retryDelay"></param>
        /// <returns></returns>
        bool Lock(string resourceName, int retryCount, TimeSpan retryDelay);

        //bool IsLocked(string resourceName);

        //释放锁
        void UnLock(string resourceName);
    }
}
