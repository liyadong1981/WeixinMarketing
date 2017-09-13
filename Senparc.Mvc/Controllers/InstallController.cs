/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：InstallController.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Web.Mvc;
using Senparc.Core.Models;
using Senparc.Service;
using StructureMap;

namespace Senparc.Mvc.Controllers
{
    public class InstallController : Controller
    {
        private ISystemConfigService _systemConfigService;
        private IAdminUserInfoService _adminUserInfoService;
        public InstallController(ISystemConfigService systemConfigService, IAdminUserInfoService adminUserInfoService)
        {
            _adminUserInfoService = adminUserInfoService;
            _systemConfigService = systemConfigService;
        }

        public ActionResult Index()
        {
            var systemConfig = _systemConfigService.GetObject(z => true);
            if (systemConfig != null)
            {
                return Content("已经进行过初始化！");
            }

            var salt = DateTime.Now.Ticks.ToString();
            var password = "123123";

            var adminUserInfo = new AdminUserInfo()
            {
                UserName = "TNT2",
                PasswordSalt = salt,
                Password = _adminUserInfoService.GetPassword(password, salt, false),
                RealName = "",
                LastLoginTime = DateTime.Now,
                ThisLoginTime = DateTime.Now,
                LastLoginIP = "",
                ThisLoginIP = "",
                Phone = "",
                AddTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };

            _adminUserInfoService.SaveObject(adminUserInfo);

            systemConfig = new SystemConfig()
            {
                Id = 1,
                SystemName = "AzureDemo",
            };
            _systemConfigService.SaveObject(systemConfig);

            return Content("OK");
        }

        public ActionResult TestAppSet()
        {
            var systemConfigService = ObjectFactory.GetInstance<ISystemConfigService>();

            var systemConfig = systemConfigService.GetObject(z => true);

            systemConfigService.SaveObject(systemConfig);

            return Content("OK");
        }
    }
}
