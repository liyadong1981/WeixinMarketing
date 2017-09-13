/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：RedPackageActivityController.cs

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
using System.Threading.Tasks;
using System.Web.Mvc;
using Senparc.Areas.WX.Models.VD;
using Senparc.Core.Config;
using Senparc.Core.Enums;
using Senparc.Core.Extensions;
using Senparc.Core.Models.WorkFlowModules;
using Senparc.Log;
using Senparc.Service;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using ServiceStack;

namespace Senparc.Areas.WX.Controllers
{
    public class RedPackageActivityController : BaseWXController
    {
        private APP_RedPackage_Activity_LogService _appRedPackageActivityLogService;
        private APP_RedPackage_ActivityService _appRedPackageActivityService;

        public RedPackageActivityController(
            APP_RedPackage_Activity_LogService appRedPackageActivityLogService,
            APP_RedPackage_ActivityService appRedPackageActivityService)
        {
            _appRedPackageActivityLogService = appRedPackageActivityLogService;
            _appRedPackageActivityService = appRedPackageActivityService;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetRedPacket(int id)
        {

            var appRedPackageActivity = _appRedPackageActivityService.GetObject(z => z.Id == id);
            if (appRedPackageActivity == null)
            {
                return RenderError("活动不存在！");
            }
            var appRedPackageActivityAwardsLog =
                _appRedPackageActivityLogService.GetObject(
                    z =>
                        z.AccountId == FullAccount.Id && z.ActivityId == id);

            if (appRedPackageActivityAwardsLog == null)
            {
                return RenderError("您未中奖！！");
            }

            var workFlowModule = appRedPackageActivity.Rule.FromJson<List<BaseModule>>();
            var module = workFlowModule.FirstOrDefault(z => z is BaseResultModule);
            if (module == null)
            {
                return RenderError("活动出问题了！");
            }
            var inputList = appRedPackageActivityAwardsLog.RegisterInfo.FromJson<List<BaseParameterSetting>>();
            var parameter = inputList.FirstOrDefault(z => z is NumberParameterSetting && z.Name == module.Input[0].Value.ToString());
            string name = "";
            double value = 0;
            if (parameter != null)
            {
                value = Convert.ToDouble(parameter.Value) * 100;
            }

            var vd = new RedPackageActivity_ItemVD()
            {
                AppRedPackageActivityAwardLog = appRedPackageActivityAwardsLog,
                ParameteName = "相似",
                ParameteValue = value,
                ResultModule = (BaseResultModule)module
            };
            return View(vd);
        }




        public async Task<ActionResult> SaveRedPackageRegisterImg(string serverId)
        {
            try
            {
                //下载图片
                var fileName = DateTime.Now.Ticks + Guid.NewGuid().ToString("n").Substring(0, 8);
                var path = "/Upload/Temp/Red/{0}".With(DateTime.Now.ToString("yyyy-MM-dd"));

                //创建目录
                if (!System.IO.Directory.Exists(Server.MapPath(path)))
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath(path));
                }

                string name = "{0}/{1}.jpg".With(path, fileName);
                var filePath = string.Format(Server.MapPath("~" + name));

                var downloadTemplateImage = await DownloadTemplateImageAsync(serverId, filePath);

                if (!downloadTemplateImage)
                {
                    downloadTemplateImage = await DownloadTemplateImageAsync(serverId, filePath, true);
                }

                if (!downloadTemplateImage)
                {
                    throw new Exception("图片上传错误（01）！");
                }
                return RenderJsonSuccessResult(true, new { Message = "图片上传成功！", Url = name });
            }
            catch (Exception ex)
            {
                LogUtility.Weixin.Error("发送图片失败 ServerId={0}".With(serverId), ex);
                return RenderError(ex.Message);
            }
        }

        /// <summary>
        /// 获取活动中奖纪录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult RedPacketLog()
        {
            var vd = new RedPackageActivity_RedPacketLogVD()
            {
                AppRedPackageActivityAwardLog =
                    _appRedPackageActivityLogService.GetFullList(z => z.AccountId == FullAccount.Id, z => z.AddTime,
                        OrderingType.Descending),
            };
            return View(vd);
        }



        /// <summary>
        /// 下载微信图片
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="fileName"></param>
        /// <param name="getNewToken"></param>
        /// <returns></returns>
        [NonAction]
        public async Task<bool> DownloadTemplateImageAsync(string serverId, string fileName, bool getNewToken = false)
        {
            var accessToken = AccessTokenContainer.TryGetAccessToken(SiteConfig.AppId, SiteConfig.AppSecret,
                getNewToken);
            using (MemoryStream ms = new MemoryStream())
            {
                await MediaApi.GetAsync(accessToken, serverId, ms);
                //保存到文件
                ms.Position = 0;
                byte[] buffer = new byte[1024];
                int bytesRead = 0;
                //判断是否上传成功
                byte[] topBuffer = new byte[1];
                await ms.ReadAsync(topBuffer, 0, 1);
                if (topBuffer[0] == '{')
                {
                    //写入日志
                    ms.Position = 0;
                    byte[] logBuffer = new byte[1024];
                    await ms.ReadAsync(logBuffer, 0, logBuffer.Length);
                    string str = System.Text.Encoding.Default.GetString(logBuffer);
                    Senparc.Log.LogUtility.Weixin.InfoFormat("下载失败：{0}。serverId：{1}", str, serverId);
                    return false;
                }
                else
                {
                    ms.Position = 0;
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        while ((bytesRead = await ms.ReadAsync(buffer, 0, buffer.Length)) != 0)
                        {
                            await fs.WriteAsync(buffer, 0, bytesRead);
                        }
                        await fs.FlushAsync();
                    }
                }
            }
            return true;
        }
    }
}