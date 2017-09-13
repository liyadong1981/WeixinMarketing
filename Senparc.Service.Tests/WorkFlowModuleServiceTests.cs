using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.AzureCognitiveService;
using Senparc.Core.Extensions;
using Senparc.Core.Models.WorkFlowModules;
using Senparc.Core.Tests;
using ServiceStack;
using StructureMap;

namespace Senparc.Service.Tests
{
    [TestClass]
    public class WorkFlowModuleServiceTests : BaseCoreTest
    {
        private readonly WorkFlowModuleService _workFlowModuleService;

        public WorkFlowModuleServiceTests()
        {
            _workFlowModuleService = ObjectFactory.GetInstance<WorkFlowModuleService>();
        }

        [TestMethod]
        public void SubmitStepTest()
        {
            //var activityId = 3;
            //var accountId = 1;
            //var weixinOpenId = "o41Y_v9akWxjqMX-qxxfeuGOUzAg";
            //var currentStep = -1;
            //var nextStep = currentStep + 1;

            //Console.WriteLine(
            //    "AccountWorkFlowDictionary.Length:{0}".With(WorkFlowModuleService.AccountWorkFlowDictionary.Count));
            ////_workFlowModuleService.CreateAccountWorkFlow(activityId, accountId, weixinOpenId);

            //var pic = @"D:\Azure认知服务\Emotion\happy.jpg";
            ////var modules = new List<BaseModule>();
            ////var registerModule = new RegisterModule
            ////{
            ////    Id = "1",
            ////    Input = new Input()
            ////    {
            ////        new ParameterSetting(ParameterSettingType.Pic, "照片", pic)
            ////    }
            ////};
            ////var moodisApiModule = new EmotionApiModule
            ////{
            ////    Id = "2",
            ////};
            ////var resultModule = new ResultModule
            ////{
            ////    Id = "3",
            ////    Input = new Input() {new EmotionApiParameterSetting("快乐", "心情API.快乐")}
            ////};
            ////moodisApiModule.Input[0].Value = "登记.照片";

            ////modules.Add(registerModule);
            ////modules.Add(moodisApiModule);
            ////modules.Add(resultModule);
            ////WorkFlowModuleService.AccountWorkFlowDictionary.Add("{0}.{1}".With(activityId, weixinOpenId), modules);
            //var dt1 = DateTime.Now;
            //_workFlowModuleService.CreateAccountWorkFlow(activityId, accountId);
            //var registerModuleReulst = _workFlowModuleService.SubmitStep(currentStep, nextStep, new Input(), activityId,
            //    accountId, "");
            //currentStep++;
            //nextStep++;

            //Assert.IsNotNull(registerModuleReulst);
            //Console.WriteLine("===========【Type={0}   Value={1}  NextStep={2}】=========".With(registerModuleReulst.Type,
            //    registerModuleReulst.Value, registerModuleReulst.NextStep));
            //Console.WriteLine(
            //    "AccountWorkFlowDictionary.Length:{0}".With(WorkFlowModuleService.AccountWorkFlowDictionary.Count));
            //Console.WriteLine("=============================================");
            //var input = new Input
            //{
            //    new PicParameterSetting("照片", pic),
            //    new StringParameterSetting("姓名", "张三"),
            //    new StringParameterSetting("电话", "18888668888")
            //};
            //var dt2 = DateTime.Now;
            //var resultModuleResult = _workFlowModuleService.SubmitStep(currentStep, nextStep, input, activityId,
            //    accountId, "");
            //Assert.IsNotNull(resultModuleResult);
            //Console.WriteLine("===================调用API：{0}".With((DateTime.Now - dt2).TotalMilliseconds));
            //Console.WriteLine("===========【Type={0}   Value={1}  NextStep={2}】=========".With(resultModuleResult.Type,
            //    resultModuleResult.Value, resultModuleResult.NextStep));
            //Console.WriteLine("================SpaceTime:{0}".With((DateTime.Now - dt2).TotalMilliseconds));
        }

        [TestMethod]
        public void CreateAccountWorkFlowTest()
        {
            //var activityId = 1;
            //var accountId = 1;
            //var weixinOpenId = "o41Y_v9akWxjqMX-qxxfeuGOUzAg";
            //var modules = new List<BaseModule>();
            //var registerModule = new RegisterModule
            //{
            //    Id = "1",
            //    Input = new Input
            //    {
            //        new PicParameterSetting("照片", null)
            //    }
            //};
            //var moodisApiModule = new EmotionApiModule
            //{
            //    Id = "2"
            //};
            //var resultModule = new RedPackageResultModule
            //{
            //    Id = "3",
            //    Input = new Input { new EmotionApiParameterSetting("快乐", "心情API.快乐") }
            //};
            //moodisApiModule.Input[0].Value = "登记.照片";


            //modules.Add(registerModule);
            //modules.Add(moodisApiModule);
            //modules.Add(resultModule);
            //var activityService = ObjectFactory.GetInstance<APP_RedPackage_ActivityService>();
            //var activity = activityService.GetObject(z => z.Id == activityId);
            //var dt1 = DateTime.Now;
            //activity.Rule = modules.SerializeToString();
            //Console.WriteLine("Serialization Time:{0}".With((DateTime.Now - dt1).TotalMilliseconds));
            //activityService.SaveObject(activity);
            //Console.WriteLine("Rule===={0}".With(activity.Rule.Length));
            //var curretnCount = WorkFlowModuleService.AccountWorkFlowDictionary.Count;
            //Console.WriteLine("AccountWorkFlowDictionary.Length:{0}".With(curretnCount));
            //var dt2 = DateTime.Now;
            //_workFlowModuleService.CreateAccountWorkFlow(activityId, accountId);
            //Console.WriteLine("Deserialization Time:{0}".With((DateTime.Now - dt2).TotalMilliseconds));
            //Assert.IsTrue(WorkFlowModuleService.AccountWorkFlowDictionary.Count > curretnCount);
            //Console.WriteLine(
            //    "AccountWorkFlowDictionary.Length:{0}".With(WorkFlowModuleService.AccountWorkFlowDictionary.Count));
            //Console.WriteLine(modules.ToJson());
            //Assert.IsNotNull(modules);
        }

        [TestMethod]
        public void GetEmotionInputTest()
        {
            var imagePath = @"E:\Senparc\AzureDemo\WebSite\Senparc.Web\Upload\Emotion\img.jpg";
            var dt1 = DateTime.Now;
            Console.WriteLine("Begin DateTime:{0}".With(dt1));

            for (var i = 0; i < 10; i++)
            {
                //Thread thread = new Thread(() =>
                //{
                var dt2 = DateTime.Now;
                Console.WriteLine("Async DateTime:{0}".With(dt2));
                var emotion = EmotionApi.UploadAndStreamDetectEmotionsAsync(imagePath);
                Console.WriteLine("=========={0}".With(emotion.Result.ToJson()));
                Console.WriteLine("End Async DateTime:{0}".With((DateTime.Now - dt1).TotalMilliseconds));
                //});
                //thread.Start();
                //var module = new EmotionApiModule();
                //var result = _workFlowModuleService.GetEmotionInput(emotion[0]);
                //Assert.IsNotNull(result);
                //foreach (var parameter in result)
                //{
                //    Console.WriteLine("=========={0}:{1}".With(parameter.Name, parameter.Value));
                //}
            }
            Console.WriteLine("End DateTime:{0}".With((DateTime.Now - dt1).TotalMilliseconds));
        }
    }
}