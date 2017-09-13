/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：StructureMapControllerFactory.cs

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
using System.Web;
using StructureMap;

namespace Senparc.Web
{
    public class StructureMapControllerFactory : DefaultControllerFactory
    {
        public static string StrcutureMapStartupTime;
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            try
            {
                if (controllerType == null)
                {
                    UrlHelper urlHelper = new UrlHelper(requestContext);
                    string url = urlHelper.Action("Error404", "Error", new { area = "", aspxerrorpath = requestContext.HttpContext.Request.Url.PathAndQuery });
                    HttpContext.Current.Response.Redirect(url);
                }
                if (!typeof(IController).IsAssignableFrom(controllerType))
                {
                    string msg = string.Format(
                        "Type requested is not a controller: {0}",
                        controllerType == null ? "" : controllerType.Name);
                    Log.LogUtility.WebLogger.ErrorFormat("{0}，{1}", requestContext.HttpContext.Request.Url, msg);
                    throw new ArgumentException(msg,
                           "controllerType");
                }

                return ObjectFactory.GetInstance(controllerType) as IController;
            }
            catch (StructureMapException)
            {
                System.Diagnostics.Debug.WriteLine(ObjectFactory.WhatDoIHave());
                throw;
            }
        }
    }
}
