using System;
using System.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Senparc.AzureCognitiveService;
using Senparc.AzureCognitiveTests;
using Senparc.Core.Extensions;

namespace Senparc.AzureCognitiveServiceTests
{
    [TestClass()]
    public class EmotionApiTests : BaseCognitiveTest
    {
        [TestMethod()]
        public void EmotionServiceTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UploadAndStreamDetectEmotionsTest()
        {
            //EmotionApi emotionService = new EmotionApi(base.subscriptionKey);
            //var imageFilePath = @"E:\Senparc\AzureDemo\WebSite\Senparc.Web\Upload\Emotion\happy.jpg";
            //Console.WriteLine("Begin Time: {0}", DateTime.Now);
            //var result = EmotionApi.UploadAndStreamDetectEmotions(imageFilePath);
            //Console.WriteLine("End Time: {0}", DateTime.Now);
            //Assert.IsNotNull(result);
            //foreach (var item in result)
            //{
            //    Console.WriteLine(
            //        "FaceRectangle=======Height：{0}====Width：{1}====Left：{2}====Top：{3}".With(
            //            item.FaceRectangle.Height, item.FaceRectangle.Width,
            //            item.FaceRectangle.Left, item.FaceRectangle.Top));
            //    foreach (var keyValuePair in item.Scores.ToRankedList())
            //    {
            //        Console.WriteLine("Scores===={0}：{1}".With(keyValuePair.Key, keyValuePair.Value));
            //    }
            //    Console.WriteLine("=============================");

            //}
        }

        [TestMethod()]
        public void UploadAndUrlDetectEmotionsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void UploadAndVideoDetectEmotionsTest()
        {
            Assert.Fail();
        }
    }
}