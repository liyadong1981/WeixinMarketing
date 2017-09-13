/*----------------------------------------------------------------
    Copyright (C) 2017 Senparc

    文件名：WorkFlowModuleService.cs

    创建标识：Senparc - 20170724

    注意：此项目是《微信开发深度解析：微信公众号、小程序高效开发秘籍》图书中第5章的WeixinMarketing项目源代码。
    本项目只包含了运行案例所必须的学习代码，以及精简的部分SenparcCore框架代码，不确保其他方面的稳定性、安全性，
    因此，请勿直接用于商业项目，例如安全性、缓存等需要根据具体情况进行调试。

    盛派网络保留所有权利。

----------------------------------------------------------------*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.ProjectOxford.Emotion.Contract;
using Senparc.Core.Extensions;
using Senparc.Core.Models.WorkFlowModules;
using Senparc.Core.Utility;
using Senparc.Repository;
using StructureMap;
using ServiceStack;
using System.Transactions;
using Senparc.AzureCognitiveService;
using Senparc.Core.Cache;
using Senparc.Core.Models;
using Senparc.Log;

namespace Senparc.Service
{
    public class WorkFlowModuleService : BaseServiceData
    {
        /// <summary>
        /// 核对账户金额锁
        /// </summary>
        public static object CheckActivityTotalMoneyLock = new object();

        public WorkFlowModuleService(ISystemConfigRepository systemConfigRepo)
            : base(systemConfigRepo)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentStep"></param>
        /// <param name="nextStep"></param>
        /// <param name="inputList"></param>
        /// <param name="activityId"></param>
        /// <param name="accountId"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public ActionValue SubmitStep(int currentStep, int nextStep, Input inputList, int activityId, int accountId,
            string ip)
        {
            var fullAccountWorkFlowCache = ObjectFactory.GetInstance<FullAccountWorkFlowCache>();
            var fullAccountWorkFlow = fullAccountWorkFlowCache.GetFullAccountWorkFlow(activityId, accountId); //用户工作流对应的
            if (fullAccountWorkFlow == null)
            {
                throw new Exception("请扫描二维码参与活动！");
            }
            //var userWorkFlowList = AccountWorkFlowDictionary[accountWorkFlowKey];

            if (nextStep > fullAccountWorkFlow.BaseModuleList.Count)
            {
                throw new Exception("没有定义过程节点！");
            }

            var currentModule = currentStep >= 0 ? fullAccountWorkFlow.BaseModuleList[currentStep] : null;
            var nextModule = fullAccountWorkFlow.BaseModuleList[nextStep];
            //TODO：没找到
            if (nextModule == null)
            {
                throw new Exception("没有定义下一个过程节点！");
            }
            if (inputList.Count > 0)
            {
                foreach (var parameterValue in inputList)
                {
                    parameterValue.Name = GetParaeterKey(currentModule.Name, parameterValue.Name); //根据前端格式 [ > ]
                    var parameter = fullAccountWorkFlow.Input.FirstOrDefault(z => z.Name == parameterValue.Name);
                    if (parameter == null)
                    {
                        fullAccountWorkFlow.Input.Add(parameterValue);
                    }
                    else
                    {
                        parameter.Value = parameterValue.Value;
                    }
                }
                fullAccountWorkFlowCache.UpdateFullAccountWorkFlowToCache(fullAccountWorkFlow);
            }
            //开始处理
            ActionValue actionValue;
            if (nextModule is RegisterModule)
            {
                var strongModule = nextModule as RegisterModule;
                actionValue = new ActionValue(ActionValueType.OpenRegister.ToString(), strongModule.Output.ToJson(),
                    currentStep + 1);
            }
            else if (nextModule is RegisterImageModule)
            {
                var strongModule = nextModule as RegisterImageModule;
                actionValue = new ActionValue(ActionValueType.OpenRegister.ToString(), strongModule.Output.ToJson(),
                    currentStep + 1);
            }
            else if (nextModule is PicContainerApiModule) //图片容器
            {
                var strongModule = nextModule as PicContainerApiModule;
                if (strongModule.Parameters == null)
                {
                    throw new Exception("输入参数节点有误{0}！".With(nextModule.Step));
                }
                actionValue = new ActionValue(ActionValueType.ShowPic.ToString(), strongModule.Parameters.Pic,
                    currentStep + 1);
            }
            else if (nextModule is ContrastPicModule) //比对结果
            {
                var strongModule = nextModule as ContrastPicModule;
                if (strongModule.Input.Count < 1)
                {
                    throw new Exception("输入参数节点有误{0}！".With(nextModule.Step));
                }
                //做对比
                var inputValue = strongModule.Input[0].Value.ToString();
                //图片容器中的输出参数
                var picContainerApiModule =
                    fullAccountWorkFlow.BaseModuleList.FirstOrDefault(
                        z => z is PicContainerApiModule && inputValue.Contains(z.Name));
                //计算机视觉中的输出参数
                var visionInput =
                    fullAccountWorkFlow.Input.FirstOrDefault(z => z.Name == strongModule.Input[1].Value.ToString());
                decimal scale = 0M;
                if (visionInput is VisionApiParameterSetting && picContainerApiModule != null)
                {
                    var visionParameters = visionInput.Value as VisionParameters;
                    if (visionParameters == null)
                    {
                        throw new Exception("图片识别错误！");
                    }
                    var parameters = ((PicContainerApiModule)picContainerApiModule).Parameters;
                    var matchNumber = 0;
                    matchNumber +=
                        visionParameters.Result.Count(visionTag => parameters.Result.All(z => z.Name == visionTag.Name));
                    scale = visionParameters.Result.Count > 0 ? (decimal)matchNumber / visionParameters.Result.Count : 0;
                }
                //TODO:计算出输出结果
                actionValue = SubmitStep(nextStep, nextStep + 1,
                    new Input() { new NumberParameterSetting("比对结果", Math.Round(scale, 2)) }, activityId, accountId, ip);
            }
            else if (nextModule is VisionApiModule) //计算机是视觉API
            {
                var strongModule = nextModule as VisionApiModule;
                var inpuValue = strongModule.Input[0].Value.ToString();
                var paraeter = fullAccountWorkFlow.Input.FirstOrDefault(z => z.Name == inpuValue);
                if (paraeter == null)
                {
                    LogUtility.WorkFlowModule.Error("VisionApiModule 获取输入参数失败");
                    throw new Exception("输入参数节点有误{0}！".With(nextModule.Step));
                }
                //调用api
                var picUrl = paraeter.Value.ToString();
                var currentInput = GetVisionInput(picUrl, strongModule.Output[0].Name);
                actionValue = SubmitStep(nextStep, nextStep + 1, currentInput, activityId, accountId, ip);
            }
            else if (nextModule is EmotionApiModule)
            {
                var strongModule = nextModule as EmotionApiModule;

                //TODO:查找参数
                var picValue = strongModule.Input[0].Value as string;
                //TODO:没找到
                if (picValue == null || fullAccountWorkFlow.Input.All(z => z.Name != picValue))
                {
                    throw new Exception("输入参数节点有误{0}！".With(nextModule.Step));
                }

                //调用微软Azure 情绪感知接口
                var currentInput = GetMoodsApiModuleInputAsync(fullAccountWorkFlow, picValue, nextModule).Result;//TODO:后期可以改用异步方法
                actionValue = SubmitStep(nextStep, nextStep + 1, currentInput, activityId, accountId, ip);
            }
            else if (nextModule is RedPackageResultModule) //红包结果
            {
                var strongModule = nextModule as RedPackageResultModule;
                //最大红包金额
                //最小红包金额
                var valueKey = strongModule.Input[0].Value as string;
                var maxRedPackageMoney = Convert.ToDecimal(strongModule.Input[1].Value);
                var minRedPackageMoney = Convert.ToDecimal(strongModule.Input[2].Value);

                var parameter = fullAccountWorkFlow.Input.FirstOrDefault(z => z.Name == valueKey);
                if (!(parameter is NumberParameterSetting))
                {
                    throw new Exception("输入参数节点有误{0}！".With(nextModule.Step));
                }
                var value = 0M;
                Decimal.TryParse(parameter.Value.ToString(), out value);

                var money = value * maxRedPackageMoney;
                var totalMoney = money > minRedPackageMoney ? money : minRedPackageMoney;
                SendActivityReward(activityId, accountId, totalMoney, nextModule, fullAccountWorkFlow.Input, ip);
                var resultData =
                    new { Money = totalMoney, ActivityId = activityId, Name = parameter.Name, Value = value * 100 };
                actionValue = new ActionValue(ActionValueType.OpenResult.ToString(), resultData.ToJson(),
                    strongModule.Step + 1);
                //TODO:清除用户该活动缓存
            }
            else if (nextModule is GradeResultModule) //评分结果
            {
                var strongModule = nextModule as GradeResultModule;
                var valueKey = strongModule.Input[0].Value as string;
                var parameter = fullAccountWorkFlow.Input.FirstOrDefault(z => z.Name == valueKey);
                if (!(parameter is NumberParameterSetting))
                {
                    throw new Exception("输入参数节点有误{0}！".With(nextModule.Step));
                }


                var gradeScore = 0M;
                decimal.TryParse(parameter.Value.ToString(), out gradeScore);
                var money = Math.Round(gradeScore, 4) * 100;//评分百分比

                //保存记录
                var appRedPackageActivityAwardLogService =
                    ObjectFactory.GetInstance<APP_RedPackage_Activity_LogService>();
                appRedPackageActivityAwardLogService.CreateAppRedPackageActivityAwardLog(activityId,
                    accountId, money, "评分",
                    fullAccountWorkFlow.Input.SerializeToString());

                //返回前端数据
                var resultData =
                    new
                    {
                        GradeScore = gradeScore,
                        ActivityId = activityId,
                        Name = parameter.Name,
                        Value = gradeScore * 100
                    };
                actionValue = new ActionValue(ActionValueType.OpenResult.ToString(), resultData.ToJson(),
                    strongModule.Step + 1);
            }
            else
            {
                throw new Exception("步骤错误！");
            }
            return actionValue;
        }

        /// <summary>
        /// 发送红包活动奖励
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="accountId"></param>
        /// <param name="totalMoney"></param>
        /// <param name="strongModule"></param>
        /// <param name="allInput">用户所有输入的信息</param>
        /// <returns></returns>
        private void SendActivityReward(int activityId, int accountId, decimal totalMoney,
            BaseModule strongModule, object allInput, string ip)
        {
            try
            {
                //TODO：数据库储存
                var appRedPackageActivityAwardLogService =
                    ObjectFactory.GetInstance<APP_RedPackage_Activity_LogService>();
                //var withdrawLogService = ObjectFactory.GetInstance<WithdrawLogService>();
                var appRedPackageActivityService = ObjectFactory.GetInstance<APP_RedPackage_ActivityService>();

                //TODO:判断余额是否足够 [启用分布式锁]
                var fullAccountWorkFlowCache = ObjectFactory.GetInstance<FullAccountWorkFlowCache>();
                using (fullAccountWorkFlowCache.Cache.BeginCacheLock(FullAccountCache.CACHE_KEY, activityId.ToString()))
                {
                    // var withdrawMoney = withdrawLogService.GetSum(z => z.ActivityId == activityId, z => z.Money);
                    var activity = appRedPackageActivityService.GetObject(z => z.Id == activityId);
                    if (totalMoney > activity.RemainingMoney) //是否大于剩余的金额
                    {
                        //TODO:停止该活动
                        throw new Exception("哎呀,失之交臂！");
                    }
                    //添加账户余额
                    var accountService = ObjectFactory.GetInstance<AccountService>();
                    var account = accountService.GetObject(z => z.Id == accountId);
                    if (account != null)
                    {
                        account.Wallet += totalMoney;
                    }
                    //TODO: 开启事务
                    using (TransactionScope scopt = new TransactionScope())
                    {
                        //TODO:获取他输入的所有的Input
                        var activityLog =
                            appRedPackageActivityAwardLogService.CreateAppRedPackageActivityAwardLog(activityId,
                                accountId, totalMoney, strongModule.Output[0].Value.ToString(),
                                allInput.SerializeToString());
                        accountService.SaveObject(account);
                        activity.RemainingMoney -= totalMoney;
                        appRedPackageActivityService.SaveObject(activity); //更新剩余金额

                        #region 企业付款

                        ////TODO:订单号长度
                        ////生成订单号
                        //var orderNumber = "TX_{2}_{0}_{1}".With(DateTime.Now.ToString("yyyyMMdd"), activityId,
                        //    Guid.NewGuid().ToString("N"));
                        ////调用企业付款 
                        //withdrawLogService.RequestToPayV3(orderNumber, accountId, activityLog.ActivityId, activityLog.Id,
                        //    totalMoney, ip);

                        #endregion

                        scopt.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtility.DebugLogger.InfoFormat("Error:{0}", ex.StackTrace.ToString());
                throw;
            }
        }

        /// <summary>
        /// 获取Api返回的Input[根据下面所有的工作项需要返回]
        /// </summary>
        /// <param name="fullAccountWorkFlow"></param>
        /// <param name="picValue"></param>
        /// <param name="currentModule"></param>
        /// <returns></returns>
        private async Task<Input> GetMoodsApiModuleInputAsync(FullAccountWorkFlow fullAccountWorkFlow, string picValue,
            BaseModule currentModule)
        {
            if (picValue == null || fullAccountWorkFlow.Input.All(z => z.Name != picValue))
            {
                throw new Exception("输入参数节点有误认知服务{0}！".With(currentModule.Step));
            }
            var inputPic = fullAccountWorkFlow.Input.FirstOrDefault(z => z.Name == picValue);
            var picUrl = Convert.ToString(inputPic.Value);
            var absoluteUrl = Server.GetMapPath(picUrl.StartsWith("~/") ? picUrl : "~" + picUrl);
            var emotionArray = await EmotionApi.UploadAndStreamDetectEmotionsAsync(absoluteUrl); //TODO:调用Azure情绪识别API
            if (emotionArray == null || emotionArray.Length <= 0)
            {
                Log.LogUtility.DebugLogger.Info("图片路径：{0}！".With(absoluteUrl));
                throw new Exception("您上传的图片没有监测到面部表情！");
            }
            var currentInput = GetEmotionInput(emotionArray[0]);
            return currentInput;
        }

        /// <summary>
        /// 获取表情Input
        /// </summary>
        /// <param name="emotion"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public Input GetEmotionInput(Emotion emotion)
        {
            var currentInput = new Input();
            foreach (var emotionTypeName in Enum.GetNames(typeof(Emotion_Type)))
            {
                var emotionType = (Emotion_Type)Enum.Parse(typeof(Emotion_Type), emotionTypeName);
                float emotionScore;
                switch (emotionType)
                {
                    case Emotion_Type.愤怒:
                        emotionScore = emotion.Scores.Anger;
                        break;
                    case Emotion_Type.轻蔑:
                        emotionScore = emotion.Scores.Contempt;
                        break;
                    case Emotion_Type.厌恶:
                        emotionScore = emotion.Scores.Disgust;
                        break;
                    case Emotion_Type.恐惧:
                        emotionScore = emotion.Scores.Fear;
                        break;
                    case Emotion_Type.快乐:
                        emotionScore = emotion.Scores.Happiness;
                        break;
                    case Emotion_Type.无表情:
                        emotionScore = emotion.Scores.Neutral;
                        break;
                    case Emotion_Type.悲伤:
                        emotionScore = emotion.Scores.Sadness;
                        break;
                    case Emotion_Type.惊讶:
                        emotionScore = emotion.Scores.Surprise;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                currentInput.Add(new NumberParameterSetting(emotionTypeName, emotionScore));
            }
            return currentInput;
        }

        /// <summary>
        /// 获取计算机视觉
        /// </summary>
        /// <param name="picUrl"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Input GetVisionInput(string picUrl, string name)
        {
            var absoluteUrl = Server.GetMapPath(picUrl.StartsWith("~/") ? picUrl : "~" + picUrl);
            var result = VisionApi.UploadAndGetTagsForImage(absoluteUrl);
            var data = new VisionParameters()
            {
                Pic = picUrl,
                Result =
                    result != null
                        ? result.Tags.Select(z => new VisionTags(z.Confidence, z.Hint, z.Name)).ToList()
                        : new List<VisionTags>()
            };
            var currentInput = new Input() { new VisionApiParameterSetting(name, data) };
            return currentInput;
        }

        /// <summary>
        /// 获取用户输入参数Key
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public string GetParaeterKey(string moduleName, string inputName)
        {
            return "{0} > {1}".With(moduleName, inputName);
        }
    }
}