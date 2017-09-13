/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：AccountController.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Senparc.Areas.Admin.Models.VD;
using Senparc.Core.Cache;
using Senparc.Core.Config;
using Senparc.Core.Enums;
using Senparc.Core.Extensions;
using Senparc.Core.Models;
using Senparc.Log;
using Senparc.Mvc;
using Senparc.Mvc.Filter;
using Senparc.Service;
using StructureMap;

namespace Senparc.Areas.Admin.Controllers
{
    [MenuFilter("Account")]
    public class AccountController : BaseAdminController
    {
        private IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public ActionResult Index(int pageIndex = 1)
        {
            int pageCount = 20;

            var accountList = _accountService.GetObjectList(pageIndex, pageCount, z => true, z => z.Id,
                OrderingType.Descending);

            var vd = new Account_IndexVD()
            {
                AccountList = accountList,
            };

            return View(vd);
        }

        public ActionResult Edit(int id = 0)
        {
            bool isEdit = id > 0;

            Account account = null;

            if (isEdit)
            {
                account = _accountService.GetObject(z => z.Id == id);
                if (account == null)
                {
                    return RenderError("商品不存在！");
                }
            }
            else
            {
                return RenderError("用户数据不能新增！");
            }

            Account_EditVD vd = new Account_EditVD()
            {
                Account = account,
            };

            return View(vd);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Prefix = "Account")] Account account_Form, string newPassWord)
        {
            bool isEdit = account_Form.Id > 0;

            if (!ModelState.IsValid)
            {
                Account_EditVD vd = new Account_EditVD()
                {
                    Account = account_Form,
                };

                return View(vd);
            }

            Account account = null;

            if (isEdit)
            {
                account = _accountService.GetObject(account_Form.Id);
                if (account == null)
                {
                    return RenderError("商品目录不存在！");
                }
            }
            else
            {
                return RenderError("用户数据不能新增！");
            }

            TryUpdateModel(account, "Account", null, new[] { "Id" });

            if (!newPassWord.IsNullOrEmpty())
            {
                account.Password = _accountService.GetPassword(newPassWord, account.PasswordSalt, false);
            }

            _accountService.SaveObject(account);

            SetMessager(MessageType.success, "保存成功");

            return RedirectToAction("Edit", "Account", new { id = account.Id });
        }

        /// <summary>
        /// 强制登陆某用户账户
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ActionResult ForceLogin(string userName)
        {
            var account = _accountService.GetObject(z => z.UserName == userName && z.WeixinOpenId!=null);
            Session["OpenId"] = account.WeixinOpenId;
            return RedirectToAction("Index", "Account", new { Area = "WX" });
        }
    }
}
