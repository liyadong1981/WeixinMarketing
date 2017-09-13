/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：WeixinOAuthController.cs

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
using Senparc.Core.Cache;
using Senparc.Core.Config;
using Senparc.Core.Enums;
using Senparc.Core.Extensions;
using Senparc.Core.Models;
using Senparc.Log;
using Senparc.Mvc.Controllers;
using Senparc.Mvc.Filter;
using Senparc.Service;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using StructureMap;
using System.Threading;
using Senparc.Weixin.Exceptions;

namespace Senparc.Areas.WX.Controllers
{
    public class WeixinOAuthController : BaseController
    {
        private IOAuthService _oAuthService;
        public WeixinOAuthController(IOAuthService oAuthService)
        {
            _oAuthService = oAuthService;
        }

        public ActionResult MpCallback(string callbackUrl, string code, string state)
        {
            if (code == null)
            {
                return Content("验证失败！");
            }

            if (state != "Azure")
            {
                return Content("请从合法途径进入！");
            }

            OAuthUserInfo userInfo = null;
            string openId = null;

            try
            {
                userInfo = _oAuthService.GetOAuthResult(SiteConfig.AppId, SiteConfig.AppSecret, code);

                openId = userInfo.openid;
            }
            catch (Exception ex)
            {
                LogUtility.OAuthLogger.InfoFormat("公众号OAuth异常，code：{0}，原因：{1}", code, ex.Message);

                return RenderError("公众号OAuth授权异常，原因{0}".With(ex.Message));
            }

            //处理Account
            var accountService = ObjectFactory.GetInstance<IAccountService>();
            var account =
                accountService.GetObject(z => z.WeixinOpenId == openId);
            if (account == null)
            {
                int shopId = 0;

                int.TryParse(HttpContext.Request.QueryString["shopId"], out shopId);

                try
                {
                    account = accountService.CreateAccountByUserInfo(userInfo, shopId);
                }
                catch (Exception ex)
                {
                    return RenderError(ex.Message);
                }
            }
            else if (userInfo!=null && account.NickName.IsNullOrEmpty() && account.PicUrl.IsNullOrEmpty())
            {
                accountService.UpdateAccountByUserInfo(userInfo, account);
            }

            Session["OpenId"] = userInfo.openid;
            //记录登录信息
            account.LastLoginTime = account.ThisLoginTime;
            account.LastLoginIP = account.ThisLoginIP;
            account.ThisLoginTime = DateTime.Now;
            account.ThisLoginIP = Request.UserHostName;
            accountService.SaveObject(account); //保存Account信息，同时会清除FullAccount信息，顺便保证“强制退出”等参数失效。

            return Redirect(callbackUrl);
        }

        public ActionResult Logout()
        {
            Session.Abandon();
            Session.RemoveAll();
            Session.Remove("OpenId");
            return Content("退出成功");
        }
    }
}
