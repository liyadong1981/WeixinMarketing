/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：LoginController.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System.Collections.Generic;
using System.Web.Mvc;
using Senparc.Areas.Admin.Models.VD;
using Senparc.Mvc;
using Senparc.Core.Extensions;
using Senparc.Core.Models;
using System.Text.RegularExpressions;
using Senparc.Log;
using Senparc.Core.Utility;
using Senparc.Core.Config;
using Senparc.Service;


namespace Senparc.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        //private IUserInfoService _userInfoService;
        public LoginController(/*IUserInfoService userInfoService*/)
        {
            //this._userInfoService = userInfoService;
        }

        public ActionResult Index(string returnUrl)
        {
            Login_IndexVD vd = new Login_IndexVD()
            {
                ReturnUrl = returnUrl,
                MessagerList = new List<Messager>(),
            };

            if (TempData["AuthorityNotReach"] != null && TempData["AuthorityNotReach"].ToString() == "TRUE")
            {
                vd.MessagerList.Add(new Messager(Core.Enums.MessageType.error, "您无权访问此页面或执行此操作！"));
                vd.AuthorityNotReach = true;
            }

            vd.IsLogined = this.HttpContext.User.Identity.IsAuthenticated;

            var tryLoginTimes = 0;
            if (Session["TryLoginTimes"] != null)
            {
                tryLoginTimes = (int)Session["TryLoginTimes"];
            }
            vd.ShowCheckCode = tryLoginTimes > SiteConfig.TryLoginTimes;

            return View(vd);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Index(Login_IndexVD vdForm)
        {
            //this.Validator(vdForm.Checkcode, "验证码", "Checkcode", null).ValidateCheckCode(CheckCodeKind.Login, true);
            //验证域名
            string errorMsg = null;

            IAdminUserInfoService adminUserInfoService = null;
            AdminUserInfo adminUserInfo = null;
            if (vdForm.UserName.IsNullOrEmpty())
            {
                errorMsg = "请填写账号！";
            }
            else if (vdForm.Password.IsNullOrEmpty())
            {
                errorMsg = "请填写密码！";
            }
            else
            {
                if (Session["TryLoginTimes"] != null)
                {
                    //TODO：验证码
                }

                if (ModelState.IsValid)
                {
                    adminUserInfoService = StructureMap.ObjectFactory.GetInstance<IAdminUserInfoService>();
                    adminUserInfo = adminUserInfoService.GetUserInfo(vdForm.UserName);
                    if (adminUserInfo == null)
                    {
                        errorMsg = "账号或密码错误！错误代码：101。";
                    }
                    //else if (userInfo.Locked)
                    //{
                    //    errorMsg = "账号已被锁定，无法登录。";
                    //}
                    else if (adminUserInfoService.TryLogin(vdForm.UserName, vdForm.Password, true) == null)
                    {
                        errorMsg = "账号或密码错误！错误代码：102。";
                    }
                }

            }

            if (!errorMsg.IsNullOrEmpty() || !ModelState.IsValid)
            {
                var tryLoginTimes = 0;
                if (Session["TryLoginTimes"] != null)
                {
                    tryLoginTimes = (int)Session["TryLoginTimes"];
                }

                vdForm.ShowCheckCode = tryLoginTimes >= SiteConfig.TryLoginTimes;

                Session["TryLoginTimes"] = tryLoginTimes + 1;

                vdForm.MessagerList = new List<Messager>();
                vdForm.MessagerList.Add(new Messager(Core.Enums.MessageType.error, errorMsg));
                return View(vdForm);
            }
            Session["TryLoginTimes"] = null;//清空登录次数

            LogUtility.AdminUserInfo.InfoFormat("用户登录成功：{0}", vdForm.UserName);
            Session["AdminLogin"] = "1";

            if (vdForm.ReturnUrl.IsNullOrEmpty())
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return Redirect(vdForm.ReturnUrl.UrlDecode());
            }
        }

        public ActionResult Logout()
        {
            IAdminUserInfoService adminUserInfoService = StructureMap.ObjectFactory.GetInstance<IAdminUserInfoService>();
            adminUserInfoService.Logout();
            return RedirectToAction("Index", "Home");
        }
    }
}
