using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Senparc.Core;
using Senparc.Core.Cache;
using Senparc.Core.Config;
using Senparc.Mvc.IoC;
using Senparc.Mvc;
using Senparc.Weixin.Cache;
using Senparc.Weixin.Cache.Redis;
using Senparc.Weixin.Threads;
using StructureMap;
//using RedisManager = Senparc.Core.Cache.BaseCache.Redis.RedisManager;

namespace Senparc.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //Log4net
            {
                DateTime st = DateTime.Now;
                var log4netFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config");
                var log4netFileInfo = new System.IO.FileInfo(log4netFilePath);
                log4net.Config.XmlConfigurator.Configure(log4netFileInfo);
                DateTime et = DateTime.Now;
                Log.LogUtility.WebLogger.InfoFormat("系统启动，log4net初始化启动时间：{0}秒", (et - st).TotalSeconds);
            }

            //IoC
            {
                DateTime st = DateTime.Now;
                Bootstrapper.ConfigureStructureMap();
                ControllerBuilder.Current.SetControllerFactory(new StructureMapControllerFactory());
                DateTime et = DateTime.Now;
                StructureMapControllerFactory.StrcutureMapStartupTime = (et - st).TotalSeconds.ToString("##,##");
            }

            //Default controller
            ControllerBuilder.Current.DefaultNamespaces.Add("Senparc.Mvc.Controllers");

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //InitCaches();


            //注册微信缓存
            RegisterWeixinCache();

            RegisterWeixinThreads();//激活微信缓存（必须）

            //注册微信
            Senparc.Weixin.MP.Containers.AccessTokenContainer.Register(SiteConfig.AppId, SiteConfig.AppSecret, "盛派网络");
        }

        /// <summary>
        /// 自定义缓存策略
        /// </summary>
        private void RegisterWeixinCache()
        {
            //如果留空，默认为localhost（默认端口）

            #region  Redis配置

            var redisConfiguration = System.Configuration.ConfigurationManager.AppSettings["Cache_Redis_Configuration"];

            RedisManager.ConfigurationOption = redisConfiguration;

            //如果不执行下面的注册过程，则默认使用本地缓存

            if (!string.IsNullOrEmpty(redisConfiguration) && redisConfiguration != "Redis配置")
            {
                CacheStrategyFactory.RegisterObjectCacheStrategy(() => RedisObjectCacheStrategy.Instance); //Redis
            }

            #endregion
        }

        /// <summary>
        /// 激活微信缓存
        /// </summary>
        private void RegisterWeixinThreads()
        {
            ThreadUtility.Register();
        }

        private void InitCaches()
        {
            //TODO:初始化缓存可以写在这里
        }
    }
}
