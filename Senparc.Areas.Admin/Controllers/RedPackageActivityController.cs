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
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.ProjectOxford.Vision.Contract;
using Senparc.Areas.Admin.Models.VD;
using Senparc.Core.Enums;
using Senparc.Core.Extensions;
using Senparc.Core.Models;
using Senparc.Core.Models.WorkFlowModules;
using Senparc.Core.Utility;
using Senparc.Log;
using Senparc.Mvc;
using Senparc.Mvc.Filter;
using Senparc.Service;
using Senparc.AzureCognitiveService;
using Senparc.Core.Cache;
using ServiceStack;
using StructureMap;

namespace Senparc.Areas.Admin.Controllers
{
    [MenuFilter("RedPackageActivity")]
    public class RedPackageActivityController : BaseAdminController
    {
        private AccountService _accountService;
        private APP_RedPackage_ActivityService _appRedPackageActivityService;
        private APP_RedPackage_Activity_LogService _appRedPackageActivityLogService;

        public RedPackageActivityController(APP_RedPackage_ActivityService appRedPackageActivityService,
            APP_RedPackage_Activity_LogService appRedPackageActivityLogService, AccountService accountService)
        {
            _appRedPackageActivityService = appRedPackageActivityService;
            _appRedPackageActivityLogService = appRedPackageActivityLogService;
            _accountService = accountService;
        }

        public ActionResult Index(int pageIndex = 1)
        {
            int pageCount = 20;

            var redPackageActivityList = _appRedPackageActivityService.GetObjectList(pageIndex, pageCount, z => true,
                z => z.Id,
                OrderingType.Descending);

            var vd = new RedPackageActivity_IndexVD()
            {
                RedPackageActivityList = redPackageActivityList,
            };

            return View(vd);
        }

        public ActionResult Edit(int id = 0)
        {
            bool isEdit = id > 0;

            APP_RedPackage_Activity appRedPackageActivity = null;

            if (isEdit)
            {
                appRedPackageActivity = _appRedPackageActivityService.GetObject(z => z.Id == id);
                if (appRedPackageActivity == null)
                {
                    return RenderError("活动不存在！");
                }
            }
            else
            {
                appRedPackageActivity = new APP_RedPackage_Activity()
                {
                    BeginTime = DateTime.Now,
                    EndTime = DateTime.Now.AddDays(1)
                };
            }

            var vd = new RedPackageActivity_EditVD()
            {
                AppRedPackageActivity = appRedPackageActivity,
                ModuleTemplateList = new List<BaseModule>()
                {
                    new RegisterModule(),
                    new RegisterImageModule(),
                    new EmotionApiModule(),
                    new RedPackageResultModule(),
                    new GradeResultModule(),
                    new VisionApiModule(),
                    new PicContainerApiModule(),
                    new ContrastPicModule()
                },
                IsEdit = isEdit,
                //IsFirstRecharge =
                //    _orderService.GetCount(z => z.ActivityId == id && z.Status == (int) Order_Status.已支付) <= 0
            };

            return View(vd);
        }

        [HttpPost]
        public ActionResult Edit(
            [Bind(Prefix = "AppRedPackageActivity")] APP_RedPackage_Activity redPackageActivity_Form,
            HttpPostedFileBase[] picContainerApiFile)
        {
            picContainerApiFile = picContainerApiFile ?? new HttpPostedFileBase[0];
            bool isEdit = redPackageActivity_Form.Id > 0;

            this.Validator(redPackageActivity_Form.Name, "活动名称", "AppRedPackageActivity.Name", false);
            this.Validator(redPackageActivity_Form.BeginTime, "开始时间", "AppRedPackageActivity.BeginTime", false);
            this.Validator(redPackageActivity_Form.EndTime, "结束时间", "AppRedPackageActivity.EndTime", false)
                .IsFalse(z => DateTime.Compare(redPackageActivity_Form.BeginTime, z) > 0, "结束时间不能小于当前时间", true);

            //解析所有图片并且调用API
            var moduleList = redPackageActivity_Form.Rule.FromJson<List<BaseModule>>();
            var picContainerApiModuleList = moduleList.Where(z => z is PicContainerApiModule).ToList();
            int i = 0;
            //if (picContainerApiFile.Length > 0)
            foreach (var file in picContainerApiFile)
            {
                // HttpPostedFileBase hpf = file as HttpPostedFileBase;
                if (file == null || file.ContentLength == 0)
                {
                    i++;
                    continue;
                }
                string uploadResult = Upload.UpLoadProductPic(file);
                bool uploadSuccess = Upload.CheckUploadSuccessful(uploadResult);
                if (uploadSuccess)
                {
                    //调用计算机视觉API TODO:图片有可能会失败
                    var result = VisionApi.UploadAndGetTagsForImage(Server.MapPath(uploadResult));
                    ((PicContainerApiModule)picContainerApiModuleList[i]).Parameters = new VisionParameters()
                    {
                        Pic = uploadResult,
                        Result =
                            result != null
                                ? result.Tags.Select(z => new VisionTags(z.Confidence, z.Hint, z.Name)).ToList()
                                : new List<VisionTags>()
                    };
                }
                i++;
            }
            if (!ModelState.IsValid)
            {
                var vd = new RedPackageActivity_EditVD()
                {
                    AppRedPackageActivity = redPackageActivity_Form,
                    ModuleTemplateList = new List<BaseModule>()
                    {
                        new RegisterModule(),
                        new RegisterImageModule(),
                        new EmotionApiModule(),
                        new RedPackageResultModule(),
                        new GradeResultModule(),
                        new VisionApiModule(),
                        new PicContainerApiModule(),
                        new ContrastPicModule()
                    },
                };

                return View(vd);
            }

            APP_RedPackage_Activity appRedPackageActivity = null;

            if (isEdit)
            {
                appRedPackageActivity = _appRedPackageActivityService.GetObject(redPackageActivity_Form.Id);
                if (appRedPackageActivity == null)
                {
                    return RenderError("活动不存在！");
                }
            }
            else
            {
                appRedPackageActivity = new APP_RedPackage_Activity()
                {
                    AddTime = DateTime.Now,
                    TotalMoney = 0,
                };
            }
            TryUpdateModel(appRedPackageActivity, "AppRedPackageActivity", null,
                new[] { "Id", "TotalMoney", "RemainingMoney" });
            appRedPackageActivity.Rule = moduleList.SerializeToString();
            appRedPackageActivity.RemainingMoney += redPackageActivity_Form.RemainingMoney;
            appRedPackageActivity.TotalMoney += redPackageActivity_Form.RemainingMoney;
            _appRedPackageActivityService.SaveObject(appRedPackageActivity);
            SetMessager(MessageType.success, "保存成功。");
            //清除当前活动所有缓存
            var fullAccountWorkFlowCache = ObjectFactory.GetInstance<FullAccountWorkFlowCache>();
            fullAccountWorkFlowCache.RemoveActivityAllFullAccountWorkFlow(appRedPackageActivity.Id);
            return RedirectToAction("Edit", "RedPackageActivity", new { id = appRedPackageActivity.Id });
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var appRedPackageActivity = _appRedPackageActivityService.GetObject(id);
            if (appRedPackageActivity == null)
            {
                return RenderError("活动不存在！");
            }

            try
            {
                _appRedPackageActivityService.DeleteObject(appRedPackageActivity);
            }
            catch (Exception ex)
            {
                return RenderError(ex.Message);
            }

            SetMessager(MessageType.success, "删除成功");

            return RedirectToAction("Index");
        }


        public ActionResult Screen(int id)
        {
            var appRedPackageActivity = _appRedPackageActivityService.GetObject(z => z.Id == id);
            if (appRedPackageActivity == null)
            {
                return RenderError("活动不存在！");
            }
            var workFlowModule = appRedPackageActivity.Rule.FromJson<List<BaseModule>>();
            var module = workFlowModule.FirstOrDefault(z => z is BaseResultModule);
            var vd = new RedPackageActivity_ScreenVD()
            {
                AppRedPackageActivity = appRedPackageActivity,
                ResultModule = module as BaseResultModule
            };
            return View(vd);
        }

        public ActionResult GetActivityAwardLog(int id)
        {
            var activityAwardLogList =
                _appRedPackageActivityLogService.GetFullList(z => z.ActivityId == id, z => z.Money,
                    OrderingType.Descending);
            Func<APP_RedPackage_Activity_Award_Log, dynamic> getAwardLogInfo = (z) =>
            {
                dynamic inputPic = null;
                try
                {
                    //TODO:获取上传的第一张图片
                    var inputList = z.RegisterInfo.FromJson<Input>();
                    var input = inputList.FirstOrDefault(m => m.Type == ParameterSettingType.Pic);
                    if (input != null)
                    {
                        inputPic = input.Value;
                    }
                }
                catch (Exception e)
                {
                    return "";
                }
                var account = _accountService.GetObject(m => m.Id == z.AccountId);
                return new { z.Id, Money = z.Money.ToString("0.00"), z.AddTime, account.PicUrl, account.NickName, InputPic = inputPic };
            };
            return RenderJsonSuccessResult(true,
                new
                {
                    ActivityAwardLogList =
                        activityAwardLogList.Select(z => getAwardLogInfo(z))
                });
        }


        [HttpPost]
        public ActionResult UpLoadImage(HttpPostedFileBase file)
        {
            try
            {
                if (file == null)
                {
                    throw new NullReferenceException("上传图片错误！");
                }
                string uploadPicResult = null;
                uploadPicResult = Upload.UpLoadPic(file);
                var uploadImageSuccess = Upload.CheckUploadSuccessful(uploadPicResult);
                if (!uploadImageSuccess)
                {
                    throw new Exception("图片保存失败！");
                }

                //调用VisionApi 
                //var result = VisionApi.UploadAndGetTagsForImage(uploadPicResult);
                return RenderJsonSuccessResult(true, new { Url = uploadPicResult });
            }
            catch (Exception ex)
            {
                return RenderJsonSuccessResult(false, new { message = ex.Message });
            }
        }
    }
}