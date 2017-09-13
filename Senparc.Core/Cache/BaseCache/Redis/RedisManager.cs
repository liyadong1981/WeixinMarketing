/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：RedisManager.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Senparc.Core.Cache.BaseCache.Redis
{
    /// <summary>
    /// Redis 链接管理
    /// </summary>
    public class RedisManager
    {
        #region ConnectionMultiplexer 单例

        /// <summary>
        /// _redis(ConnectionMultiplexer)单例
        /// </summary>
        internal static ConnectionMultiplexer _redis
        {
            get
            {
                return NestedRedis.instance;//返回Nested类中的静态成员instance
            }
        }

        class NestedRedis
        {
            static NestedRedis()
            {
            }
            //将instance设为一个初始化的ConnectionMultiplexer新实例
            internal static readonly ConnectionMultiplexer instance = GetManager();
        }

        #endregion

        /// <summary>
        /// 链接设置字符串
        /// </summary>
        public static string ConfigurationOption { get; set; }


        /// <summary>
        /// ConnectionMultiplexer
        /// </summary>
        public static ConnectionMultiplexer Manager
        {
            get { return _redis; }
        }

        /// <summary>
        /// 默认连接字符串
        /// </summary>
        /// <returns></returns>
        public static string GetDefaultConnectionString()
        {
            //return "localhost,defaultDatabase=4";//默认数据库
            return ConfigurationManager.AppSettings["Cache_Redis_Configuration"];//默认数据库
        }

        private static ConnectionMultiplexer GetManager(string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                if (ConfigurationOption == null)
                {
                    connectionString = GetDefaultConnectionString();
                }
                else
                {
                    return ConnectionMultiplexer.Connect(ConfigurationOptions.Parse(ConfigurationOption));
                }
            }

            //            var redisConfigInfo = RedisConfigInfo.GetConfig();
            //            #region options 设置说明

            //            /*
            //abortConnect ： 当为true时，当没有可用的服务器时则不会创建一个连接
            //allowAdmin ： 当为true时 ，可以使用一些被认为危险的命令
            //channelPrefix：所有pub/sub渠道的前缀
            //connectRetry ：重试连接的次数
            //connectTimeout：超时时间
            //configChannel： Broadcast channel name for communicating configuration changes
            //defaultDatabase ： 默认0到-1
            //keepAlive ： 保存x秒的活动连接
            //name:ClientName
            //password:password
            //proxy:代理 比如 twemproxy
            //resolveDns : 指定dns解析
            //serviceName ： Not currently implemented (intended for use with sentinel)
            //ssl={bool} ： 使用sll加密
            //sslHost={string}	： 强制服务器使用特定的ssl标识
            //syncTimeout={int} ： 异步超时时间
            //tiebreaker={string}：Key to use for selecting a server in an ambiguous master scenario
            //version={string} ： Redis version level (useful when the server does not make this available)
            //writeBuffer={int} ： 输出缓存区的大小
            //    */

            //            #endregion
            //            var options = new ConfigurationOptions()
            //            {
            //                ServiceName = redisConfigInfo.ServerList,

            //            };

            return ConnectionMultiplexer.Connect(connectionString);
        }
    }
}