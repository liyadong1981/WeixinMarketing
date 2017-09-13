/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：HomeController.cs

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
using Senparc.Core.Models.WorkFlowModules;
using Senparc.Mvc.Filter;
using Senparc.Service;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Helpers;
using Senparc.AzureCognitiveService;
using Senparc.Core.Cache;
using Senparc.Log;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using ServiceStack;
using StructureMap;

namespace Senparc.Areas.WX.Controllers
{
    public class HomeController : BaseWXController
    {
        private WorkFlowModuleService _workFlowModuleService;
        private APP_RedPackage_ActivityService _appRedPackageActivityService;

        public HomeController(WorkFlowModuleService workFlowModuleService, APP_RedPackage_ActivityService appRedPackageActivityService)
        {
            _workFlowModuleService = workFlowModuleService;
            _appRedPackageActivityService = appRedPackageActivityService;
        }

        #region MyRegion

        /// <summary>
        /// 下载图片素材
        /// </summary>
        /// <param name="serverId"></param>
        /// <param name="fileName"></param>
        /// <param name="getNewToken"></param>
        /// <returns></returns>
        //private bool DownloadTemplateImage(string serverId, string fileName, bool getNewToken = false)
        //{
        //    var accessToken = AccessTokenContainer.TryGetAccessToken(SiteConfig.APPID, SiteConfig.APPSECRET, getNewToken);
        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        MediaApi.Get(accessToken, serverId, ms);
        //        //保存到文件
        //        ms.Position = 0;
        //        byte[] buffer = new byte[1024];
        //        int bytesRead = 0;
        //        //判断是否上传成功
        //        byte[] topBuffer = new byte[1];
        //        ms.Read(topBuffer, 0, 1);
        //        if (topBuffer[0] == '{')
        //        {
        //            //写入日志
        //            ms.Position = 0;
        //            byte[] logBuffer = new byte[1024];
        //            ms.Read(logBuffer, 0, logBuffer.Length);
        //            string str = System.Text.Encoding.Default.GetString(logBuffer);
        //            Senparc.Log.LogUtility.Weixin.InfoFormat("下载失败：{0}", str);
        //            return false;
        //        }
        //        else
        //        {
        //            ms.Position = 0;
        //            using (FileStream fs = new FileStream(fileName, FileMode.Create))
        //            {
        //                while ((bytesRead = ms.Read(buffer, 0, buffer.Length)) != 0)
        //                {
        //                    fs.Write(buffer, 0, bytesRead);
        //                }
        //                fs.Flush();
        //            }
        //        }
        //    }
        //    return true;
        //}
        /// <summary>
        /// jssdk下载商品评论图片方法
        /// </summary>
        /// <param name="serverId"></param>
        /// <returns></returns>
        //[HttpPost]
        //public ActionResult DownloadProductCommentImg(string serverId)
        //{
        //    //TODO 上线部署需在upload文件夹里添加ProductComment文件夹
        //    string name = "/upload/ProductComment/{0}.jpg".With(DateTime.Now.Ticks + Guid.NewGuid().ToString("n").Substring(0, 8));
        //    var fileName = string.Format(Server.MapPath("~" + name));
        //    if (!DownloadTemplateImage(serverId, fileName))
        //    {
        //        if (!DownloadTemplateImage(serverId, fileName, true))
        //        {
        //            return RenderJsonSuccessResult(false, new { Message = "下载错误" });
        //        }
        //    }

        //    return RenderJsonSuccessResult(true, new { Message = name });
        //} 

        #endregion

        /// <summary>
        /// JSSDK参数获取
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public ActionResult GetSystemConfig(string url)
        {
            //获取时间戳
            var timestamp = JSSDKHelper.GetTimestamp();
            //获取随机码
            string nonceStr = JSSDKHelper.GetNoncestr();
            string ticket = JsApiTicketContainer.TryGetJsApiTicket(SiteConfig.AppId, SiteConfig.AppSecret);

            //获取签名
            string signature = JSSDKHelper.GetSignature(ticket, nonceStr, timestamp, url);
            return RenderJsonSuccessResult(true, new
            {
                appId = SiteConfig.AppId,
                timestamp,
                nonceStr,
                signature,
            }, true);
        }

        [MenuFilter("Home")]
        public ActionResult Index(int? id)
        {
            if (!id.HasValue)
            {
                return RenderError("活动不存在！");
            }
            var activity = _appRedPackageActivityService.GetObject(z => z.Id == id);
            if (activity == null)
            {
                return RenderError("活动不存在！");
            }
            if (DateTime.Compare(activity.BeginTime, DateTime.Now) > 0)
            {
                return RenderError("活动还没开始呢 亲！");
            }
            if (DateTime.Compare(activity.EndTime, DateTime.Now) < 0)
            {
                return RenderError("活动已经结束呢 亲！");
            }
            var vd = new Home_IndexVD()
            {
                ActivityId = activity.Id
            };
            return View(vd);
        }

        [HttpPost]
        public ActionResult Shaked(int id)
        {
            try
            {
                //创建用户对应工作流
                var fullAccountWorkFlowCache = ObjectFactory.GetInstance<FullAccountWorkFlowCache>();
                var activity = _appRedPackageActivityService.GetObject(z => z.Id == id);
                if (activity == null)
                {
                    return RenderJsonSuccessResult(false, new { Message = "还没有活动呢 亲！" });
                }
                fullAccountWorkFlowCache.CreateFullAccountWorkFlow(activity, FullAccount.Id);
                var actionValue = _workFlowModuleService.SubmitStep(-1, 0, new Input(), id, FullAccount.Id, null);
                return RenderJsonSuccessResult(true, actionValue);
            }
            catch (Exception ex)
            {
                return RenderJsonSuccessResult(false, new { Message = "活动出问题了 哭！" });
            }
        }

        [HttpPost]
        public Task<ActionResult> SubmitRegister(int currentStep, int nextStep, int activityId, string inputJson)
        {
            //TODO:验证正确性

            return Task<ActionResult>.Factory.StartNew(() =>
            {

                try
                {
                    //var input = Newtonsoft.Json.JsonConvert.DeserializeObject<Input>(inputJson);
                    var input = inputJson.FromJson<Input>();
                    var actionValue = _workFlowModuleService.SubmitStep(currentStep, nextStep, input, activityId,
                        FullAccount.Id, HttpContext.Request.UserHostAddress);
                    return RenderJsonSuccessResult(true, actionValue);
                }
                catch (Exception ex)
                {
                    LogUtility.DebugLogger.Debug(ex.Message);
                    return RenderJsonSuccessResult(false, new { Message = "您上传的图片没有监测到面部表情！" });
                }
            });
        }

        //public ActionResult GetEmotion()
        //{
        //    var imgPath = "~/Upload/Emotion/happy.jpg";
        //    var emotion = EmotionApi.UploadAndStreamDetectEmotions(Server.MapPath(imgPath));
        //    return Content(emotion.ToJson());
        //}
        [HttpPost]
        public ActionResult GetUserInput(int id)
        {
            try
            {
                var fullAccountWorkFlowCache = ObjectFactory.GetInstance<FullAccountWorkFlowCache>();
                var fullAccountWorkFlow = fullAccountWorkFlowCache.GetFullAccountWorkFlow(id, FullAccount.Id); //用户工作流对应的
                var emotionInputs = new List<object>();
                foreach (var module in fullAccountWorkFlow.BaseModuleList)
                {
                    var inputs = module.Input.Where(z => z is NumberParameterSetting).Select(z => z.Value);
                    emotionInputs.AddRange(inputs);
                }
                return RenderJsonSuccessResult(true, new { Emotions = emotionInputs.ToJson() });
            }
            catch (Exception ex)
            {
                //TODO:非法请求
                return RenderJsonSuccessResult(false, new { message = "非法请求！" });
            }
        }
    }
}