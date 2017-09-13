/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：SystemConfigController.cs

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
using System.Threading.Tasks;
using System.Web.Mvc;
using Senparc.Areas.Admin.Models.VD;
using Senparc.Core.Enums;
using Senparc.Core.Models;
using Senparc.Mvc;
using Senparc.Mvc.Filter;
using Senparc.Service;

namespace Senparc.Areas.Admin.Controllers
{
    [MenuFilter("SystemConfig.Index")]
    public class SystemConfigController : BaseAdminController
    {
        private ISystemConfigService _systemConfigService;
        public SystemConfigController(ISystemConfigService systemConfigService)
        {
            _systemConfigService = systemConfigService;
        }

        public ActionResult Index()
        {
            var vd = new SystemConfig_IndexVD()
            {
                SystemConfig = _systemConfigService.GetObject(z => true),
            };
            return View(vd);
        }


        [HttpPost]
        public ActionResult Index([Bind(Prefix = "SystemConfig")] SystemConfig systemConfig_Form)
        {
            this.Validator(systemConfig_Form.SystemName, "系统名称", "SystemConfig.SystemName", false);

            if (!ModelState.IsValid)
            {
                var vd = new SystemConfig_IndexVD()
                {
                    SystemConfig = systemConfig_Form,
                };
                return View(vd);
            }

            var systemConfig = _systemConfigService.GetObject(systemConfig_Form.Id);

            if (systemConfig == null)
            {
                return RenderError("信息不存在！");
            }

            this.TryUpdateModel(systemConfig, "SystemConfig", null, new[] { "Id" });
            this._systemConfigService.SaveObject(systemConfig);

            base.SetMessager(MessageType.success, "修改成功！");
            return RedirectToAction("Index");
        }
    }
}
