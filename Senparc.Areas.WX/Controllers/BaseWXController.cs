/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：BaseWXController.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using Senparc.Areas.WX.Filter;
using Senparc.Core.Cache;
using Senparc.Core.Enums;
using Senparc.Core.Models;
using Senparc.Mvc.Controllers;
using Senparc.Core.Extensions;
using Senparc.Areas.WX.Models.VD;
using Senparc.Service;
using Senparc.Weixin;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Helpers;
using StructureMap;
using Senparc.Mvc;
using Senparc.Core.Config;

namespace Senparc.Areas.WX.Controllers
{
    [WXAuthorize(ForNotAuthorityUrl = new[] { "Message/Item" })]
    public class BaseWXController : BaseController
    {
        public FullAccount FullAccount { get; set; }

        // public Shop CurrentShop { get; set; }

        public BaseWXController()
        {
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (SiteConfig.IsDebug && Session["OpenId"] == null)
            {
                FullAccount = new FullAccount()
                {
                    UserName = "Test",
                    WeixinOpenId = "Test123321",
                    Id = 1
                };
            }
            else
            {
                // Session["OpenId"]//为null不会通过WXAuthorize
                var openId = Session["OpenId"] == null ? null : Session["OpenId"].ToString();

                var fullAccountCache = ObjectFactory.GetInstance<IFullAccountCache>();
                FullAccount = fullAccountCache.GetObjectByOpenId(openId);
                if (FullAccount == null)
                {
                    filterContext.Result = RenderError("用户信息不存在！");
                }
            }

            base.OnActionExecuting(filterContext);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (filterContext.Controller.ViewData.Model is IBaseWXVD)
            {
                IBaseWXVD vd = filterContext.Controller.ViewData.Model as IBaseWXVD;
                vd.FullAccount = FullAccount;
            }
            base.OnResultExecuting(filterContext);
        }

        [NonAction]
        public ActionResult PartialAngularView(BaseWXVD vd)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(vd);
            }
            else
            {
                return View("../Home/Index", new Home_IndexVD());
            }
        }

        [NonAction]
        public ActionResult PartialAngularView(string viewName, BaseWXVD vd)
        {
            if (Request.IsAjaxRequest())
            {
                return PartialView(viewName, vd);
            }
            else
            {
                return View("../Home/Index", new Home_IndexVD());
            }
        }
    }
}
