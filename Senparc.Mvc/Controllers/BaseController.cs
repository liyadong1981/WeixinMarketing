/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：BaseController.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Web.Mvc;
using StructureMap;
using Senparc.Core.Extensions;
using System.Web.Routing;
using System.Net;
using Senparc.Core.Enums;
using Senparc.Core.Models;
using Senparc.Core.Cache;
using Senparc.Mvc.Filter;
using BaseVD = Senparc.Core.Models.VD;

namespace Senparc.Mvc.Controllers
{
    //[RequireHttps]
    [SenparcHandleError]
    public class BaseController : AsyncController
    {
        //private ISystemConfigService _systemConfigService;
        protected DateTime PageStartTime { get; set; }
        protected DateTime PageEndTime { get; set; }
        protected FullSystemConfig FullSystemConfig { get; set; }
        public FullAdminUserInfo FullAdminUserInfo { get; set; }

        public BaseController()
        {
            PageStartTime = DateTime.Now;
        }


        public string UserName
        {
            get
            {
                return User.Identity.IsAuthenticated ? User.Identity.Name : null;
            }
        }

        public bool IsAdmin
        {
            get
            {
                return Session["AdminLogin"] as string == "1";
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            TempData["Messager"] = TempData["Messager"];
            var fullSystemCache = ObjectFactory.GetInstance<IFullSystemConfigCache>();
            FullSystemConfig = fullSystemCache.Data;

            if (User.Identity.IsAuthenticated)
            {
                var FullAdminCache = ObjectFactory.GetInstance<IFullAdminUserInfoCache>();
                FullAdminUserInfo = FullAdminCache.GetObject(User.Identity.Name);
            }

            base.OnActionExecuting(filterContext);
        }


        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {

            if (filterContext.Controller.ViewData.Model is BaseVD.IBaseVD)
            {
                var vd = filterContext.Controller.ViewData.Model as BaseVD.IBaseVD;
                vd.UserName = this.UserName;
                vd.IsAdmin = this.IsAdmin;
                vd.RouteData = this.RouteData;
                vd.FullAdminUserInfo = FullAdminUserInfo;
                vd.FullSystemConfig = FullSystemConfig;

                vd.MessagerList = vd.MessagerList ?? new List<Messager>();
                if (TempData["Messager"] as List<Messager> != null)
                {

                    vd.MessagerList.AddRange(TempData["Messager"] as List<Messager>);
                }
                else
                {
                    if (!ModelState.IsValid)
                    {
                        vd.MessagerList.Add(new Messager(MessageType.error, "提交信息有错误，请检查。"));
                    }
                }
            }

            if (filterContext.Controller.ViewData.Model is BaseVD.IBaseUiVD)
            {
                var vd = filterContext.Controller.ViewData.Model as BaseVD.IBaseUiVD;
                PageEndTime = DateTime.Now;
                vd.PageStartTime = PageStartTime;
                vd.PageEndTime = PageEndTime;
            }

            base.OnResultExecuting(filterContext);
        }

        /// <summary>
        /// 默认转到Edit页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return RedirectToAction("Edit");
        }

        protected override void HandleUnknownAction(string actionName)
        {
            string url = Url.Action("Error404", "Error", new { aspxerrorpath = Request.Url.PathAndQuery/*, t = DateTime.Now.Ticks*/ });
            this.Response.Redirect(url, true);
            //base.HandleUnknownAction(actionName);
        }

        #region 成功或错误提示页面

        [NonAction]
        public virtual ActionResult RenderSuccess(string message, string backUrl = null, string backAction = null, string backController = null, RouteValueDictionary backRouteValues = null, BaseVD.SuccessVD vd = null)
        {
            if (backUrl.IsNullOrEmpty())
            {
                backAction = backAction ?? RouteData.GetRequiredString("action");
                backController = backController ?? RouteData.GetRequiredString("controller");
                backRouteValues = backRouteValues ?? new RouteValueDictionary();

            }
            vd = vd ?? new BaseVD.SuccessVD()
           {
               Message = message,
               BackUrl = backUrl,
               BackAction = backAction,
               BackController = backController,
               BackRouteValues = backRouteValues
           };
            return View("Success", vd);
        }

        [NonAction]
        public virtual ActionResult RenderUnauthorized()
        {
            TempData["AuthorityNotReach"] = "无权操作，请联系管理员！";
            return new HttpUnauthorizedResult();
        }

        [NonAction]
        public virtual ActionResult RenderError(string message)
        {
            //保留原有的controller和action信息
            ViewData["FakeControllerName"] = RouteData.GetRequiredString("controller");
            ViewData["FakeActionName"] = RouteData.GetRequiredString("action");

            return View("Error", new BaseVD.Error_ExceptionVD
            {
                HandleErrorInfo = new HandleErrorInfo(new Exception(message), Url.RequestContext.RouteData.GetRequiredString("controller"), Url.RequestContext.RouteData.GetRequiredString("action"))
            });
        }
        [NonAction]
        public ActionResult RenderError404()
        {
            //return RedirectToAction("Error404", "Error", new { aspxerrorpath = Request.Url.PathAndQuery });
            return Error404(
                Request.Url
                .ShowWhenNullOrEmpty(new Uri("http://localhost/"))
                .PathAndQuery);//不转向
        }
        /// <summary>
        /// 显示404页面
        /// </summary>
        /// <param name="aspxerrorpath"></param>
        /// <returns></returns>
        [NonAction]
        public virtual ActionResult Error404(string aspxerrorpath = null)
        {
            if (string.IsNullOrEmpty(aspxerrorpath) && Request.UrlReferrer != null)
            {
                aspxerrorpath = Request.UrlReferrer.PathAndQuery;
            }
            var vd = new BaseVD.Error_Error404VD
            {
                Url = aspxerrorpath
            };
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            var result = View("Error404", vd);//HttpNotFound();
            return result;
        }

        #endregion

        /// <summary>
        /// 显示Json结果
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="result">数据，如果失败，可以为字符串的说明</param>
        /// <returns></returns>
        [NonAction]
        public JsonResult RenderJsonSuccessResult(bool success, object result, JsonRequestBehavior jsonRequestBehavior)
        {
            return Json(new
            {
                Success = success,
                Result = result
            }, jsonRequestBehavior);
        }

        [NonAction]
        public JsonResult RenderJsonSuccessResult(bool success, object result, bool allowGet = false)
        {
            return RenderJsonSuccessResult(success, result, allowGet ? JsonRequestBehavior.AllowGet : JsonRequestBehavior.DenyGet);
        }

        public void SetMessager(MessageType messageType, string messageText, bool showClose = true)
        {
            if (TempData["Messager"] == null || !(TempData["Messager"] is List<Messager>))
            {
                TempData["Messager"] = new List<Messager>();
            }

            (TempData["Messager"] as List<Messager>).Add(new Messager(messageType, messageText, showClose));
        }
    }
}
