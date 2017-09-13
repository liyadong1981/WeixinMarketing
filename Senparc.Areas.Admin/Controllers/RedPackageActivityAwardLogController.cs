/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：RedPackageActivityAwardLogController.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Web.Mvc;
using Senparc.Areas.Admin.Models.VD;
using Senparc.Core.Enums;
using Senparc.Core.Extensions;
using Senparc.Core.Models;
using Senparc.Mvc.Filter;
using Senparc.Service;

namespace Senparc.Areas.Admin.Controllers
{
    [MenuFilter("AwardLog")]
    public class RedPackageActivityAwardLogController : BaseAdminController
    {
        private APP_RedPackage_Activity_LogService _appRedPackageActivityAwardLogService;

        public RedPackageActivityAwardLogController(
            APP_RedPackage_Activity_LogService appRedPackageActivityAwardLogService)
        {
            _appRedPackageActivityAwardLogService = appRedPackageActivityAwardLogService;
        }

        public ActionResult Index(int pageIndex = 1)
        {
            int pageCount = 20;

            var redPackageActivityAwardLogList = _appRedPackageActivityAwardLogService.GetObjectList(pageIndex,
                pageCount, z => true, z => z.Id,
                OrderingType.Descending);//, new string[] {"Account", "APP_RedPackage_Activity", "APP_RedPackage_Activity_Award" }

            var vd = new RedPackageActivityAwardLog_IndexVD()
            {
                RedPackageActivityAwardLogList = redPackageActivityAwardLogList,
            };

            return View(vd);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var appRedPackageActivityAwardLog = _appRedPackageActivityAwardLogService.GetObject(id);
            if (appRedPackageActivityAwardLog == null)
            {
                return RenderError("奖项记录不存在！");
            }

            try
            {
                _appRedPackageActivityAwardLogService.DeleteObject(appRedPackageActivityAwardLog);
            }
            catch (Exception ex)
            {
                return RenderError(ex.Message);
            }

            SetMessager(MessageType.success, "删除成功");

            return RedirectToAction("Index");
        }
    }
}