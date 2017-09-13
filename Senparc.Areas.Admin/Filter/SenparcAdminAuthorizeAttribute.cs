/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：SenparcAdminAuthorizeAttribute.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Senparc.Mvc;
using Senparc.Core.Extensions;
using Senparc.Service;

namespace Senparc.Areas.Admin.Filter
{
    public class SenparcAdminAuthorizeAttribute : SenparcAuthorizeAttribute
    {
        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            IPrincipal user = httpContext.User;
            if (!base.IsLogined(httpContext))
            {
                return false;//未登录
            }

            var result = base.AuthorizeCore(httpContext);
            if (result)
            {

                if (!httpContext.Request.IsLocal)
                {
                    var adminSession = httpContext.Session["AdminLogin"] as string;
                    if (adminSession.IsNullOrEmpty())
                    {
                        try
                        {
                            IAdminUserInfoService userInfoService =
                                StructureMap.ObjectFactory.GetInstance<IAdminUserInfoService>();
                            userInfoService.Logout(); //强制退出登录
                        }
                        catch
                        {
                        }
                        result = false;
                    }
                }
            }
            return result;
        }
    }
}
