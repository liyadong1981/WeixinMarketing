/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：AdminUserInfoController.cs

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
using Senparc.Areas.Admin.Controllers;
using Senparc.Areas.Admin.Models.VD;
using Senparc.Core.Enums;
using Senparc.Core.Extensions;
using Senparc.Core.Models;
using Senparc.Mvc.Filter;
using Senparc.Service;
using Senparc.Mvc;

namespace Senparc.Areas.Admin.Controllers
{
    [MenuFilter("AdminUserInfo.Index")]
    public class AdminUserInfoController : BaseAdminController
    {
        private IAdminUserInfoService _adminUserInfoService;
        public AdminUserInfoController(IAdminUserInfoService adminUserInfoService)
        {
            this._adminUserInfoService = adminUserInfoService;
        }

        public ActionResult Index(int pageIndex = 1)
        {
            int pageSize = 20;

            AdminUserInfo_IndexVD vd = new AdminUserInfo_IndexVD()
            {
                AdminUserInfoList = this._adminUserInfoService.GetObjectList(pageIndex, pageSize, z => true, z => z.Id, OrderingType.Descending)
            };
            return View(vd);
        }

        public ActionResult Edit(int id = 0)
        {
            bool isEdit = id > 0;

            if (!isEdit)
            {

            }

            AdminUserInfo userInfo = null;
            if (isEdit)
            {
                userInfo = _adminUserInfoService.GetAdminUserInfo(id);
                if (userInfo == null)
                {
                    return RenderError("信息不存在！");
                }
            }
            else
            {
                userInfo = new AdminUserInfo();
            }
            AdminUserInfo_EditVD vd = new AdminUserInfo_EditVD()
            {
                IsEdit = isEdit,
                AdminUserInfo = userInfo,
            };
            return View(vd);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Prefix = "AdminUserInfo")]AdminUserInfo userInfo_Form, string password)
        {
            bool isEdit = userInfo_Form.Id > 0;

            this.Validator(userInfo_Form.UserName, "用户名", "AdminUserInfo.UserName", false)
                .IsFalse(z => this._adminUserInfoService.CheckUserNameExisted(userInfo_Form.Id, z), "用户名已存在！", true);
            this.Validator(userInfo_Form.RealName, "真实姓名", "AdminUserInfo.RealName", false);
            this.Validator(userInfo_Form.Phone, "电话", "AdminUserInfo.Phone", false);

            if (!isEdit || !password.IsNullOrEmpty())
            {
                this.Validator(password, "密码", "Password", false).MinLength(6);
            }

            if (!ModelState.IsValid)
            {
                AdminUserInfo_EditVD vd = new AdminUserInfo_EditVD()
                            {
                                IsEdit = isEdit,
                                AdminUserInfo = userInfo_Form
                            };
                return View(vd);
            }

            AdminUserInfo userInfo = null;
            if (isEdit)
            {
                userInfo = _adminUserInfoService.GetAdminUserInfo(userInfo_Form.Id);
                if (userInfo == null)
                {
                    return RenderError("信息不存在！");
                }
            }
            else
            {
                var passwordSalt = DateTime.Now.Ticks.ToString();
                userInfo = new AdminUserInfo()
                               {
                                   PasswordSalt = passwordSalt,
                                   LastLoginTime = DateTime.Now,
                                   ThisLoginTime = DateTime.Now,
                                   AddTime = DateTime.Now,
                                   UpdateTime = DateTime.Now
                               };
            }

            if (!password.IsNullOrEmpty())
            {
                userInfo.Password = this._adminUserInfoService.GetPassword(password, userInfo.PasswordSalt, false);//生成密码
            }

            this.TryUpdateModel(userInfo, "AdminUserInfo", null, new[] { "Id" });
            this._adminUserInfoService.SaveObject(userInfo);

            base.SetMessager(MessageType.success, "{0}成功！".With(isEdit ? "修改" : "新增"));
            return RedirectToAction("Edit", new { id = userInfo.Id });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var userInfo = _adminUserInfoService.GetAdminUserInfo(id);
            if (userInfo == null)
            {
                return RenderError("信息不存在！");
            }
            this._adminUserInfoService.DeleteObject(userInfo);

            SetMessager(MessageType.success, "删除成功！");

            return RedirectToAction("Index");
        }
    }
}
