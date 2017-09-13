/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：SiteConfig.cs

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
using System.Configuration;
using System.IO;
using Senparc.Core.Enums;
using Senparc.Core.Models;
using Senparc.Core.Utility;
using System.Web;
using System.Threading;
using System.Web.Configuration;
using Senparc.Core.Extensions;

namespace Senparc.Core.Config
{
    public static class SiteConfig
    {
        private static string _applicationPath;
        public static string ApplicationPath
        {
            get
            {
                if (_applicationPath == null)
                {
                    string path = Senparc.Core.Utility.Server.HttpContext.Request.ApplicationPath;
                    if (path.EndsWith("/"))
                    {
                        _applicationPath = path.Substring(0, path.Length - 1);
                    }
                    else
                    {
                        _applicationPath = path;
                    }
                }
                return _applicationPath;
            }
        }

        public readonly static string VERSION = "1.3.3";
        public static string SenparcConfigDirctory = "~/App_Data/DataBase/";
        public const string AntiForgeryTokenSalt = "WEIXIN_MARKETING__SENPARC";
        public static readonly long OfficalWeiboUserId = 2513419820;
        //public const string DomainName = "http://azure.senparc.com";//TODO 修改域名
        public const string DefaultTemplate = "default";
        public const string DATABASE_ENCRYPT_KEY = "az@Ure^25HYUss1$3543";//数据库字符串加密密钥
        public const string PASSWORDSALT = "aAAB45f8e6";
        public const string WEIXINREMARK = "";
        //public const string APPID = "wx669ef95216eef885";//微信 
        //public const string APPSECRET = "c27e1cd2e6a5d697d767baaf5b91132f";//微信 TODO：盛派小助手
        public const string MCHID = "微信支付MCHID";//微信支付 TODO：需要填写
        public const string MCHKEY = "微信支付KEY";//微信支付 TODO：Key
        //public const string TENPAYAPPID = "";//微信支付【企业付款】
        public const string DEFAULT_AVATAR = "11";//默认头像地址
        /// <summary>
        /// 域名http://xx.xxx.com
        /// </summary>
        public static string DomainName
        {
            get { return ConfigurationManager.AppSettings["DomainName"]; }
        }
        /// <summary>
        /// AppId
        /// </summary>
        public static string AppId
        {
            get { return ConfigurationManager.AppSettings["AppId"]; }
        }
        /// <summary>
        /// AppSecret
        /// </summary>
        public static string AppSecret
        {
            get { return ConfigurationManager.AppSettings["AppSecret"]; }
        }


        /// <summary>
        /// 情绪识别ApiKey
        /// </summary>
        public static string EmotionKey
        {
            get { return ConfigurationManager.AppSettings["EmotionKey"]; }
        }
        /// <summary>
        /// 计算机视觉ApiKey
        /// </summary>
        public static string VisionKey
        {
            get { return ConfigurationManager.AppSettings["VisionKey"]; }
        }


        public static int UserExpires
        {//用户登录有效时间（单位：小时）
            get
            {
                int expires = 1;
                int.TryParse(ConfigurationManager.AppSettings["UserExpires"], out expires);
                return expires;
            }
        }

        public static string EncryptKey
        {
            get { return ConfigurationManager.AppSettings["EncryptKey"]; }
        }

        /// <summary>
        /// 开发者收入比例
        /// </summary>
        public static readonly long DeveloperIncomRate = (long)0.5;

        /// <summary>
        /// WBS格式
        /// </summary>
        public static readonly string WBSFormat = "000";

        /// <summary>
        /// 最大自动发送Email次数
        /// </summary>
        public static readonly int MaxSendEmailTimes = 5;
        /// <summary>
        /// 用户在线不活动过期时间(分钟)
        /// </summary>
        public static readonly int UserOnlineTimeoutMinutes = 10;
        /// <summary>
        /// 最多免验证码尝试登陆次数
        /// </summary>
        public static readonly int TryLoginTimes = 1;
        /// <summary>
        /// 最大数据库备份文件个数
        /// </summary>
        public static readonly int MaxBackupDatabaseCount = 200;

        /// <summary>
        /// 最多免验证码尝试登录次数
        /// </summary>
        public static readonly int TryUserLoginTimes = 3;

        ///// <summary>
        ///// 自动短信到期提示时间点（天）
        ///// </summary>
        //public static int[] SmsExpireAlertLastDays = new int[] { 30, 10, 3, 1 };//提醒日期

        //统计数据
        public static DateTime ApplicationStartTime = DateTime.Now;
        public static int PageViewCount { get; set; }//网站启动后前台页面浏览量

        //异步线程
        public static Dictionary<string, Thread> AsynThread = new Dictionary<string, Thread>();//后台运行线程

        private static bool isDebug = false;
        private static bool isDebugChecked = false;

        /// <summary>
        /// 判断是否是测试状态，如果测试状态为true，则强制认为IsDebug=true;
        /// </summary>
        public static bool IsTest = false;
        public static bool IsDebug
        {
            get
            {
                if (IsTest)
                {
                    return true;
                }

                //每次都重新判断
                if (!isDebugChecked)
                {
                    isDebug = ConfigurationManager.AppSettings["SystemIsDebug"] == "true"
                                      //HttpContext context = Server.HttpContext;
                                      //isDebug = (context.IsDebuggingEnabled && context.Request.IsLocal)
                                      //              || context.Request.Url.Host.ToUpper().Contains("LOCAL")
                                      //              || context.Request.Url.HostNameType == UriHostNameType.IPv4
                                      //    //       (!ConfigurationManager.ConnectionStrings["SenparcEntities"].ConnectionString.Contains("a0420205424"))
                                      ;
                    isDebugChecked = true;
                }
                return isDebug;
            }
        }

        static readonly string _cacheTypeString = WebConfigurationManager.AppSettings["CacheType"];
        private static CacheType? _cacheType;

        /// <summary>
        /// 缓存类型
        /// </summary>
        public static CacheType CacheType
        {
            get
            {
                if (_cacheType == null)
                {
                    if (_cacheTypeString.IsNullOrEmpty())
                    {
                        _cacheType = CacheType.Location;
                    }
                    else
                    {
                        switch (_cacheTypeString.ToUpper())
                        {
                            case "MEMCACHED":
                                _cacheType = CacheType.Memcached;
                                break;
                            case "REDIS":
                                _cacheType = File.Exists(Server.GetMapPath("~/App_Data/UseRedis.txt"))
                                    ? CacheType.Redis //必须文件存在才会启用
                                    : CacheType.Location;
                                break;
                            default:
                                _cacheType = CacheType.Location;
                                break;
                        }
                    }
                }

                return _cacheType.Value;
            }
            set { _cacheType = value; }
        }
    }
}
