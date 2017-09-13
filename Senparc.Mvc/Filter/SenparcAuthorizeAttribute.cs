/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：SenparcAuthorizeAttribute.cs

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
using Senparc.Core;
using Senparc.Core.Enums;
using System.Web.Routing;
using Senparc.Core.Cache;
using Senparc.Core.Extensions;
using Senparc.Core.Utility;
using System.Text.RegularExpressions;
using Senparc.Service;
using Senparc.Log;

namespace Senparc.Mvc
{
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
            Justification = "Unsealed so that subclassed types can set properties in the default constructor or override our behavior.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
    public class SenparcAuthorizeAttribute : FilterAttribute,/* AuthorizeAttribute,*/ IAuthorizationFilter
    {
        //public UserRoles[] UserRoles { get; set; }
        //private IList<UserType> UserTypes { get; set; }

        //public string[] ForAuthorityActionsPrefix { get; set; }
        private string Message { get; set; }
        //public UserRole_Type[] RoleTypes { get; set; }

        // This method must be thread-safe since it is called by the thread-safe OnCacheAuthorization() method.

        public SenparcAuthorizeAttribute()
        { }


        protected bool IsLogined(HttpContextBase httpContext)
        {
            return httpContext != null && httpContext.User.Identity.IsAuthenticated;
        }

        protected virtual bool AuthorizeCore(HttpContextBase httpContext)
        {
            //return true;
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;
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
            ////需要特殊验证的Action
            //if (this.ForAuthorityActionsPrefix != null)
            //{
            //    string actionName = filterContext.RequestContext.RouteData.GetRequiredString("action").ToUpper();
            //    if (this.ForAuthorityActionsPrefix.FirstOrDefault(p => actionName.StartsWith(p.ToUpper())) == null)
            //    {
            //        return;//此Action不需要特殊验证，返回。
            //    }
            //}

            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            //判断不能用IP直接登录
            var url = filterContext.HttpContext.Request.Url.Host;

            if (AuthorizeCore(filterContext.HttpContext))
            {
                // ** IMPORTANT **
                // Since we're performing authorization at the action level, the authorization code runs
                // after the output caching module. In the worst case this could allow an authorized user
                // to cause the page to be cached, then an unauthorized user would later be served the
                // cached page. We work around this by telling proxies not to cache the sensitive page,
                // then we hook our custom authorization code into the caching mechanism so that we have
                // the final say on whether a page should be served from the cache.

                HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
                cachePolicy.SetProxyMaxAge(new TimeSpan(0));
                cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
            }
            else
            {
                // auth failed, redirect to login page

                //if (filterContext.RouteData.DataTokens["area"] != null
                //    && filterContext.RouteData.DataTokens["area"].ToString().ToUpper() == "ADMIN")
                //{
                //    filterContext.Controller.TempData["AdminLogin"] = true;
                //}

                //todo: to a special page
                filterContext.Result = new HttpUnauthorizedResult();
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
