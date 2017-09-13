/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：WXAuthorizeAttribute.cs

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
using System.Web.Mvc;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Security.Principal;
using Senparc.Core.Models;
using Senparc.Weixin;
using Senparc.Core;
using Senparc.Core.Config;
using Senparc.Core.Enums;
using System.Web.Routing;
using Senparc.Core.Cache;
using Senparc.Core.Extensions;
using Senparc.Core.Utility;
using System.Text.RegularExpressions;
using Senparc.Service;
using Senparc.Log;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using StructureMap;

namespace Senparc.Areas.WX.Filter
{
    /// <summary>
    /// 服务号OAuth2.0的验证
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
            Justification = "Unsealed so that subclassed types can set properties in the default constructor or override our behavior.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class WXAuthorizeAttribute : FilterAttribute, IAuthorizationFilter
    {

        public string[] ForAuthorityActionsPrefix { get; set; }
        public string[] ForNotAuthorityUrl { get; set; }
        private bool forceLogout = false;//被强制退出登录

        public WXAuthorizeAttribute()
        {
    
        }

        protected bool IsLogined(HttpContextBase httpContext)
        {
            //如果是本地测试
            if (Senparc.Core.Config.SiteConfig.IsDebug)
            {
                return true;
            }
            //
            //判断是否通过认证
            var openId = httpContext.Session["OpenId"] != null ? httpContext.Session["OpenId"].ToString() : null;
            if (openId == null)
            {
                //未通过验证

                return false;
            }
            return true;
        }

        protected virtual bool AuthorizeCore(HttpContextBase httpContext)
        {
            //return true;
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            //IPrincipal user = httpContext.User;
            if (!IsLogined(httpContext))
            {
                return false;//未登录
            }

            return true;
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
           
            //排除验证的Action
            if (this.ForNotAuthorityUrl != null)
            {
                string excludeAction = filterContext.RequestContext.RouteData.GetRequiredString("action").ToUpper();
                string excludeController = filterContext.RequestContext.RouteData.GetRequiredString("controller").ToUpper();
                string currentUrl = "{0}/{1}".With(excludeController, excludeAction);
                if (this.ForNotAuthorityUrl.Any(z => z.ToUpper().Equals(currentUrl)))
                {
                    return;//此Action不需要特殊验证，返回。
                }
            }

            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

           

            if (AuthorizeCore(filterContext.HttpContext))
            {
                
                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
            }
            else
            {
                var returnUrl = "{0}/WX/WeixinOAuth/MpCallback?callbackUrl={1}"
                                 .With(SiteConfig.DomainName, SiteConfig.DomainName + filterContext.HttpContext.Request.Url.PathAndQuery.ToString().UrlEncode());
                var getCodeUrl = OAuthApi.GetAuthorizeUrl(SiteConfig.AppId, returnUrl, "Azure", OAuthScope.snsapi_userinfo);
                filterContext.Result = new RedirectResult(getCodeUrl);
            }
        }

        // This method must be thread-safe since it is called by the caching module.
        protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            bool isAuthorized = AuthorizeCore(httpContext);
            return (isAuthorized) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
        }
    }
}
